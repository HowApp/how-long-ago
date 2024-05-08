namespace How.Core.Database.Entities.Identity;

using Microsoft.AspNetCore.Identity;
using Storage;

public class HowUser : IdentityUser<int>
{
    public int? ImageId { get; set; }
    public Image Image { get; set; }
    public virtual ICollection<HowUserRole> UserRoles {get; set;}
}