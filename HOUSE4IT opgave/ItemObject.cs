namespace HOUSE4IT_opgave;

/// <summary>
/// a class to hold all the data for each item
/// </summary>
public class ItemObject
{
    public string Item { get; set; } = string.Empty;
    public string ArticleDescription { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string KostprisEUR { get; set; } = string.Empty;
    public int PriceUnit { get; set; }
    public int PriceGroup {  get; set; }
    public DateOnly DateOfIssuance { get; set; }
}