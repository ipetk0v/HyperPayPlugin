using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.HyperPayCopyAndPay.Models;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.HyperPayCopyAndPay.Services
{
    public interface IHyperPayServices
    {
        Dictionary<string, dynamic> RequestForm(Order order);

        bool RequestPaymentStatus(string id,int orderId, out ValidatePayment responseData);

        string ReturnHyperPaySettingsId(Order order);
    }
}
