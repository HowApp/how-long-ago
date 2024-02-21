namespace How.Common.Constants;

public static class AppConstants
{
    public static class Role
    {
         public static readonly (int Id, string Name) User = new (1, "User");
         public static readonly (int Id, string Name) Admin = new (2, "Admin");
    }

}