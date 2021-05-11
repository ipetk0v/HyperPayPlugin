using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.HyperPayCopyAndPay
{
    /// <summary>
    /// Represents settings of "Check money order" payment plugin
    /// </summary>
    public class HyperPayPaymentSettings : ISettings
    { 
        public string DescriptionText { get; set; }

        public bool AdditionalFeePercentage { get; set; }

        public decimal AdditionalFee { get; set; }

        public string Url { get; set; }

        public string Id { get; set; }

        public string MadaId { get; set; }

        public bool IsTestMode { get; set; }

        public string Authorization { get; set; }
    }
}
