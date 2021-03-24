using System;
using System.Collections.Generic;
using System.Text;

namespace Ynab_Sms
{
    public class MessageContentManager
    {
        // Maps phone numbers to a collection of strings to send to said phone number
        private Dictionary<string, ICollection<string>> m_dict;

        /// <summary>
        /// Contains the raw form of the content that will be stiched togehter into a message and sent out
        /// </summary>
        /// <param name="dict"></param>
        public MessageContentManager(Dictionary<string, ICollection<string>> dict)
        {
            m_dict = dict;
        }

        /// <summary>
        /// Compare the budget config file and the budet details from the YNAB servers to get the necessary information to send out
        /// </summary>
        public static MessageContentManager Create(BudgetConfigFile budgetConfigFile, IEnumerable<BudgetDetails> budgetDetails)
        {
            Dictionary<string, ICollection<string>> dict = new Dictionary<string, ICollection<string>>();

            foreach (BudgetDetails budgetDetail in budgetDetails)
            {
                foreach (CategoryGroup categoryGroup in budgetDetail.CategoryGroups)
                {
                    foreach (Category category in categoryGroup.Categories)
                    {
                        ICollection<string> phoneNumbersThatCare = budgetConfigFile.GetPhoneNumbersThatRegisteredForBudgetItem(budgetDetail.Id, categoryGroup.Name, category.Name);
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

            return new MessageContentManager(dict);
        }

        /// <summary>
        /// Get the list of phone numbers
        /// </summary>
        public ICollection<string> GetPhoneNumbers()
        {
            return m_dict.Keys;
        }

        /// <summary>
        /// Get the message that we will send to the phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
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

        /// <summary>
        /// ToString
        /// </summary>
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