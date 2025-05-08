namespace AngApp.Server.Models.User;

public record UserProfile
{
    public UserProfile(String name, String? email, List<String> userRoles, String? userName)
    {
        Name = name;
        Email = email;
        UserRoles = userRoles;
        UserName = userName;
    }


    public String Name { get; set; }
    public String? Email { get; set; }
    public List<String> UserRoles { get; set; }
    public String? UserName { get; set; }
}