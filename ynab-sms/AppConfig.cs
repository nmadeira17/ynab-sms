using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

public class AppConfig
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("budget_items_json_file")]
    public string BudgetItemsJsonFile { get; set; }

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