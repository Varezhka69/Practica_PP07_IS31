using System.ComponentModel.DataAnnotations;

public class Fabric
{
    [Key] 
    public int FabricID { get; set; }[Required]  
public string Name { get; set; }

public string Type { get; set; }

public string Description { get; set; }
}
