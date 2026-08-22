using Microsoft.AspNetCore.Components;

namespace CircleHub.Client.Helpers;

public class ToastNavigationHelper
{
    public static void NavigateWithToast(NavigationManager nav, string url, string message, string messageType)
    {
        Dictionary<string, object?> toastQueryParams = new()
        {
            { "message", message },
            { "messageType", messageType }
        };
        nav.NavigateTo(nav.GetUriWithQueryParameters(url, toastQueryParams));
    }
}