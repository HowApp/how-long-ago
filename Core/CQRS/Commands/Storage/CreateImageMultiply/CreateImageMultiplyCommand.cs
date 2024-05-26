namespace How.Core.CQRS.Commands.Storage.CreateImageMultiply;

using Common.CQRS;
using Common.ResultType;
using Models.ServicesModel;

public sealed class CreateImageMultiplyCommand : ICommand<Result<int[]>>
{
    public List<ImageInternalModel> Images { get; set; }
}