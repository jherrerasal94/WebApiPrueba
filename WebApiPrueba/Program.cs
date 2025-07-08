using Microsoft.EntityFrameworkCore;
using WebApiPrueba.Context;

var builder = WebApplication.CreateBuilder(args);

// --- Configuración de CORS ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; // Define un nombre para tu política

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200") // Permite solo este origen (tu Angular app)
                                .AllowAnyHeader()       // Permite cualquier encabezado HTTP
                                .AllowAnyMethod();      // Permite cualquier método HTTP (GET, POST, PUT, DELETE)
                                                        // .AllowCredentials(); // Si vas a usar cookies o autenticación basada en credenciales (ej. JWT), deberías añadir esto
                      });
});
// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
