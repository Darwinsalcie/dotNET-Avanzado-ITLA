using API.Extensions;
using API.Hubs;
using API.Middlewares;
using Infraestructure.Context;
using Infraestructure.Data;
using Infraestructure.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);


// CORS
builder.Services.AddCors(o =>
  o.AddPolicy("AllowAll", p =>
    p.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader()));


// 3) Swagger con esquema JWT
builder.Services.AddSwaggerGen(c =>
{
c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

// Definición del esquema Bearer
c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    Description = "Escribe 'Bearer' [espacio] y luego tu token JWT"
});


    // Aplicar globalmente a endpoints protegidos
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//Habilitar CORS
app.UseCors("AllowAll");

//Primero Autentica y luego Autoriza
app.UseAuthentication();
app.UseAuthorization();


// Middleware de manejo de excepciones personalizado
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.MapHub<NotificationHub>("/ReceiveNotificacion");

// Seed de datos
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DataSeeder.SeedAsync(db);
}

app.Run();

