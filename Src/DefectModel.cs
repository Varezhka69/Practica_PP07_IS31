using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

public class Smartphone
{
    [Key]
    public int SmartphoneID { get; set; }
    public string Model { get; set; }
    public string Manufacturer { get; set; }
    public string Number { get; set; }
    public DateTime ProductionDate { get; set; }
    public int PartyID { get; set; }

    [ForeignKey("PartyID")]
    public virtual Party Party { get; set; }
    
    public Smartphone()
    {
        ProductionDate = DateTime.Today;
    }
}

public class Screen
{
    [Key]
    public int ScreenID { get; set; }
    public int SmartphoneID { get; set; }
    public string Material { get; set; }
    public decimal Supplier { get; set; }
    public DateTime InstallationDate { get; set; }

    [ForeignKey("SmartphoneID")]
    public virtual Smartphone Smartphone { get; set; }

    public Screen()
    {
        Material = "OLED";
    }
}

public class Defect
{
    [Key]
    public int DefectID { get; set; }
    public int ScreenID { get; set; }
    public string Type { get; set; }
    public string Size { get; set; }
    public string Coordinates { get; set; }
    public DateTime DiscoveryDate { get; set; }

    [ForeignKey("ScreenID")]
    public virtual Screen Screen { get; set; }

    public Defect()
    {
        DiscoveryDate = DateTime.Now;
        Type = "Царапина";
        Size = "Легкий";
        Coordinates = "Центр";
    }
}

public class Party
{
    [Key]
    public int PartyID { get; set; }
    public string Name { get; set; }
    public DateTime ArrivalDate { get; set; }
    public int Quantity { get; set; }

    public Party()
    {
        ArrivalDate = DateTime.Today;
        Name = "Новая партия";
        Quantity = 1;
    }
}

public class Controller
{
    [Key]
    public int ControllerID { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }

    public Controller()
    {
        Name = "Новый контроллер";
        Surname = "Абвгд";
    }
}

public class Image
{
    [Key]
    public int ImageID { get; set; }
    public int DefectID { get; set; }
    public string FileName { get; set; }

    [ForeignKey("DefectID")]
    public virtual Defect Defect { get; set; }

    public Image()
    {
        FileName = "image.jpg";
    }
}

public class Role
{
    [Key]
    public int RoleID { get; set; }

    [Required]
    [StringLength(50)]
    public string RoleName { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    public virtual ICollection<User> Users { get; set; }

    public Role()
    {
        Users = new HashSet<User>();
    }
}

public class User
{
    [Key]
    public int UserID { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [StringLength(100)]
    public string Password { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; }

    [StringLength(100)]
    public string Email { get; set; }

    public bool IsActive { get; set; }

    public int RoleID { get; set; }

    [ForeignKey("RoleID")]
    public virtual Role Role { get; set; }

    public DateTime CreatedDate { get; set; }

    public User()
    {
        IsActive = true;
        CreatedDate = DateTime.Now;
    }
}

public class DefectContext : DbContext
{
    public DefectContext() : base("name=DefectConnection")
    {
      
        Database.SetInitializer(new MigrateDatabaseToLatestVersion<DefectContext, SmartphoneDefectsDatabase1.Migrations.Configuration>());
    }

    public DbSet<Smartphone> Smartphones { get; set; }
    public DbSet<Screen> Screens { get; set; }
    public DbSet<Defect> Defects { get; set; }
    public DbSet<Party> Parties { get; set; }
    public DbSet<Controller> Controllers { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
}
