using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Inspection
{
    [Key]
    public int InspectionID { get; set; }[ForeignKey("Fabric")]
public int FabricID { get; set; }

[ForeignKey("User")]
public int UserID { get; set; }

public DateTime Date { get; set; }

public string Notes { get; set; }

public Fabric Fabric { get; set; } 
public User User { get; set; }     
}
