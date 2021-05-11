using Newtonsoft.Json;

namespace Nop.Plugin.Payments.HyperPayCopyAndPay.Models
{
    public class ValidatePayment
    {
        public ValidatePayment()
        {
            Result = new ResultModel();
            ResultDetails = new ResultDetailsModel();
            Card = new CardModel();
            Customer = new CustomerModel();
            CustomParameters = new CustomParametersModel();
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("paymentType")]
        public string PaymentType { get; set; }

        [JsonProperty("paymentBrand")]
        public string PaymentBrand { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("descriptor")]
        public string Descriptor { get; set; }

        [JsonProperty("result")]
        public ResultModel Result { get; set; }

        [JsonProperty("resultDetails")]
        public ResultDetailsModel ResultDetails { get; set; }

        [JsonProperty("card")]
        public CardModel Card { get; set; }

        [JsonProperty("customer")]
        public CustomerModel Customer { get; set; }

        [JsonProperty("customParameters")]
        public CustomParametersModel CustomParameters { get; set; }

        [JsonProperty("risk")]
        public RiskModel Risk { get; set; }

        [JsonProperty("buildNumber")]
        public string BuildNumber { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("ndc")]
        public string Ndc { get; set; }

        public class ResultModel
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }
        }

        public class ResultDetailsModel
        {
            [JsonProperty("ConnectorTxID1")]
            public string ConnectorTxID1 { get; set; }
        }

        public class CardModel
        {
            [JsonProperty("bin")]
            public string Bin { get; set; }

            [JsonProperty("binCountry")]
            public string BinCountry { get; set; }

            [JsonProperty("last4Digits")]
            public string Last4Digits { get; set; }

            [JsonProperty("holder")]
            public string Holder { get; set; }

            [JsonProperty("expiryMonth")]
            public string ExpiryMonth { get; set; }

            [JsonProperty("expiryYear")]
            public string ExpiryYear { get; set; }
        }

        public class CustomerModel
        {
            [JsonProperty("email")]
            public string Email { get; set; }
        }

        public class CustomParametersModel
        {
            [JsonProperty("CTPE_DESCRIPTOR_TEMPLATE")]
            public string CTPEDESCRIPTORTEMPLATE { get; set; }
        }

        public class RiskModel
        {
            [JsonProperty("score")]
            public string Score { get; set; }
        }
    }
}
