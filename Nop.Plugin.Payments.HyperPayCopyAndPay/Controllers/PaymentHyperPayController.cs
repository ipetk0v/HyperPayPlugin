using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.HyperPayCopyAndPay.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.HyperPayCopyAndPay.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class PaymentHyperPayController : BasePaymentController
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public PaymentHyperPayController(ILanguageService languageService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _languageService = languageService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var hyperPayPaymentSettings = _settingService.LoadSetting<HyperPayPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                DescriptionText = hyperPayPaymentSettings.DescriptionText
            };

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.DescriptionText = _localizationService
                    .GetLocalizedSetting(hyperPayPaymentSettings, x => x.DescriptionText, languageId, 0, false, false);
            });
            model.AdditionalFee = hyperPayPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = hyperPayPaymentSettings.AdditionalFeePercentage;

            model.Id = hyperPayPaymentSettings.Id;
            model.MadaId = hyperPayPaymentSettings.MadaId;
            model.Authorization = hyperPayPaymentSettings.Authorization;
            model.Url = hyperPayPaymentSettings.Url;
            model.IsTestMode = hyperPayPaymentSettings.IsTestMode;
            model.DescriptionText = hyperPayPaymentSettings.DescriptionText;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.DescriptionText_OverrideForStore = _settingService.SettingExists(hyperPayPaymentSettings, x => x.DescriptionText, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(hyperPayPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(hyperPayPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.IsTestMode_OverrideForStore = _settingService.SettingExists(hyperPayPaymentSettings, x => x.IsTestMode, storeScope);
                model.Id_OverrideForStore = _settingService.SettingExists(hyperPayPaymentSettings, x => x.Id, storeScope);
                model.MadaId_OverrideForStore = _settingService.SettingExists(hyperPayPaymentSettings, x => x.MadaId, storeScope);
                model.Authorization_OverrideForStore = _settingService.SettingExists(hyperPayPaymentSettings, x => x.Authorization, storeScope);
                model.Url_OverrideForStore = _settingService.SettingExists(hyperPayPaymentSettings, x => x.Url, storeScope);
            }

            return View("~/Plugins/Payments.HyperPayCopyAndPay/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return RedirectToAction("Configure");

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var hyperPayPaymentSettings = _settingService.LoadSetting<HyperPayPaymentSettings>(storeScope);

            //save settings
            hyperPayPaymentSettings.DescriptionText = model.DescriptionText;
            hyperPayPaymentSettings.AdditionalFee = model.AdditionalFee;
            hyperPayPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            hyperPayPaymentSettings.Authorization = model.Authorization;
            hyperPayPaymentSettings.Id = model.Id;
            hyperPayPaymentSettings.IsTestMode = model.IsTestMode;
            hyperPayPaymentSettings.Url = model.Url;
            hyperPayPaymentSettings.MadaId = model.MadaId;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(hyperPayPaymentSettings, x => x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(hyperPayPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(hyperPayPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(hyperPayPaymentSettings, x => x.Authorization, model.Authorization_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(hyperPayPaymentSettings, x => x.Id, model.Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(hyperPayPaymentSettings, x => x.Url, model.Url_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(hyperPayPaymentSettings, x => x.MadaId, model.MadaId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(hyperPayPaymentSettings, x => x.IsTestMode, model.IsTestMode_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //localization. no multi-store support for localization yet.
            foreach (var localized in model.Locales)
            {
                _localizationService.SaveLocalizedSetting(hyperPayPaymentSettings,
                    x => x.DescriptionText, localized.LanguageId, localized.DescriptionText);
            }

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return RedirectToAction("Configure");
        }
        #endregion
    }
}