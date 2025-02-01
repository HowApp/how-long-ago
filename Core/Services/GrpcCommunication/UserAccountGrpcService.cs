namespace How.Core.Services.GrpcCommunication;

using Common;
using CQRS.Commands.Internal.UserRegister;
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
    
    public override async Task<RegisterUserReply> UserRegister(RegisterUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Content Received User ID: {UserId}", request.UserId);

        if (request.UserId == 0)
        {
            _logger.LogError("Received User Register Message without User ID");
        }

        var result = await _sender.Send(new UserRegisterCommand
        {
            UserId = request.UserId,
        });

        var reply = new RegisterUserReply();

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
}