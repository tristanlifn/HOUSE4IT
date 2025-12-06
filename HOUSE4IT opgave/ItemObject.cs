using CsvHelper.Configuration.Attributes;

namespace HOUSE4IT_opgave;

/// <summary>
/// a class to hold all the data for each item
/// </summary>
public class ItemObject
{
    public string Item { get; set; } = string.Empty;
    public string articledescription { get; set; } = string.Empty;
    public string unit { get; set; } = string.Empty;
    public string kostprisEUR { get; set; } = string.Empty;
    public int priceunit { get; set; }
    public int pricegroup {  get; set; }
    public DateOnly dateofissuance { get; set; }
}