using System;
using System.Collections.Generic;

using Ynab_Sms.Logging;

namespace Ynab_Sms
{
    public static class Runner
    {
        public static void Run(string configFilePath)
        {
            AppConfig appConfig = AppConfig.CreateFromJsonFile(configFilePath);
            if (appConfig == null)
                return;

            BudgetItemsConfig budgetItemsConfig = BudgetItemsConfig.CreateFromJson(appConfig.BudgetItemsJsonFile);
            if (budgetItemsConfig == null)
                return;

            IEnumerable<BudgetDetails> budgetDetails = YnabApi.GetBudgets(appConfig.AccessToken, budgetItemsConfig.GetBudgetIds());
            Dictionary<string, ICollection<string>> ret = FindItemsSpecifiedInConfig(budgetItemsConfig, budgetDetails);

            foreach (string phoneNumber in ret.Keys)
            {
                Logger.Log(phoneNumber);

                foreach (string msg in ret[phoneNumber])
                {
                    Logger.Log(String.Format("\t- {0}", msg));
                }
            }
        }

        private static Dictionary<string, ICollection<string>> FindItemsSpecifiedInConfig(BudgetItemsConfig budgetItemsConfig, IEnumerable<BudgetDetails> budgetDetails)
        {
            Dictionary<string, ICollection<string>> ret = new Dictionary<string, ICollection<string>>();

            foreach (BudgetDetails budgetDetail in budgetDetails)
            {
                foreach (CategoryGroup categoryGroup in budgetDetail.CategoryGroups)
                {
                    foreach (Category category in categoryGroup.Categories)
                    {
                        ICollection<string> phoneNumbersThatCare = budgetItemsConfig.GetPhoneNumbersThatRegisteredForBudgetItem(budgetDetail.Id, categoryGroup.Name, category.Name);
                        if (phoneNumbersThatCare.Count == 0)
                            continue;

                        string catName = categoryGroup.Name + " | " + category.Name;
                        string balance = Utils.YnabLongToFormattedString(category.Remaining);
                        string entry = String.Format("{0}: {1}", catName, balance);

                        foreach (string phoneNumber in phoneNumbersThatCare)
                        {
                            if (!ret.ContainsKey(phoneNumber))
                                ret.Add(phoneNumber, new List<string>());

                            ret[phoneNumber].Add(entry);
                        }
                    }
                }
            }

            return ret;
        }
    }
}