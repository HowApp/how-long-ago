namespace How.Core.CQRS.Commands.Storage.CreateImage;

using Common.CQRS;
using Common.ResultType;
using Models.ServicesModel;

public sealed class CreateImageCommand : ICommand<Result<int>>
{
    public ImageInternalModel Image { get; set; }
}