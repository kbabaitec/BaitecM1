using System.ComponentModel.DataAnnotations;
namespace StoreApi.Models;

public class Item
{
    [Required] public string Name { get; set; } = "";
    [Required] public string Code { get; set; } = "";
    [Required] public string Brand { get; set; } = "";
    [Range(0.01, 1000000)] public decimal UnitPrice { get; set; }
}
