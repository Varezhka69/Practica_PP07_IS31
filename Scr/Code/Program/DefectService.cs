using System;
using System.Linq;

public class DefectService
{
    private readonly TextileDefectContext _context;

    public DefectService(TextileDefectContext context)
    {
        _context = context;
    }

    // Создание ткани
    public void CreateFabric(string name, string type, string description)
    {
        var fabric = new Fabric { Name = name, Type = type, Description = description };
        _context.Fabrics.Add(fabric);
        _context.SaveChanges();
        Console.WriteLine($"Ткань '{name}' успешно создана.");
    }

    // Получение ткани по ID
    public Fabric GetFabric(int fabricId)
    {
        return _context.Fabrics.Find(fabricId);
    }

    // Обновление ткани
    public void UpdateFabric(int fabricId, string name, string type, string description)
    {
        var fabric = _context.Fabrics.Find(fabricId);
        if (fabric != null)
        {
            fabric.Name = name;
            fabric.Type = type;
            fabric.Description = description;
            _context.SaveChanges();
            Console.WriteLine($"Ткань с ID '{fabricId}' успешно обновлена.");
        }
        else
        {
            Console.WriteLine($"Ткань с ID '{fabricId}' не найдена.");
        }
    }

    // Удаление ткани
    public void DeleteFabric(int fabricId)
    {
        var fabric = _context.Fabrics.Find(fabricId);
        if (fabric != null)
        {
            _context.Fabrics.Remove(fabric);
            _context.SaveChanges();
            Console.WriteLine($"Ткань с ID '{fabricId}' успешно удалена.");
        }
        else
        {
            Console.WriteLine($"Ткань с ID '{fabricId}' не найдена.");
        }
    }

    // Создание дефекта
    public void CreateDefect(int fabricId, string type, string location, string image, string description, DateTime time)
    {
        var defect = new Defect { FabricID = fabricId, Type = type, Location = location, Image = image, Description = description, Time = time };
        _context.Defects.Add(defect);
        _context.SaveChanges();
        Console.WriteLine($"Дефект успешно создан для ткани с ID '{fabricId}'.");
    }

    // Получение дефекта по ID
    public Defect GetDefect(int defectId)
    {
        return _context.Defects.Find(defectId);
    }

    // Обновление дефекта
    public void UpdateDefect(int defectId, int fabricId, string type, string location, string image, string description, DateTime time)
    {
        var defect = _context.Defects.Find(defectId);
        if (defect != null)
        {
            defect.FabricID = fabricId;
            defect.Type = type;
            defect.Location = location;
            defect.Image = image;
            defect.Description = description;
            defect.Time = time;
            _context.SaveChanges();
            Console.WriteLine($"Дефект с ID '{defectId}' успешно обновлен.");
        }
        else
        {
            Console.WriteLine($"Дефект с ID '{defectId}' не найден.");
        }
    }

    // Удаление дефекта
    public void DeleteDefect(int defectId)
    {
        var defect = _context.Defects.Find(defectId);
        if (defect != null)
        {
            _context.Defects.Remove(defect);
            _context.SaveChanges();
            Console.WriteLine($"Дефект с ID '{defectId}' успешно удален.");
        }
        else
        {
            Console.WriteLine($"Дефект с ID '{defectId}' не найден.");
        }
    }

    // Пример метода для получения всех дефектов определенной ткани
    public IQueryable<Defect> GetDefectsByFabric(int fabricId)
    {
        return _context.Defects.Where(d => d.FabricID == fabricId);
    }
}
