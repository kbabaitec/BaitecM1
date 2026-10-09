using System.Collections.Concurrent;
namespace StoreApi.Models;

public class ItemRepository
{
    private readonly ConcurrentDictionary<string, Item> _items = new(StringComparer.OrdinalIgnoreCase);
    public ItemRepository()
    {
        Add(new Item
        {
            Name = "Instant Noodles Chicken",
            Code = "NDL-001",
            Brand = "Lucky Me",
            UnitPrice
             = 14.50m
        });
        Add(new Item
        {
            Name = "3-in-1 Coffee Original",
            Code = "COF-001",
            Brand = "Nescafe",
            UnitPrice
             = 8.00m
        });
        Add(new Item
        {
            Name = "Laundry Bar 350g",
            Code = "SOP-001",
            Brand = "Tide",
            UnitPrice
             = 22.00m
        });
        Add(new Item
        {
            Name = "Softdrink 1L",
            Code = "DRK-001",
            Brand = "Coca-Cola",
            UnitPrice
             = 45.00m
        });
    }
    public IEnumerable<Item> GetAll(string? search = null) =>
        _items.Values
              .Where(i => string.IsNullOrWhiteSpace(search)
                       || i.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                       || i.Brand.Contains(search, StringComparison.OrdinalIgnoreCase)
                       || i.Code.Contains(search, StringComparison.OrdinalIgnoreCase))
              .OrderBy(i => i.Code);
    public Item? Get(string code) => _items.TryGetValue(code, out var i) ? i : null;
    public bool Add(Item item) => _items.TryAdd(item.Code, item);
    public bool Update(string code, Item item) => _items.ContainsKey(code) && (_items[code] = item) !=
        null;
    public bool Delete(string code) => _items.TryRemove(code, out _);
}