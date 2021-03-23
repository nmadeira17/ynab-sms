using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

/// <summary>
/// Common functionality for all entries in budget_items.json
/// </summary>
public interface IBudgetConfigEntry
{
    bool IsValid(out string errorMessage);
}

/// <summary>
/// Representst the entirety of the budget_items.json file
/// </summary>
public class BudgetItemsConfig : IBudgetConfigEntry
{
    [JsonPropertyName("entries")]
    public IList<EntryConfig> Entries { get; set; }

    /// <summary>
    /// Initialize an instance of BudgetItemsConfig from a JSON file
    /// </summary>
    /// <param name="jsonFile">The path to the input JSON file</param>
    public static BudgetItemsConfig CreateFromJson(string jsonFile)
    {
        string jsonString = "";
        BudgetItemsConfig config = null;

        try
        {
            jsonString = File.ReadAllText(jsonFile);
        }
        catch(Exception e)
        {
            Console.WriteLine(String.Format("Failed to read budget_items.json. Error: %s", e));
            return null;
        }

        try
        {
            config = JsonSerializer.Deserialize<BudgetItemsConfig>(jsonString);
        }
        catch(Exception e)
        {
            Console.WriteLine(String.Format("Failed to deserialize budget_items.json. Error: %s", e));
            return null;
        }

        return config;
    }

    /// <summary>
    /// Write the object to a JSON string
    /// </summary>
    public string ToJson(bool prettyPrint = false)
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = prettyPrint,
        };

        return JsonSerializer.Serialize<BudgetItemsConfig>(this, options);
    }

    /// <summary>
    /// Is this object a structurall valid object
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (Entries.Count == 0)
        {
            errorMessage = "Entries list is empty.";
            return false;
        }

        foreach (EntryConfig entry in Entries)
        {
            if (!entry.IsValid(out errorMessage))
                return false;
        }

        errorMessage = "NoError";
        return true;
    }
}

/// <summary>
/// Details for a single entry in the config.
/// The phone number to send an SMS to and the list of budgets they care about
/// </summary>
public class EntryConfig : IBudgetConfigEntry
{
    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("budgets")]
    public IList<BudgetConfig> Budgets { get; set; }

    /// <summary>
    /// Is this object a structurall valid object
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (PhoneNumber.Length != 10)
        {
            errorMessage = "Invalid phone number";
            return false;
        }

        if (Budgets.Count == 0)
        {
            errorMessage = "Budgets list is empty";
            return false;
        }

        foreach (BudgetConfig budgetConfig in Budgets)
        {
            if (!budgetConfig.IsValid(out errorMessage))
                return false;
        }

        errorMessage = "NoError";
        return true;
    }
}

/// <summary>
/// Specifies a budget and the items in the budget to care about
/// </summary>
public class BudgetConfig : IBudgetConfigEntry
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("budget_items")]
    public IList<BudgetItemConfig> BudgetItems { get; set; }

    /// <summary>
    /// Is this object a structurall valid object
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (Id.Length == 0)
        {
            errorMessage = "Must specify ID";
            return false;
        }

        if (BudgetItems.Count == 0)
        {
            errorMessage = "Budget items list is empty";
            return false;
        }

        foreach (BudgetItemConfig budgetItemConfig in BudgetItems)
        {
            if (!budgetItemConfig.IsValid(out errorMessage))
                return false;
        }

        errorMessage = "NoError";
        return true;
    }
}

/// <summary>
/// Specifies a single category and line item to process
/// </summary>
public class BudgetItemConfig : IBudgetConfigEntry
{
    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("line_item")]
    public string LineItem { get; set; }

    /// <summary>
    /// Is this object a structurall valid object
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (Category.Length == 0)
        {
            errorMessage = "Category must be specified.";
            return false;
        }

        if (LineItem.Length == 0)
        {
            errorMessage = "LineItem must be specified.";
            return false;
        }

        errorMessage = "NoError";
        return true;
    }
}