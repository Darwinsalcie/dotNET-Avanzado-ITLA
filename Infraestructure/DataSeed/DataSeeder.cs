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
              new Todo(1, "Comprar víveres", "Comprar ñames, yuca y platano", DateTime.Today.AddDays(1), "Metadato1", Status.Pendiente, null),
              new Todo(2, "Llamar al doctor", "Llamar al cardiologo para pregunta sobre el dolor en el pecho", DateTime.Today.AddDays(2), "Metadato 2", Status.Completado, Priority.Low),
              new Todo(3, "Enviar informe", "Revisar los detalles sobre el informe de gastos", DateTime.Today.AddDays(3), null, Status.Pendiente, Priority.High)
          );

            await db.SaveChangesAsync();
        }
    }
}


