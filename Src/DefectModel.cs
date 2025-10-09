using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Party
{
    [Key]
    [Column("ID_Party")]
    public int PartyID { get; set; }

    [Required]
    [StringLength(50)]
    public string Number { get; set; }

    [Column("Production_date")]
    public DateTime ProductionDate { get; set; }

    public int Quantity { get; set; }

    public virtual ICollection<Smartphone> Smartphones { get; set; }

    public Party()
    {
        Smartphones = new HashSet<Smartphone>();
        ProductionDate = DateTime.Today;
        Quantity = 1;
    }
}

public class Smartphone
{
    [Key]
    [Column("ID_Smartphone")]
    public int SmartphoneID { get; set; }

    [Required]
    [StringLength(100)]
    public string Model { get; set; }

    [Required]
    [StringLength(100)]
    public string Manufacturer { get; set; }

    [Required]
    [StringLength(50)]
    public string Number { get; set; }

    [Column("Production_date")]
    public DateTime ProductionDate { get; set; }

    [ForeignKey("Party")]
    [Column("ID_Party")]
    public int PartyID { get; set; }

    public virtual Party Party { get; set; }
    public virtual ICollection<Screen> Screens { get; set; }

    public Smartphone()
    {
        Screens = new HashSet<Screen>();
        ProductionDate = DateTime.Today;
        Manufacturer = "Не указан";
    }
}

public class Controller
{
    [Key]
    [Column("ID_Controller")]
    public int ControllerID { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string Surname { get; set; }

    public virtual ICollection<Defect> Defects { get; set; }

    public Controller()
    {
        Defects = new HashSet<Defect>();
        Name = "Имя";
        Surname = "Фамилия";
    }

    [NotMapped]
    public string FullName => $"{Name} {Surname}";
}

public class Screen
{
    [Key]
    [Column("ID_Screen")]
    public int ScreenID { get; set; }

    [ForeignKey("Smartphone")]
    [Column("ID_Smartphone")]
    public int SmartphoneID { get; set; }

    [Required]
    [StringLength(50)]
    public string Material { get; set; }

    [Required]
    [StringLength(100)]
    public string Supplier { get; set; }

    [Column("Installation_date")]
    public DateTime InstallationDate { get; set; }

    public virtual Smartphone Smartphone { get; set; }
    public virtual ICollection<Defect> Defects { get; set; }

    public Screen()
    {
        Defects = new HashSet<Defect>();
        InstallationDate = DateTime.Today;
        Material = "Стекло";
        Supplier = "Не указан";
    }
}

public class Defect
{
    [Key]
    [Column("ID_Defect")]
    public int DefectID { get; set; }

    [ForeignKey("Screen")]
    [Column("ID_Screen")]
    public int ScreenID { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; }

    public decimal Size { get; set; }

    [Required]
    [StringLength(100)]
    public string Coordinates { get; set; }

    [Column("Date_discovery")]
    public DateTime DateDiscovery { get; set; }

    [ForeignKey("Controller")]
    [Column("ID_Controller")]
    public int ControllerID { get; set; }

    public virtual Screen Screen { get; set; }
    public virtual Controller Controller { get; set; }
    public virtual ICollection<Image> Images { get; set; }

    public Defect()
    {
        Images = new HashSet<Image>();
        DateDiscovery = DateTime.Now;
        Type = "Царапина";
        Coordinates = "X:0,Y:0";
    }
}

public class Image
{
    [Key]
    [Column("ID_Image")]
    public int ImageID { get; set; }

    [ForeignKey("Defect")]
    [Column("ID_Defect")]
    public int DefectID { get; set; }

    [Required]
    [StringLength(500)]
    [Column("File_path")]
    public string FilePath { get; set; }

    public virtual Defect Defect { get; set; }

    public Image()
    {
        FilePath = "";
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
        Database.SetInitializer<DefectContext>(null);
        Configuration.LazyLoadingEnabled = false;
        Configuration.ProxyCreationEnabled = false;
    }

    // Основные таблицы
    public DbSet<Party> Parties { get; set; }
    public DbSet<Smartphone> Smartphones { get; set; }
    public DbSet<Controller> Controllers { get; set; }
    public DbSet<Screen> Screens { get; set; }
    public DbSet<Defect> Defects { get; set; }
    public DbSet<Image> Images { get; set; }

    // Таблицы администрирования
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();

        // Явно указываем имена таблиц
        modelBuilder.Entity<Party>().ToTable("Party");
        modelBuilder.Entity<Smartphone>().ToTable("Smartphone");
        modelBuilder.Entity<Controller>().ToTable("Controller");
        modelBuilder.Entity<Screen>().ToTable("Screen");
        modelBuilder.Entity<Defect>().ToTable("Defect");
        modelBuilder.Entity<Image>().ToTable("Image");
        modelBuilder.Entity<Role>().ToTable("Roles");
        modelBuilder.Entity<User>().ToTable("Users");

        base.OnModelCreating(modelBuilder);
    }
}
