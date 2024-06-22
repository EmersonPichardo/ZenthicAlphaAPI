using Application.Roles.Add;
using Application.Roles.Delete;
using Application.Roles.Get;
using Application.Roles.GetAll;
using Application.Roles.GetPaginated;
using Application.Roles.Update;
using Presentation._Common.Endpoints;

namespace Presentation.Endpoints;

public class RolesEndpoints : BaseEndpointCollection
{
    public RolesEndpoints() : base("roles")
    {
        //Queries
        DefineGetPaginatedEndpoint<GetRolesPaginatedQuery, GetRolesPaginatedQueryResponse>();
        DefineGetAllEndpoint<GetAllRolesQuery, GetAllRolesQueryResponse>();
        DefineGetEndpoint<GetRoleQuery, GetRoleQueryResponse>();

        //Commands
        DefineInsertEndpoint<AddRoleCommand>();
        DefineUpdateEndpoint<UpdateRoleCommand>();
        DefineDeleteEndpoint<DeleteRoleCommand>();
    }
}
