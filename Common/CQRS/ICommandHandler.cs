namespace How.Common.CQRS;

using MediatR;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse> 
    where TCommand : ICommand<TResponse>
{
    
}