namespace How.Core.Services.GrpcCommunication;

using Common;
using CQRS.Commands.Internal.UserDelete;
using CQRS.Commands.Internal.UserRegister;
using CQRS.Commands.Internal.UserUpdateSuspend;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

public class UserAccountGrpcService : UserAccount.UserAccountBase
{
    private readonly ILogger<UserAccountGrpcService> _logger;
    private readonly ISender _sender;

    public UserAccountGrpcService(ILogger<UserAccountGrpcService> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }
    
    public override async Task<Reply> UserRegister(RegisterUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Content Received User ID: {UserId}", request.UserId);

        if (request.UserId == 0)
        {
            _logger.LogError("Received User Register Message without User ID");
        }

        var result = await _sender.Send(new InternalUserRegisterCommand
        {
            UserId = request.UserId,
        });

        var reply = new Reply();

        if (result.Succeeded)
        {
            if (result.Data == 0)
            {
                _logger.LogInformation($"User not registered. User ID: {request.UserId}");
            }

            reply.Success = true;
        }
        else
        {
            _logger.LogError(result.GetErrorMessages());
            reply.Success = false;
        }

        return reply;
    }

    public override async Task<Reply> UserDelete(DeleteUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Content Received User ID: {UserId}", request.UserId);
        
        if (request.UserId == 0)
        {
            _logger.LogError("Received User Delete Message without User ID");
        }
        
        var salt = "Deleted_" + Guid.NewGuid();

        var result = await _sender.Send(new InternalUserDeleteCommand
        {
            UserId = request.UserId,
            Salt = salt
        });
        
        var reply = new Reply();
        
        if (result.Succeeded)
        {
            if (result.Data == 0)
            {
                _logger.LogInformation($"User not deleted. User ID: {request.UserId}");
            }

            reply.Success = true;
        }
        else
        {
            _logger.LogError(result.GetErrorMessages());
            reply.Success = false;
        }

        return reply;
    }

    public override async Task<Reply> UserSuspend(SuspendUser request, ServerCallContext context)
    {
        _logger.LogInformation("Content Received User ID: {UserId}", request.UserId);
        
        if (request.UserId == 0)
        {
            _logger.LogError("Received User update Suspend state Message without User ID");
        }
        
        var result = await _sender.Send(new InternalUserUpdateSuspendCommand
        {
            UserId = request.UserId,
            State = request.State
        });
        
        var reply = new Reply();
        
        if (result.Succeeded)
        {
            if (result.Data == 0)
            {
                _logger.LogInformation($"User suspend state not updated. User ID: {request.UserId}");
            }

            reply.Success = true;
        }
        else
        {
            _logger.LogError(result.GetErrorMessages());
            reply.Success = false;
        }

        return reply;
    }
}