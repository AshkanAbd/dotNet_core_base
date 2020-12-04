namespace dotNet_base.Components.Configs
{
    public class PaymentConfig
    {
        public ZarinPal ZarinPal { get; set; }
    }

    public class ZarinPal
    {
        public string MerchantId { get; set; }
        public bool Sandbox { get; set; }
        public string CallbackUrl { get; set; }
    }
}