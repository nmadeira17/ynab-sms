using System;

using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Ynab_Sms.Logging;

namespace Ynab_Sms
{
    /// <summary>
    /// Sends a message to a phone number
    /// </summary>
    public interface IMessageSender
    {
        void Send(string toPhoneNumber, string message);
    }

    /// <summary>
    /// Will send an SMS message
    /// </summary>
    public class SmsMessageSender : IMessageSender
    {
        private string m_accountSid;
        private string m_authToken;
        private string m_fromPhoneNumber;

        public SmsMessageSender(string accountSid, string authToken, string fromPhoneNumber)
        {
            m_accountSid = accountSid;
            m_authToken = authToken;
            m_fromPhoneNumber = fromPhoneNumber;
        }

        /// <summary>
        /// Initialize the Twilio Client
        /// </summary>
        public bool Init()
        {
            try
            {
                TwilioClient.Init(m_accountSid, m_authToken);
                return true;
            }
            catch(Exception e)
            {
                Logger.Log(String.Format("Failed to initialize SMS client. Error:\n{0}", e.ToString()));
                return false;
            }
        }

        /// <summary>
        /// Send SMS messages
        /// </summary>
        public void Send(string toPhoneNumber, string message)
        {
            if (!toPhoneNumber.StartsWith("+1"))
                toPhoneNumber = toPhoneNumber.Insert(0, "+1");

            MessageResource messageResource = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(m_fromPhoneNumber),
                to: new Twilio.Types.PhoneNumber(toPhoneNumber)
            );

            Logger.Log(String.Format("Message sent to: {0}", toPhoneNumber));
        }
    }

    /// <summary>
    /// Thin wrapper around logging to the command line
    /// </summary>
    public class CommandLineMessegeSender : IMessageSender
    {
        public void Send(string toPhoneNumber, string message)
        {
            Console.WriteLine(String.Format("{0}\n{1}", toPhoneNumber, message));
        }
    }
}