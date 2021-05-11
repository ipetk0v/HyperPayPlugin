using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.HyperPayCopyAndPay.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        public string DescriptionText { get; set; }

        public string Error { get; set; }

        public PaymentType PaymentType { get; set; }

        public int PaymentTypeId { get; set; }
    }
}