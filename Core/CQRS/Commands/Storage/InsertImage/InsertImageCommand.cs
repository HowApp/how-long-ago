namespace How.Core.CQRS.Commands.Storage.InsertImage;

using Common.CQRS;
using Common.ResultType;
using Models.ServicesModel;

public sealed class InsertImageCommand : ICommand<Result<int>>
{
    public int CurrentUserId { get; set; }
    public ImageInternalModel Image { get; set; }
}