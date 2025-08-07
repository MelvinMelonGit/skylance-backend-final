using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using skylance_backend.Data;
using skylance_backend.Middlewares;
using skylance_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container.

// Remove duplicate AddDbContext and consolidate with options

builder.Services.AddDbContext<SkylanceDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ).UseLazyLoadingProxies());

builder.Services.AddScoped<ITripService, TripService>();

// Bind the FastApi section to a POCO
builder.Services.Configure<FastApi>(
    builder.Configuration.GetSection("FastApi")
);

// Register MLService, pulling BaseUrl from the bound options
builder.Services.AddHttpClient<MLService>((sp, client) =>
{
    // resolve the options
    var opts = sp.GetRequiredService<IOptions<FastApi>>().Value;
    client.BaseAddress = new Uri(opts.BaseUrl);
});
// Register the background worker to catch and update bookings without prediction
builder.Services.AddHostedService<BookingPredictionWorker>();

builder.Services.AddControllers();

// Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use CORS BEFORE routing (and BEFORE Authorization)
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<AuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Initialize DB with migrations instead of EnsureCreated()
initDB();

app.Run();

// init our database
void initDB()
{
// create the environment to retrieve our database context
    using (var scope = app.Services.CreateScope())
    {
// get database context from DI-container
        var ctx = scope.ServiceProvider.GetRequiredService<SkylanceDbContext>();
        if (! ctx.Database.CanConnect())
            ctx.Database.EnsureCreated(); // create database
    }
}

