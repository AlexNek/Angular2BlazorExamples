using Netlify.SharedResources;

namespace Netlify;

public static class AlertIdExtensions
{
    private static readonly Dictionary<AlertId, string> _alertIdToString = new Dictionary<AlertId, string>
                                                                               {
                                                                                   { AlertId.GenericError, "GENERIC_ERROR" },
                                                                                   { AlertId.NetworkError, "NETWORK_ERROR" },
                                                                                   { AlertId.BadCredentials, "BAD_CREDENTIALS" },
                                                                                   { AlertId.UserDuplicated, "USER_DUPLICATED" },
                                                                                   { AlertId.PasswordChanged, "PASSWORD_CHANGED" },
                                                                                   { AlertId.UserSaved, "USER_SAVED" },
                                                                                   { AlertId.UpdateUserError, "UPDATE_USER_ERROR" },
                                                                                   { AlertId.CurrentPasswordError, "CURRENT_PASSWORD_ERROR" },
                                                                                   { AlertId.SessionExpired, "SESSION_EXPIRED" },
                                                                                   { AlertId.AccountDeleted, "ACCOUNT_DELETED" },
                                                                                   { AlertId.DoubleVoted, "DOUBLE_VOTED" },
                                                                                   { AlertId.HeroDeleted, "HERO_DELETED" }
                                                                               };

    public static string ToStringLocalized(this AlertId alertId, SharedLocalizer localizer)
    {
        return localizer[_alertIdToString[alertId]];
    }
}