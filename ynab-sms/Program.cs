using System;

namespace Ynab_Sms
{
    class Program
    {
        static void Main(string[] args)
        {
            string budgetItemsJsonFile = "../Config/budget_items.json";
            BudgetItemsConfig budgetConfig = BudgetItemsConfig.CreateFromJson(budgetItemsJsonFile);
            if (budgetConfig == null)
            {
                return;
            }

            Console.WriteLine(budgetConfig.ToJson());
        }
    }
}
