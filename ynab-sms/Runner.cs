using System;
using System.Collections.Generic;

namespace Ynab_Sms
{
    public static class Runner
    {
        public static void Run(CommandLineOptions options)
        {
            if (options.Verbose)
                Logging.Logger.Level = Logging.LoggingLevel.Verbose;

            AppConfig appConfig = AppConfig.CreateFromJsonFile(options.ConfigFilePath);
            if (appConfig == null)
                return;

            BudgetItemsConfig budgetItemsConfig = BudgetItemsConfig.CreateFromJson(appConfig.BudgetItemsJsonFile);
            if (budgetItemsConfig == null)
                return;

            IEnumerable<BudgetDetails> budgetDetails = YnabApi.GetBudgets(appConfig.AccessToken, budgetItemsConfig.GetBudgetIds());
            MessageContentManager messageContentManager = MessageContentManager.Create(budgetItemsConfig, budgetDetails);

            CommandLineMessegeSender commandLineMessageSender = new CommandLineMessegeSender();
            SmsMessageSender smsMessageSender = new SmsMessageSender(appConfig.TwilioSid, appConfig.TwilioAuthToken, appConfig.TwilioPhoneNumber);
            if (!smsMessageSender.Init())
                return;

            foreach (string phoneNumber in messageContentManager.GetPhoneNumbers())
            {
                string message = messageContentManager.GetMessageForPhoneNumber(phoneNumber);
                commandLineMessageSender.Send(phoneNumber, message);

                if (options.SendSms)
                    smsMessageSender.Send(phoneNumber, message);
            }
        }
    }
}