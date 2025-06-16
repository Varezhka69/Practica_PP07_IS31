using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int UserID { get; set; }[Required]
public string Name { get; set; }

[Required]
public string Password { get; set; }

public string Role { get; set; }
}
