using Domain.Entities;
using Infraestructure.Context;
using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            var set = db.Set<Todo<string>>();
            if (await set.AnyAsync())
                return;

            set.AddRange(
              new Todo<string>("Comprar víveres", "Comprar ñames, yuca y platano", DateTime.Today.AddDays(1), "Metadato1", "Pending"),
              new Todo<string>("Llamar al doctor", "Llamar al cardiologo para pregunta sobre el dolor en el pecho", DateTime.Today.AddDays(2), "Metadato2", "Pending"),
              new Todo<string>("Enviar informe", "Revisar los detalles sobre el informe de gastos", DateTime.Today.AddDays(3), "Metadato3", "Pending")
          );

            await db.SaveChangesAsync();
        }
    }
}


