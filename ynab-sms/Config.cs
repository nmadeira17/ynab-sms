using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

/// <summary>
/// Common functionality for entries in all config files
/// </summary>
public interface IConfigEntry
{
    bool IsValid(out string errorMessage);
}

///////////////////////////////////////////////////////////////////////////////////////////////////
//
// App Config
//
///////////////////////////////////////////////////////////////////////////////////////////////////
public class AppConfig : IConfigEntry
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("budget_items_json_file")]
    public string BudgetItemsJsonFile { get; set; }

    /// <summary>
    /// Initialize from the path to the app config json file
    /// </summary>
    public static AppConfig CreateFromJsonFile(string jsonFile)
    {
        string jsonString = "";
        AppConfig appConfig = null;

        try
        {
            jsonString = File.ReadAllText(jsonFile);
        }
        catch(Exception e)
        {
            Console.WriteLine(String.Format("Failed to read config.json. Error:\n{0}", e));
            return null;
        }

        try
        {
            appConfig = JsonSerializer.Deserialize<AppConfig>(jsonString);
        }
        catch(Exception e)
        {
            Console.WriteLine(String.Format("Failed to deserialize config.json.\nError: {0}", e));
            return null;
        }

        string errorMessage = "";
        if (!appConfig.IsValid(out errorMessage))
        {
            Console.WriteLine(String.Format("config.json is invalid. {0}", errorMessage));
            return null;
        }

        return appConfig;
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

        return JsonSerializer.Serialize<AppConfig>(this, options);
    }

    /// <summary>
    /// Is this object a structurall valid object
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (AccessToken.Length == 0)
        {
            errorMessage = "AccessToken must be present.";
            return false;
        }

        if (BudgetItemsJsonFile.Length == 0)
        {
            errorMessage = "Path to budget items json file must be present.";
            return false;
        }

        errorMessage = "NoError";
        return true;
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////
//
// BudgetItemsConfig
//
///////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Representst the entirety of the budget_items.json file
/// </summary>
public class BudgetItemsConfig : IConfigEntry
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
            Console.WriteLine(String.Format("Failed to read budget_items.json. Error:\n{0}", e));
            return null;
        }

        try
        {
            config = JsonSerializer.Deserialize<BudgetItemsConfig>(jsonString);
        }
        catch(Exception e)
        {
            Console.WriteLine(String.Format("Failed to deserialize budget_items.json.\nError: {0}", e));
            return null;
        }

        string errorMessage = "";
        if (!config.IsValid(out errorMessage))
        {
            Console.WriteLine(String.Format("budget_items.json is invalid. {0}", errorMessage));
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
public class EntryConfig : IConfigEntry
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
public class BudgetConfig : IConfigEntry
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
public class BudgetItemConfig : IConfigEntry
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