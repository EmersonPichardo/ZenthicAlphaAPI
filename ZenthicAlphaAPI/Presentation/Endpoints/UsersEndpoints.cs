﻿using Application.Users.Add;
using Application.Users.ChangePassword;
using Application.Users.Get;
using Application.Users.GetAll;
using Application.Users.GetPaginated;
using Application.Users.Login;
using Application.Users.Logout;
using Application.Users.RefreshToken;
using Application.Users.ResetPassword;
using MediatR;
using Presentation._Common.Endpoints;
using Presentation._Common.ExceptionHandler;

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

        DefineEndpoint(HttpVerbose.Post, "/refresh-token",
            RefreshToken, 200, typeof(RefreshUserTokenCommandResponse));

        DefineEndpoint(HttpVerbose.Post, "/logout",
            Logout, 200);

        DefineEndpoint(HttpVerbose.Patch, "/reset-password",
            ResetPassword, 200);

        DefineEndpoint(HttpVerbose.Patch, "/change-password",
            ChangePassword, 200);
    }

    private async Task<IResult> Login(ISender mediator, LoginUserCommand command)
    {
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    }
    private async Task<IResult> RefreshToken(ISender mediator)
    {
        var command = new RefreshUserTokenCommand();
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    }
    private async Task<IResult> Logout(ISender mediator)
    {
        var command = new LogoutCurrentUserCommand();
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    }
    private async Task<IResult> ResetPassword(ISender mediator, ResetUserPasswordCommand command)
    {
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    }
    private async Task<IResult> ChangePassword(ISender mediator, ChangeUserPasswordCommand command)
    {
        var result = await mediator.Send(command);

        return result.Match(
            ResultFactory.Ok,
            ResultFactory.ProblemDetails
        );
    }
}
