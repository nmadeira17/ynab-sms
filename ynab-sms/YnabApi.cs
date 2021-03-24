using System;
using System.Collections.Generic;
using System.Text;

namespace Ynab_Sms
{
    /// <summary>
    /// Wrapper class to interact with the YNAB.SDK APIs
    /// YNAB.SDK is what makes the network calls to hit YNAB servers
    /// </summary>
    public static class YnabApi
    {
        /// <summary>
        /// Make network calls to collect details about the budgets specified
        /// </summary>
        public static IEnumerable<BudgetDetails> GetBudgets(string accessToken, IEnumerable<string> budgetIds)
        {
            IList<BudgetDetails> budgetDetails = new List<BudgetDetails>();

            YNAB.SDK.API ynabApi = new YNAB.SDK.API(accessToken);

            foreach (string budgetId in budgetIds)
            {
                YNAB.SDK.Model.BudgetDetailResponse budgetResponse = ynabApi.Budgets.GetBudgetById(budgetId);
                if (budgetResponse == null)
                {
                    Console.WriteLine(String.Format("Error getting budget details for budget ID [{0}]. Are you sure this is the correct ID?"), budgetId);
                    continue;
                }

                budgetDetails.Add(BudgetDetails.Create(budgetResponse));
            }

            return budgetDetails;
        }
    }

    /// <summary>
    /// A single instance of a budget
    /// </summary>
    public class BudgetDetails
    {
        public string Name { get; private set; }
        public ICollection<CategoryGroup> CategoryGroups { get; private set; }

        public BudgetDetails(string name, ICollection<CategoryGroup> categoryGroups)
        {
            Name = name;
            CategoryGroups = categoryGroups;
        }

        /// <summary>
        /// Create an instance given the YNAB.SDK resposen object that comes from making network calls to YNAB servers
        /// </summary>
        public static BudgetDetails Create(YNAB.SDK.Model.BudgetDetailResponse response)
        {
            YNAB.SDK.Model.BudgetDetail modelBudgetDetail = response.Data.Budget;

            Dictionary<Guid, CategoryGroup> categoryGroups = new Dictionary<Guid, CategoryGroup>();
            foreach (YNAB.SDK.Model.CategoryGroup modelCategoryGroup in modelBudgetDetail.CategoryGroups)
            {
                CategoryGroup categoryGroup = new CategoryGroup(modelCategoryGroup.Name, modelCategoryGroup.Id);
                categoryGroups.Add(categoryGroup.Id, categoryGroup);
            }

            foreach (YNAB.SDK.Model.Category modelCategory in modelBudgetDetail.Categories)
            {
                Category category = new Category(modelCategory.Name, modelCategory.Id, modelCategory.CategoryGroupId, modelCategory.Budgeted, modelCategory.Activity, modelCategory.Balance);
                if (!categoryGroups.ContainsKey(category.CategoryGroupId))
                    continue;

                categoryGroups[category.CategoryGroupId].AddCategory(category);
            }

            return new BudgetDetails(modelBudgetDetail.Name, categoryGroups.Values);
        }

        /// <summary>
        /// ToString()
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format("Budget: {0} | {1} Category Groups", Name, CategoryGroups.Count));

            foreach (CategoryGroup group in CategoryGroups)
            {
                sb.AppendLine(String.Format("{0} | {1} Categories", group.Name, group.Categories.Count));

                foreach (Category category in group.Categories)
                    sb.AppendLine(String.Format("\t-{0}", category.Name));
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// All information related to a CategoryGroup
    /// </summary>
    public class CategoryGroup
    {
        public string Name { get; private set; }
        public Guid Id { get; private set; }
        public IList<Category> Categories { get; private set; }

        public CategoryGroup(string name, Guid id)
        {
            Name = name;
            Id = id;

            Categories = new List<Category>();
        }

        /// <summary>
        /// Add a single category to this group
        /// </summary>
        public void AddCategory(Category category)
        {
            if (Categories.Contains(category))
                return;
            
            Categories.Add(category);
        }
    }

    /// <summary>
    /// A single category in a YNAB budget. Also can be thought of as an individual "line item"
    /// </summary>
    public class Category
    {
        public string Name { get; private set; }

        public Guid Id { get; private set; }
        public Guid CategoryGroupId { get; private set; }

        public long Budgeted { get; private set; }
        public long Activity { get; private set; }
        public long Remaining { get; private set; }

        public Category(string name, Guid id, Guid categoryGroupId, long budgeted, long activity, long remaining)
        {
            Name = name;
            Id = id;
            CategoryGroupId = categoryGroupId;
            Budgeted = budgeted;
            Activity = activity;
            Remaining = remaining;
        }
    }
}