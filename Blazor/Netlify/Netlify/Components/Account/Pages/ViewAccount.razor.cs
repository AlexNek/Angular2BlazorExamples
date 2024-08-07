using Microsoft.AspNetCore.Components;

namespace Netlify.Components.Account.Pages
{
    public partial class ViewAccount : ComponentBase
    {
        [SupplyParameterFromQuery]
        private string? FirstName { get; set; }

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (!string.IsNullOrEmpty(FirstName))
            {
                if (await ClaimsHelper.UpdateUserNameAsync(HttpContext, FirstName))
                {
                    // Remove query parameters from the current URL
                    string currentUri = Navigation.Uri;
                    Uri uri = new Uri(currentUri);
                    string baseUrl = uri.GetLeftPart(UriPartial.Path);
                    // Force a full page reload
                    Navigation.NavigateTo(baseUrl, true);
                }
            }
        }
    }
}
