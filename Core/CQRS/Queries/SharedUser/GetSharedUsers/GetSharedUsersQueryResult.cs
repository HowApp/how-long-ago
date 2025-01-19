namespace How.Core.CQRS.Queries.SharedUser.GetSharedUsers;

public sealed class GetSharedUsersQueryResult
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MainHash { get; set; }
    public string ThumbnailHash { get; set; }
}