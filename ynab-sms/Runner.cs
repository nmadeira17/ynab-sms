using System;

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

        Console.WriteLine(budgetConfig.ToJson());
    }
}