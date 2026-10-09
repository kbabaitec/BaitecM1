using System.Drawing;
using System.Windows.Forms;

namespace StoreClient;

public class MainForm : Form
{
    private readonly ItemApiClient _api = new();
    private readonly DataGridView grid = new();
    private readonly TextBox txtName = new(), txtCode = new(), txtBrand = new(), txtPrice = new(), txtSearch = new();
    private readonly Button btnAdd = new(), btnUpdate = new(), btnDelete = new(), btnClear = new(), btnSearch = new();
    private readonly Label lblStats = new();

    public MainForm()
    {
        Text = "Sari-Sari Store Item Manager";
        Size = new Size(900, 600);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(255, 248, 235);
        Font = new Font("Segoe UI", 10);

        var header = new Label
        {
            Text = "🛒  Sari-Sari Store Item Manager",
            Dock = DockStyle.Top,
            Height = 55,
            BackColor = Color.FromArgb(214, 69, 65),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(15, 0, 0, 0)
        };

        // ---- Input panel (left) ----
        var panel = new Panel { Dock = DockStyle.Left, Width = 260, Padding = new Padding(15) };
        int y = 15;
        void AddField(string label, TextBox box)
        {
            panel.Controls.Add(new Label { Text = label, Left = 15, Top = y, AutoSize = true });
            box.Left = 15; box.Top = y + 22; box.Width = 225;
            panel.Controls.Add(box);
            y += 60;
        }
        AddField("Name", txtName);
        AddField("Code (unique)", txtCode);
        AddField("Brand", txtBrand);
        AddField("Unit Price (₱)", txtPrice);

        void StyleButton(Button b, string text, Color c, int left, int top, int w)
        {
            b.Text = text; b.Left = left; b.Top = top; b.Width = w; b.Height = 36;
            b.BackColor = c; b.ForeColor = Color.White; b.FlatStyle = FlatStyle.Flat;
            panel.Controls.Add(b);
        }
        StyleButton(btnAdd, "Add", Color.SeaGreen, 15, y, 108);
        StyleButton(btnUpdate, "Update", Color.SteelBlue, 132, y, 108);
        StyleButton(btnDelete, "Delete", Color.Firebrick, 15, y + 45, 108);
        StyleButton(btnClear, "Clear", Color.Gray, 132, y + 45, 108);

        // ---- Search bar ----
        var searchPanel = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
        txtSearch.PlaceholderText = "Search by name, brand or code...";
        txtSearch.SetBounds(10, 12, 400, 28);
        btnSearch.Text = "Search"; btnSearch.SetBounds(420, 10, 100, 30);
        btnSearch.BackColor = Color.FromArgb(214, 69, 65); btnSearch.ForeColor = Color.White;
        btnSearch.FlatStyle = FlatStyle.Flat;
        searchPanel.Controls.AddRange(new Control[] { txtSearch, btnSearch });

        // ---- Grid ----
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = Color.White;
        grid.RowHeadersVisible = false;

        lblStats.Dock = DockStyle.Bottom; lblStats.Height = 30;
        lblStats.TextAlign = ContentAlignment.MiddleLeft; lblStats.Padding = new Padding(10, 0, 0, 0);

        Controls.Add(grid);
        Controls.Add(lblStats);
        Controls.Add(searchPanel);
        Controls.Add(panel);
        Controls.Add(header);

        // ---- Events ----
        Load += async (_, _) => await LoadItems();
        btnSearch.Click += async (_, _) => await LoadItems(txtSearch.Text);
        btnClear.Click += (_, _) => ClearForm();
        btnAdd.Click += async (_, _) => await Save(isNew: true);
        btnUpdate.Click += async (_, _) => await Save(isNew: false);
        btnDelete.Click += async (_, _) => await Remove();
        grid.SelectionChanged += (_, _) => FillFormFromGrid();
    }

    private async Task LoadItems(string? search = null)
    {
        try
        {
            var items = await _api.GetAllAsync(search);
            grid.DataSource = items;
            lblStats.Text = items.Count == 0
                ? "No items found."
                : $"Total items: {items.Count}   |   Average price: ₱{items.Average(i => i.UnitPrice):N2}   |   Highest: ₱{items.Max(i => i.UnitPrice):N2}";
        }
        catch (Exception ex)
        {
            MessageBox.Show("Cannot reach the API. Is it running?\n\n" + ex.Message, "Connection error");
        }
    }

    private void FillFormFromGrid()
    {
        if (grid.CurrentRow?.DataBoundItem is Item i)
        {
            txtName.Text = i.Name; txtCode.Text = i.Code; txtBrand.Text = i.Brand;
            txtPrice.Text = i.UnitPrice.ToString("0.00");
            txtCode.ReadOnly = true; // code is the key, can't be edited
        }
    }

    private void ClearForm()
    {
        txtName.Clear(); txtCode.Clear(); txtBrand.Clear(); txtPrice.Clear();
        txtCode.ReadOnly = false; grid.ClearSelection();
    }

    private async Task Save(bool isNew)
    {
        if (!decimal.TryParse(txtPrice.Text, out var price) || price <= 0)
        { MessageBox.Show("Enter a valid unit price."); return; }
        if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtBrand.Text))
        { MessageBox.Show("Name, Code and Brand are required."); return; }

        var item = new Item { Name = txtName.Text.Trim(), Code = txtCode.Text.Trim(), Brand = txtBrand.Text.Trim(), UnitPrice = price };
        var error = isNew ? await _api.AddAsync(item) : await _api.UpdateAsync(item);
        if (error != null) { MessageBox.Show(error, "Error"); return; }

        ClearForm();
        await LoadItems();
    }

    private async Task Remove()
    {
        if (string.IsNullOrWhiteSpace(txtCode.Text)) { MessageBox.Show("Select an item first."); return; }
        if (MessageBox.Show($"Delete item {txtCode.Text}?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        await _api.DeleteAsync(txtCode.Text);
        ClearForm();
        await LoadItems();
    }
}