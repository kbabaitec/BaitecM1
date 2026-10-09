using Microsoft.AspNetCore.Mvc;
using StoreApi.Models;
namespace StoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly ItemRepository _repo;
    public ItemsController(ItemRepository repo) => _repo = repo;
    // GET api/items?search=lucky
    [HttpGet]
    public ActionResult<IEnumerable<Item>> GetAll([FromQuery] string? search) =>
        Ok(_repo.GetAll(search));
    // GET api/items/NDL-001
    [HttpGet("{code}")]
    public ActionResult<Item> GetByCode(string code)
    {
        var item = _repo.Get(code);
        return item is null ? NotFound($"Item '{code}' not found.") : Ok(item);
    }
    // POST api/items
    [HttpPost]
    public ActionResult<Item> Create(Item item)
    {
        if (!_repo.Add(item)) return Conflict($"Item code '{item.Code}' already exists.");
        return CreatedAtAction(nameof(GetByCode), new { code = item.Code }, item);
    }
    // PUT api/items/NDL-001
    [HttpPut("{code}")]
    public IActionResult Update(string code, Item item)
    {
        item.Code = code;
        return _repo.Update(code, item) ? NoContent() : NotFound($"Item '{code}' not found.");
    }
    // DELETE api/items/NDL-001
    [HttpDelete("{code}")]
    public IActionResult Delete(string code) =>
        _repo.Delete(code) ? NoContent() : NotFound($"Item '{code}' not found.");
}