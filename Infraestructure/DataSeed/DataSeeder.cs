using Domain.Entities;
using Infraestructure.Context;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            var set = db.Set<Todo>();
            if (await set.AnyAsync())
                return;

            set.AddRange(
              new Todo("Comprar víveres", "Comprar ñames, yuca y platano", DateTime.Today.AddDays(1), "Metadato1", Status.Pendiente, null),
              new Todo("Llamar al doctor", "Llamar al cardiologo para pregunta sobre el dolor en el pecho", DateTime.Today.AddDays(2), "Metadato 2", Status.Completado, Priority.Low),
              new Todo("Enviar informe", "Revisar los detalles sobre el informe de gastos", DateTime.Today.AddDays(3), null, Status.Pendiente, Priority.High)
          );

            await db.SaveChangesAsync();
        }
    }
}


