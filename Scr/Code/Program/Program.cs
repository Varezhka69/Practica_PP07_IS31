using System;
using System.Linq;

namespace TextileDefectTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new TextileDefectContext())
            {
                var defectService = new DefectService(context);

                // Пример использования сервиса:
                defectService.CreateFabric("Хлопок", "Натуральная", "100% хлопок");
                var fabric = defectService.GetFabric(1); // Предполагаем, что ткань с ID 1 только что создана
                if (fabric != null)
                {
                    defectService.CreateDefect(fabric.FabricID, "Дырка", "Верхний левый угол", "path/to/image.jpg", "Небольшая дырка", DateTime.Now);
                    var defects = defectService.GetDefectsByFabric(fabric.FabricID);
                    foreach (var defect in defects)
                    {
                        Console.WriteLine($"Тип дефекта: {defect.Type}, Местоположение: {defect.Location}");
                    }
                }
                else
                {
                    Console.WriteLine("Ткань не найдена.");
                }
            }

            Console.ReadKey();
        }
    }
}
