using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

using Ynab_Sms.Logging;

namespace Ynab_Sms
{
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

        [JsonPropertyName("budget_config_json_file")]
        public string BudgetConfigJsonFile { get; set; }

        [JsonPropertyName("twilio_sid")]
        public string TwilioSid { get; set; }

        [JsonPropertyName("twilio_auth_token")]
        public string TwilioAuthToken { get; set; }

        [JsonPropertyName("twilio_phone_number")]
        public string TwilioPhoneNumber { get; set; }

        /// <summary>
        /// Initialize from the path to the app config json file
        /// </summary>
        public static AppConfig CreateFromJsonFile(string jsonFile)
        {
            string jsonString = "";
            AppConfig appConfig = null;

            Logger.Log("Parsing config file...", LoggingLevel.Verbose);

            try
            {
                jsonString = File.ReadAllText(jsonFile);
            }
            catch(Exception e)
            {
                Logger.Log(String.Format("Failed to read config.json. Error:\n{0}", e));
                return null;
            }

            try
            {
                appConfig = JsonSerializer.Deserialize<AppConfig>(jsonString);
            }
            catch(Exception e)
            {
                Logger.Log(String.Format("Failed to deserialize config.json.\nError: {0}", e));
                return null;
            }

            string errorMessage = "";
            if (!appConfig.IsValid(out errorMessage))
            {
                Logger.Log(String.Format("config.json is invalid. {0}", errorMessage));
                return null;
            }

            Logger.Log("Done!", LoggingLevel.Verbose);

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

            if (BudgetConfigJsonFile.Length == 0)
            {
                errorMessage = "Path to budget items json file must be present.";
                return false;
            }

            if (TwilioSid.Length == 0)
            {
                errorMessage = "TwilioSid must be present.";
                return false;
            }

            if (TwilioAuthToken.Length == 0)
            {
                errorMessage = "TwilioAuthToken must be present.";
                return false;
            }

            if (TwilioPhoneNumber.Length == 0)
            {
                errorMessage = "TwilioPhoneNumber must be present.";
                return false;
            }

            errorMessage = "NoError";
            return true;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // BudgetConfigFile
    //
    ///////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Representst the entirety of the budget_config.json file
    /// </summary>
    public class BudgetConfigFile : IConfigEntry
    {
        [JsonPropertyName("users")]
        public IList<UserConfig> UserConfigs { get; set; }

        /// <summary>
        /// Initialize an instance of BudgetConfigFile from a JSON file
        /// </summary>
        /// <param name="jsonFile">The path to the input JSON file</param>
        public static BudgetConfigFile CreateFromJson(string jsonFile)
        {
            string jsonString = "";
            BudgetConfigFile config = null;

            Logger.Log("Parsing budget_config.json...", LoggingLevel.Verbose);

            try
            {
                jsonString = File.ReadAllText(jsonFile);
            }
            catch(Exception e)
            {
                Logger.Log(String.Format("Failed to read budget_config.json. Error:\n{0}", e));
                return null;
            }

            try
            {
                config = JsonSerializer.Deserialize<BudgetConfigFile>(jsonString);
            }
            catch(Exception e)
            {
                Logger.Log(String.Format("Failed to deserialize budget_config.json.\nError: {0}", e));
                return null;
            }

            string errorMessage = "";
            if (!config.IsValid(out errorMessage))
            {
                Logger.Log(String.Format("budget_config.json is invalid. {0}", errorMessage));
                return null;
            }

            Logger.Log("Done!", LoggingLevel.Verbose);

            return config;
        }

        /// <summary>
        /// Collect the IDs for the unique budgets specified in the config file
        /// </summary>
        public IEnumerable<string> GetBudgetIds()
        {
            HashSet<string> budgetIds = new HashSet<string>();
            foreach (UserConfig userConfig in UserConfigs)
            {
                foreach (BudgetConfig budgetConfig in userConfig.BudgetConfigs)
                {
                    budgetIds.Add(budgetConfig.Id);
                }
            }

            return budgetIds;
        }

        /// <summary>
        /// Traverse the configuration file data structures for phone numbers that declared interest in an item
        /// </summary>
        public ICollection<string> GetPhoneNumbersThatRegisteredForBudgetItem(Guid budgetId, string categoryGroupName, string categoryName)
        {
            IList<string> phoneNumbersThatCare = new List<string>();

            foreach (UserConfig userConfig in UserConfigs)
            {
                string phoneNumber = userConfig.GetPhoneNumberIfRegisteredForBudgetItem(budgetId, categoryGroupName, categoryName);
                if (phoneNumber == null)
                    continue;
                
                phoneNumbersThatCare.Add(phoneNumber);
            }

            return phoneNumbersThatCare;
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

            return JsonSerializer.Serialize<BudgetConfigFile>(this, options);
        }

        /// <summary>
        /// Is this object a structurall valid object
        /// </summary>
        public bool IsValid(out string errorMessage)
        {
            if (UserConfigs.Count == 0)
            {
                errorMessage = "UserConfigs list is empty.";
                return false;
            }

            foreach (UserConfig userConfig in UserConfigs)
            {
                if (!userConfig.IsValid(out errorMessage))
                    return false;
            }

            errorMessage = "NoError";
            return true;
        }
    }

    /// <summary>
    /// Details for a single user in the config file
    /// The phone number to send an SMS to and the list of budgets they care about
    /// </summary>
    public class UserConfig : IConfigEntry
    {
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("budgets")]
        public IList<BudgetConfig> BudgetConfigs { get; set; }

        public string GetPhoneNumberIfRegisteredForBudgetItem(Guid budgetId, string categoryGroupName, string categoryName)
        {
            foreach (BudgetConfig budgetConfig in BudgetConfigs)
            {
                if (budgetConfig.ContainsEntry(budgetId, categoryGroupName, categoryName))
                    return PhoneNumber;
            }

            return null;
        }

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

            if (BudgetConfigs.Count == 0)
            {
                errorMessage = "BudgetConfigs list is empty";
                return false;
            }

            foreach (BudgetConfig budgetConfig in BudgetConfigs)
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
        public IList<BudgetCategoryConfig> BudgetCategoryConfigs { get; set; }

        public bool ContainsEntry(Guid budgetId, string categoryGroupName, string categoryName)
        {
            if (budgetId.ToString() != Id)
                return false;

            foreach (BudgetCategoryConfig budgetCategoryConfig in BudgetCategoryConfigs)
            {
                if (budgetCategoryConfig.IsEqual(categoryGroupName, categoryName))
                    return true;
            }

            return false;
        }

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

            if (BudgetCategoryConfigs.Count == 0)
            {
                errorMessage = "Budget items list is empty";
                return false;
            }

            foreach (BudgetCategoryConfig budgetCategoryConfig in BudgetCategoryConfigs)
            {
                if (!budgetCategoryConfig.IsValid(out errorMessage))
                    return false;
            }

            errorMessage = "NoError";
            return true;
        }
    }

    /// <summary>
    /// Specifies a single category group and category
    /// </summary>
    public class BudgetCategoryConfig : IConfigEntry
    {
        [JsonPropertyName("category_group")]
        public string CategoryGroup { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        public bool IsEqual(string categoryGroupName, string categoryName)
        {
            if (categoryGroupName == CategoryGroup && categoryName == Category)
                return true;

            return false;
        } 

        /// <summary>
        /// Is this object a structurall valid object
        /// </summary>
        public bool IsValid(out string errorMessage)
        {
            if (CategoryGroup.Length == 0)
            {
                errorMessage = "CategoryGroup must be specified.";
                return false;
            }

            if (Category.Length == 0)
            {
                errorMessage = "Category must be specified.";
                return false;
            }

            errorMessage = "NoError";
            return true;
        }
    }
}