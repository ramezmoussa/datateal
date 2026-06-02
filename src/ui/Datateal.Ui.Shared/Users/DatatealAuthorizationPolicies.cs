using Datateal.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Datateal.Ui.Shared.Users;

/// <summary>
/// Shared authorization policy configuration used by both the server and WASM client.
/// </summary>
public static class DatatealAuthorizationPolicies
{
    public static void Configure(AuthorizationOptions options)
    {
        options.AddPolicy(AuthPolicy.Admin, p =>
            p.RequireRole(DatatealRole.Admin));
        options.AddPolicy(AuthPolicy.NodePoolManage, p =>
            p.RequireRole(DatatealRole.Admin, DatatealRole.NodePoolContributor));
        options.AddPolicy(AuthPolicy.NodePoolOperate, p =>
            p.RequireRole(DatatealRole.Admin, DatatealRole.NodePoolContributor, DatatealRole.NodePoolOperator));
        options.AddPolicy(AuthPolicy.JobManage, p =>
            p.RequireRole(DatatealRole.Admin, DatatealRole.JobContributor));
        options.AddPolicy(AuthPolicy.JobOperate, p =>
            p.RequireRole(DatatealRole.Admin, DatatealRole.JobContributor, DatatealRole.JobOperator));
        options.AddPolicy(AuthPolicy.JobRead, p =>
            p.RequireRole(DatatealRole.Admin, DatatealRole.JobContributor, DatatealRole.JobOperator, DatatealRole.JobReader));
        options.AddPolicy(AuthPolicy.WorkspaceManage, p =>
            p.RequireRole(DatatealRole.Admin, DatatealRole.WorkspaceContributor));
        options.AddPolicy(AuthPolicy.WorkspaceRead, p =>
            p.RequireRole(DatatealRole.Admin, DatatealRole.WorkspaceContributor, DatatealRole.WorkspaceReader));
        options.AddPolicy(AuthPolicy.CatalogManage, p =>
            p.RequireRole(DatatealRole.Admin, DatatealRole.CatalogContributor));
        options.AddPolicy(AuthPolicy.EnvironmentManage, p =>
            p.RequireRole(DatatealRole.Admin, DatatealRole.EnvironmentManager));
    }
}
