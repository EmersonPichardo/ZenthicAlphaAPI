using Application.Users.Add;
using Application.Users.ChangePassword;
using Application.Users.Get;
using Application.Users.GetAll;
using Application.Users.GetPaginated;
using Application.Users.Login;
using Application.Users.Logout;
using Application.Users.ResetPassword;
using MediatR;
using Presentation._Common.Endpoints;

namespace Presentation.Endpoints;

public class UsersEndpoints : BaseEndpointCollection
{
    public UsersEndpoints() : base("users")
    {
        //Queries
        DefineGetPaginatedEndpoint<GetUsersPaginatedQuery, GetUsersPaginatedQueryResponse>();
        DefineGetAllEndpoint<GetAllUsersQuery, GetAllUsersQueryResponse>();
        DefineGetEndpoint<GetUserQuery, GetUserQueryResponse>();

        //Commands
        DefineInsertEndpoint<AddUserCommand>();

        DefineEndpoint(HttpVerbose.Post, "/login",
            Login, 200, typeof(LoginUserCommandResponse));

        DefineEndpoint(HttpVerbose.Post, "/logout",
            Logout, 200);

        DefineEndpoint(HttpVerbose.Patch, "/resetPassword",
            ResetPassword, 200);

        DefineEndpoint(HttpVerbose.Patch, "/changePassword",
            ChangePassword, 200);
    }

    private async Task<IResult> Login(ISender mediator, LoginUserCommand command)
    {
        var response = await mediator.Send(command);
        return Results.Ok(response);
    }
    private async Task<IResult> Logout(ISender mediator)
    {
        var command = new LogoutCurrentUserCommand();
        await mediator.Send(command);

        return Results.Ok();
    }
    private async Task<IResult> ResetPassword(ISender mediator, ResetUserPasswordCommand command)
    {
        await mediator.Send(command);
        return Results.Ok();
    }
    private async Task<IResult> ChangePassword(ISender mediator, ChangeUserPasswordCommand command)
    {
        await mediator.Send(command);
        return Results.Ok();
    }
}
