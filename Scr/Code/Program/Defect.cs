using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Defect
{
    [Key]
    public int DefectID { get; set; }[ForeignKey("Fabric")]
public int FabricID { get; set; }

public string Type { get; set; }

public string Location { get; set; }

public string Image { get; set; }

public string Description { get; set; }

public DateTime Time { get; set; }

public Fabric Fabric { get; set; }
}
