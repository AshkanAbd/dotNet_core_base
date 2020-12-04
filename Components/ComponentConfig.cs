namespace dotNet_base.Components
{
    public class ComponentConfig
    {
        public Configs.SmsConfig Sms { get; set; }
        public Configs.PaymentConfig Payment { get; set; }
        public Configs.JwtConfig Jwt { get; set; }
        public string Environment { get; set; }
        public string OrderTimeout { get; set; }
    }
}