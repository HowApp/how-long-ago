namespace How.Core.Models.ServicesModel;

using Common.Models;
using Infrastructure.Enums;

public class GetEventsPaginationModel : PaginationModel
{
    public int CurrentUserId { get; set; }
    public string Search { get; set; }
    public EventStatus Status { get; set; }
    public EventAccessType Access { get; set; }
    public FilterType FilterType { get; set; }
}