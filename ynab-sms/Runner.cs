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
            MessageContent messageContent = MessageContent.Create(budgetItemsConfig, budgetDetails);

            Logger.Log(messageContent.ToString());
        }
    }
}