using System;
using System.Threading.Tasks;
using Kavenegar;

namespace dotNet_base.Components.SMS
{
    public class SMS
    {
        public static async Task<bool> Send(Configs.SmsConfig config, string msg, string receiver)
        {
            if (!config.ShouldSend) {
                return true;
            }

            try {
                var api = new KavenegarApi(config.ApiKey);
                api.Send(config.Sender, receiver, msg);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        public static async Task<bool> Lookup(Configs.SmsConfig config, string template, string receiver, string token)
        {
            if (!config.ShouldSend) {
                return true;
            }

            try {
                var api = new KavenegarApi(config.ApiKey);
                api.VerifyLookup(receiver, token, template);
                return true;
            }
            catch (Exception) {
                return false;
            }
        }
    }
}