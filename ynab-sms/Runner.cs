using System;
using System.Collections.Generic;
using System.Linq;

namespace Ynab_Sms
{
    public static class Runner
    {
        public static void Run(string configFilePath)
        {
            AppConfig appConfig = AppConfig.CreateFromJsonFile(configFilePath);
            if (appConfig == null)
                return;

            BudgetItemsConfig budgetConfig = BudgetItemsConfig.CreateFromJson(appConfig.BudgetItemsJsonFile);
            if (budgetConfig == null)
            {
                return;
            }

            IEnumerable<BudgetDetails> budgetDetails = YnabApi.GetBudgets(appConfig.AccessToken, budgetConfig.GetBudgetIds());
            foreach (BudgetDetails budgetDetail in budgetDetails)
            {
                Console.WriteLine("\n" + budgetDetail.ToString());
            }
        }
    }
}