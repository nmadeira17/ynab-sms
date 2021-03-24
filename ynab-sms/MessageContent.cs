using System;
using System.Collections.Generic;
using System.Text;

namespace Ynab_Sms
{
    public class MessageContent
    {
        // Maps phone numbers to a collection of strings to send to said phone number
        private Dictionary<string, ICollection<string>> m_dict;

        public MessageContent(Dictionary<string, ICollection<string>> dict)
        {
            m_dict = dict;
        }

        public static MessageContent Create(BudgetItemsConfig budgetItemsConfig, IEnumerable<BudgetDetails> budgetDetails)
        {
            Dictionary<string, ICollection<string>> dict = new Dictionary<string, ICollection<string>>();

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
                        string balance = Utils.YnabLongToFormattedString(category.Balance);
                        string entry = String.Format("{0}: {1}", catName, balance);

                        foreach (string phoneNumber in phoneNumbersThatCare)
                        {
                            if (!dict.ContainsKey(phoneNumber))
                                dict.Add(phoneNumber, new List<string>());

                            dict[phoneNumber].Add(entry);
                        }
                    }
                }
            }

            return new MessageContent(dict);
        }

        public ICollection<string> GetPhoneNumbers()
        {
            return m_dict.Keys;
        }

        public string GetMessageForPhoneNumber(string phoneNumber)
        {
            if (!m_dict.ContainsKey(phoneNumber))
                return null;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("YNAB-SMS");

            foreach (string s in m_dict[phoneNumber])
                sb.AppendLine(String.Format("- {0}", s));

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string phoneNumber in m_dict.Keys)
            {
                sb.AppendLine(phoneNumber);

                foreach (string msg in m_dict[phoneNumber])
                {
                    sb.AppendLine(String.Format("\t- {0}", msg));
                }
            }

            return sb.ToString();
        }
    }
}