using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.HyperPayCopyAndPay.Models;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.HyperPayCopyAndPay.Services
{
    public class HyperPayServices : IHyperPayServices
    {
        private readonly HyperPayPaymentSettings _hyperPayPaymentSettings;
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IOrderService _orderService;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger _logger;
        private readonly IStateProvinceService _stateProvinceService;

        public HyperPayServices(HyperPayPaymentSettings hyperPayPaymentSettings,
            IWorkContext workContext,
            ICurrencyService currencyService,
            IAddressService addressService,
            ICountryService countryService,
            IOrderService orderService,
            IEncryptionService encryptionService,
            ILogger logger,
            IStateProvinceService stateProvinceService)
        {
            _hyperPayPaymentSettings = hyperPayPaymentSettings;
            _workContext = workContext;
            _currencyService = currencyService;
            _addressService = addressService;
            _countryService = countryService;
            _orderService = orderService;
            _encryptionService = encryptionService;
            _logger = logger;
            _stateProvinceService = stateProvinceService;
        }

        public string ReturnHyperPaySettingsId(Order order)
        {
            var cardType = _encryptionService.DecryptText(order.CardType);
            return cardType.ToLower().Equals("mada") ? _hyperPayPaymentSettings.MadaId : _hyperPayPaymentSettings.Id;
        }

        public Dictionary<string, dynamic> RequestForm(Order order)
        {
            var currency = _workContext.WorkingCurrency.CurrencyCode;
            var address = _addressService.GetAddressById(order.BillingAddressId);

            var countryCode = address.CountryId.HasValue
                ? _countryService.GetCountryById(address.CountryId.Value).TwoLetterIsoCode
                : "US";

            var county = address.StateProvinceId.HasValue
                ? _stateProvinceService.GetStateProvinceById(address.StateProvinceId.Value).Name
                : "NULL";

            var priceValue = _currencyService
                .ConvertFromPrimaryStoreCurrency(order.OrderTotal, _workContext.WorkingCurrency);

            if (_hyperPayPaymentSettings.IsTestMode)
                priceValue = Math.Round(priceValue, 0, MidpointRounding.AwayFromZero);

            var amount = priceValue.ToString("0.00").Replace("٫", ".");
            var entityId = ReturnHyperPaySettingsId(order);
            
            Dictionary<string, dynamic> responseData;
            string data = $"entityId={entityId}" +
                          $"&amount={amount}" +
                          $"&currency={currency}" +
                          $"&customer.email={address.Email}" +
                          $"&billing.street1={address.Address1}" +
                          $"&billing.city={address.City}" +
                          $"&billing.state={county}" +
                          $"&billing.country={countryCode}" +
                          $"&billing.postcode={address.ZipPostalCode}" +
                          $"&merchantTransactionId={order.Id}" +
                          $"&customer.givenName={address.FirstName}" +
                          $"&customer.surname={address.LastName}" +
                          "&paymentType=DB";

            try
            {
                var urlIsValid = _hyperPayPaymentSettings.Url.Substring(_hyperPayPaymentSettings.Url.Length - 1) == "/";
                string url = urlIsValid ? _hyperPayPaymentSettings.Url + "v1/checkouts" : _hyperPayPaymentSettings.Url + "/v1/checkouts";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.Headers["Authorization"] = _hyperPayPaymentSettings.Authorization;
                request.ContentType = "application/x-www-form-urlencoded";
                Stream PostData = request.GetRequestStream();
                PostData.Write(buffer, 0, buffer.Length);
                PostData.Close();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
                    reader.Close();
                    dataStream.Close();
                }
                return responseData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new Dictionary<string, dynamic>();
            }
        }

        public bool RequestPaymentStatus(string id, int orderId, out ValidatePayment responseData)
        {
            responseData = null;
            try
            {
                var order = _orderService.GetOrderById(orderId);
                var entityId = ReturnHyperPaySettingsId(order);
                string data = $"entityId={entityId}";

                var urlIsValid = _hyperPayPaymentSettings.Url.Substring(_hyperPayPaymentSettings.Url.Length - 1) == "/";
                string url = urlIsValid
                    ? _hyperPayPaymentSettings.Url + $"v1/checkouts/{id}/payment?" + data
                    : _hyperPayPaymentSettings.Url + $"/v1/checkouts/{id}/payment?" + data;

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                request.Headers["Authorization"] = _hyperPayPaymentSettings.Authorization;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    var dataStream = response.GetResponseStream();
                    var reader = new StreamReader(dataStream);
                    var respJson = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                    responseData = JsonConvert.DeserializeObject<ValidatePayment>(respJson);
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
