using API.Extensions;
using API.Middlewares;
using Infraestructure.Context;
using Infraestructure.Data;
using Infraestructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices();


// CORS
builder.Services.AddCors(o =>
  o.AddPolicy("AllowAll", p =>
    p.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



//Habilitar CORS
app.UseCors("AllowAll");
app.UseAuthorization();

    app.UseDeveloperExceptionPage();


app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DataSeeder.SeedAsync(db);
}


app.Run();
