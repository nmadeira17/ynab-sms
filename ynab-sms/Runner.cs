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

            BudgetItemsConfig budgetConfig = BudgetItemsConfig.CreateFromJson(appConfig.BudgetItemsJsonFile);
            if (budgetConfig == null)
            {
                return;
            }

            IEnumerable<BudgetDetails> budgetDetails = YnabApi.GetBudgets(appConfig.AccessToken, budgetConfig.GetBudgetIds());
            foreach (BudgetDetails budgetDetail in budgetDetails)
            {
                Logger.Log("\n" + budgetDetail.ToString(), LoggingLevel.Verbose);
            }
        }
    }
}