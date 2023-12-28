using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

builder.Services.AddHttpClient<iCommandDataClient, HttpCommandDataclient>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


Console.WriteLine($"--> {builder.Configuration["CommandService"]}");


if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Running DB MSSQLServer");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else {
    Console.WriteLine("--> Running DB inMem");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem"));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

PrepDb.PrepPopulation(app , builder.Environment.IsProduction());

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
