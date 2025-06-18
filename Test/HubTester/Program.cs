using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

async Task RunAsync()
{
    Console.WriteLine("🔐 Introduce tu JWT:");
    string? jwt;

    // Bucle hasta que se introduzca un token válido
    do
    {
        jwt = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(jwt))
        {
            Console.WriteLine("❌ El token no puede estar vacío. Inténtalo de nuevo:");
        }
    } while (string.IsNullOrWhiteSpace(jwt));

    var connection = new HubConnectionBuilder()
        .WithUrl("https://localhost:7230/ReceiveNotificacion", options =>
        {
            options.AccessTokenProvider = () => Task.FromResult(jwt);
        })
        .WithAutomaticReconnect()
        .Build();

    // Evento que escucha mensajes del hub
    connection.On<object>("ReceiveNotificacion", payload =>
    {
        Console.WriteLine($"📨 Mensaje recibido: {payload}");
    });

    try
    {
        Console.WriteLine("🔌 Conectando al Hub...");
        await connection.StartAsync();
        Console.WriteLine("✅ Conectado. Esperando mensajes...\nPresiona [Enter] para salir.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error al conectar con el Hub:");
        Console.WriteLine(ex.Message);
        Console.WriteLine("🔁 Inténtalo de nuevo.\n");
        await RunAsync(); // Reintenta
        return;
    }

    Console.ReadLine(); // Mantener el programa vivo
}

// Inicia el cliente
await RunAsync();
