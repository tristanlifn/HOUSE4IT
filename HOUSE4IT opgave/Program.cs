using System.Collections;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace HOUSE4IT_opgave;

internal class Program
{
    public static void Main()
    {
        List<EditedItems> editedItemsList = []; 
        
        string[] files = ["prisliste1.csv", "prisliste2.csv"];
        foreach (string file in files)
        {
            List<ItemObject>? originalItems = ReadItems(file);
            if (originalItems != null)
            {
                List<EditedItems> editedItems = EditPrice(originalItems);
                editedItems = EditItemName(editedItems);
                editedItemsList.AddRange(editedItems);
            }
        }
        
        WriteItems(editedItemsList);
    }

    /// <summary>
    /// Opens the specified file and converts it to a list
    /// </summary>
    /// <param name="fileName">The name of the csv file, including file extension</param>
    private static List<ItemObject>? ReadItems(string fileName)
    {
        CsvConfiguration config = new(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null, // makes it not care if a header is not found
            MissingFieldFound = null, // Makes it not care if data for a field is not found
            Delimiter = ";"
        };

        List<ItemObject>? originalItems = null;
        try
        {
            using StreamReader reader = new(fileName);
            using CsvReader csv = new(reader, config);
            IEnumerable<ItemObject> records = csv.GetRecords<ItemObject>();
            originalItems = new(records); // Converts the IEnumerable to a list, to avoid "Possible multiple enumerations of IEnumerable collection" (CA1851)
        }
        catch
        {
            Console.WriteLine("ERROR: File not found: " + fileName);
        }
        return originalItems;
    }

    /// <summary>
    /// Edits the price of an item to the respective pricegroup, and converts to dkk
    /// </summary>
    /// <param name="items">A list of ItemObjects to modify</param>
    /// <returns>The modified list</returns>
    private static List<EditedItems> EditPrice(List<ItemObject> items)
    {
        List<EditedItems> editedItems = [];
        foreach (ItemObject item in items)
        {
            decimal newPrice;
            decimal[] groupPrecent = [30, 50, 40, 40, 50];
            decimal price = decimal.Parse(item.KostprisEUR.Replace(" €", ""));
            try
            {
                newPrice = price * (groupPrecent[item.PriceGroup - 21] / 100 + 1); // minus the PriceGroup with 21 to get it to align with index 0
            }
            catch
            {
                Console.WriteLine($"PriceGroup for item {item.Item} is invalid, using default (30%)");
                newPrice = price * 1.3m;
            }
            decimal newPriceDkk = newPrice * 746 / 100; // Convert the price to dkk
            newPriceDkk = Math.Round(newPriceDkk, 2);
            
            item.KostprisEUR = newPriceDkk.ToString("C");
            
            editedItems.Add(new EditedItems
            {
                Varenummer = item.Item,
                Navn = item.ArticleDescription,
                KostIDkk = newPriceDkk + " DKK",
                BeregnetSalgsPrisIDkk = (newPriceDkk * item.PriceUnit) + " DKK",
            });
        }
        return editedItems;
    }
    
    /// <summary>
    /// Replaces all instenced of "black" or "Balck" with "sort" or "Sort" 
    /// </summary>
    /// <param name="items">A list of ItemObjects to modify</param>
    /// <returns>The modified list</returns>
    private static List<EditedItems> EditItemName(List<EditedItems> items)
    {
        foreach (EditedItems item in items)
        {
            item.Navn = item.Navn.Replace("black", "sort").Replace("Black", "Sort");
        }
        return items;
    }

    /// <summary>
    /// Writes the list of all edited items to a new file, overwrites if there is an existing file
    /// </summary>
    /// <param name="items">A list of edited items to write to the file</param>
    private static void WriteItems(List<EditedItems> items)
    {
        CsvConfiguration config = new(CultureInfo.InvariantCulture) { Delimiter = ";" }; // Sets the Delimiter to ";"
        
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outPath = Path.GetFullPath(Path.Combine(desktopPath, @"..\", @"Downloads\ny_prisliste.csv")); // Sets the path of the file to the current users downloads folder

            using StreamWriter writer = new(outPath);
            using CsvWriter csv = new(writer, config);
            csv.WriteRecords(items);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

