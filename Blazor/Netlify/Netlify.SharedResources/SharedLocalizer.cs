using Microsoft.Extensions.Localization;

namespace Netlify.SharedResources
{
    public class SharedLocalizer
    {
        private readonly IStringLocalizer<SharedLocalizer> _localizer;

        public SharedLocalizer(IStringLocalizer<SharedLocalizer> localizer)
        {
            _localizer = localizer;
        }

        public string this[string key] => _localizer[key];
    }
}
