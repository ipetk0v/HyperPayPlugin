using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.HyperPayCopyAndPay.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("HyperPayForm",
                "hyperpay/validatepayment/{orderGuid}",
                new { controller = "HyperPay", action = "ValidatePayment" });

            endpointRouteBuilder.MapControllerRoute("HyperPayPayment",
                "hyperpay/payment/{orderId}",
                new { controller = "HyperPay", action = "Payment" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 1;
    }
}