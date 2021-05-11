using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.HyperPayCopyAndPay.Models
{
    public class ConfigurationModel : BaseNopModel, ILocalizedModel<ConfigurationModel.ConfigurationLocalizedModel>
    {
        public ConfigurationModel()
        {
            Locales = new List<ConfigurationLocalizedModel>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }
        
        [NopResourceDisplayName("Plugins.Payment.HyperPay.DescriptionText")]
        public string DescriptionText { get; set; }
        public bool DescriptionText_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.HyperPay.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.HyperPay.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        public IList<ConfigurationLocalizedModel> Locales { get; set; }

        public string Id { get; set; }
        public bool Id_OverrideForStore { get; set; }

        public bool IsTestMode { get; set; }
        public bool IsTestMode_OverrideForStore { get; set; }

        public string MadaId { get; set; }
        public bool MadaId_OverrideForStore { get; set; }

        public string Authorization { get; set; }
        public bool Authorization_OverrideForStore { get; set; }

        public string Url { get; set; }
        public bool Url_OverrideForStore { get; set; }

        #region Nested class

        public partial class ConfigurationLocalizedModel : ILocalizedLocaleModel
        {
            public int LanguageId { get; set; }
            
            [NopResourceDisplayName("Plugins.Payment.HyperPay.DescriptionText")]
            public string DescriptionText { get; set; }
        }

        #endregion

    }
}