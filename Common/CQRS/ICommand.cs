namespace How.Common.CQRS;

using MediatR;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
    
}