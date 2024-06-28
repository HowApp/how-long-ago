namespace How.Core.Database.Entities.SharedUser;

using Base;
using Identity;

public class SharedUser : IdentityKey
{
    public int UserOwnerId { get; set; }
    public HowUser UserOwner { get; set; }
    
    public int UserSharedId { get; set; }
    public HowUser UserShared { get; set; }
}