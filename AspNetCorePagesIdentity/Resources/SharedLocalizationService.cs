using Microsoft.Extensions.Localization;
using System.Reflection;

namespace AspNetCorePagesIdentity.Resources
{
    public class SharedLocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public SharedLocalizationService(IStringLocalizerFactory factory)
        {
            var type = typeof(IdentityResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResource", assemblyName.Name);
        }

        public LocalizedString GetLocalizedHtmlString(string key)
        {
            return _localizer[key];
        }
    }
}