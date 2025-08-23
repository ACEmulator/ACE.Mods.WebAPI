namespace ACE.Mods.WebAPI.Authentication;

/// <summary>
/// When annotated on a service method, requests will only be allowed
/// if the authenticated user has the specified roles.
/// </summary>
/// <param name="grants">The roles which need to be present in order to let the request pass</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RequireGrantAttribute(params string[] grants) : InterceptWithAttribute<GrantInterceptor>
{

    /// <summary>
    /// The roles which need to be present in order to let the request pass.
    /// </summary>
    public string[] Grants => grants;

}
