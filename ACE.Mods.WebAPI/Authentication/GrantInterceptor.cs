using GenHTTP.Modules.Reflection.Operations;
using static GenHTTP.Modules.ErrorHandling.StructuredErrorMapper;

namespace ACE.Mods.WebAPI.Authentication;

public class GrantInterceptor : IOperationInterceptor
{
    private string[]? _Grants;

    public void Configure(object attribute)
    {
        if (attribute is RequireGrantAttribute grantAttribute)
        {
            _Grants = grantAttribute.Grants;
        }
    }

    public ValueTask<InterceptionResult?> InterceptAsync(IRequest request, Operation operation, IReadOnlyDictionary<string, object?> arguments)
    {
        if (_Grants?.Length > 0)
        {
            var user = request.GetUser<IUser>();

            if (user == null)
            {
                var status = ResponseStatus.Unauthorized;
                var message = "Authorization required to access this endpoint.";

                var errorModel = new ErrorModel(status, message);
                var result = new InterceptionResult(errorModel);
                result.Status(status);

                return new(result);
            }

            var userRoles = user.Roles;

            var missing = new List<string>(_Grants.Length);

            if (userRoles != null)
            {
                foreach (var role in _Grants)
                {
                    if (!userRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
                    {
                        missing.Add(role);
                    }
                }
            }
            else
            {
                missing.AddRange(_Grants);
            }

            if (missing.Count > 0)
            {
                var status = ResponseStatus.Forbidden;
                var message = "User is not authorized to access this endpoint.";

                var errorModel = new ErrorModel(status, message);
                var result = new InterceptionResult(errorModel);
                result.Status(status);

                return new(result);
            }
        }

        return default;
    }
}
