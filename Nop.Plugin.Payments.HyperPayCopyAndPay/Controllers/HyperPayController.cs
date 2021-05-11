using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.HyperPayCopyAndPay.Models;
using Nop.Plugin.Payments.HyperPayCopyAndPay.Services;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System.Text.RegularExpressions;

namespace Nop.Plugin.Payments.HyperPayCopyAndPay.Controllers
{
    public class HyperPayController : BasePluginController
    {
        private readonly ILogger _logger;
        private readonly IHyperPayServices _hyperPayServices;
        private readonly IWebHelper _webHelper;
        private readonly HyperPayPaymentSettings _hyperPayPaymentSettings;
        private readonly IOrderService _orderService;

        public HyperPayController(ILogger logger,
            IHyperPayServices hyperPayServices,
            IWebHelper webHelper,
            HyperPayPaymentSettings hyperPayPaymentSettings,
            IOrderService orderService)
        {
            _logger = logger;
            _hyperPayServices = hyperPayServices;
            _webHelper = webHelper;
            _hyperPayPaymentSettings = hyperPayPaymentSettings;
            _orderService = orderService;
        }

        public IActionResult Payment(string orderId)
        {
            var result = int.TryParse(orderId, out int idOrder);

            if (result == false)
                return RedirectToRoute("HyperPayPayment", new { orderId });

            var order = _orderService.GetOrderById(idOrder);

            if (order.PaymentStatus == PaymentStatus.Paid)
                return RedirectToRoute("HomePage");

            try
            {
                var requestForm = _hyperPayServices.RequestForm(order);

                if (requestForm.Count == 0)
                    throw new ArgumentException("requestForm count === 0");

                var id = requestForm["id"];
                var validateUrl = $"{_webHelper.GetStoreLocation()}hyperpay/validatepayment?orderId={orderId}";

                var settingsId = _hyperPayServices.ReturnHyperPaySettingsId(order);

                var model = new HyperPayModel
                {
                    FormId = id,
                    ValidateUrl = validateUrl,
                    RequestUrl = _hyperPayPaymentSettings.Url,
                    PaymentType = settingsId == _hyperPayPaymentSettings.Id ? "VISA MASTER" : "MADA"
                };

                return View("~/Plugins/Payments.HyperPayCopyAndPay/Views/Payment.cshtml", model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return RedirectToRoute("HomePage");
            }
        }

        public IActionResult ValidatePayment(string id, string resourcePath, string orderId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(resourcePath))
            {
                _logger.Error("HyperPayController => ValidatePament => id or resourcePath is null");
                return RedirectToRoute("HyperPayPayment", new { orderId });
            }

            var resultOrder = int.TryParse(orderId, out int idOrder);
            if(resultOrder == false)
                return RedirectToRoute("HyperPayPayment", new { orderId });

            if (!_hyperPayServices.RequestPaymentStatus(id, idOrder, out var responseData))
                return RedirectToRoute("HyperPayPayment", new { orderId });

            var result = responseData.Result;
            var paidStatusRegex = new Regex(@"^(000\.000\.|000\.100\.1|000\.[36])");

            if (paidStatusRegex.Match(result.Code).Success)
            {

                var order = _orderService.GetOrderById(idOrder);
                order.PaymentStatus = PaymentStatus.Paid;
                order.OrderStatus = OrderStatus.Processing;
                _orderService.UpdateOrder(order);

                return RedirectToRoute("CheckoutCompleted", new { orderId });
            }

            return RedirectToRoute("HyperPayPayment", new { orderId });
        }
    }
}
