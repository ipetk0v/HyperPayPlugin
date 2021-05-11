using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.HyperPayCopyAndPay.Models;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.HyperPayCopyAndPay.Components
{
    [ViewComponent(Name = "HyperPay")]
    public class HyperPayViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.HyperPayCopyAndPay/Views/PaymentInfo.cshtml", new PaymentInfoModel());
        }
    }
}
