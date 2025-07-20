using Microsoft.EntityFrameworkCore;
using MinimalApiMovies;
using MinimalApiMovies.Endpoints;
using MinimalApiMovies.Services.Genre;

var builder = WebApplication.CreateBuilder(args);

#region =========Services=========

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
        policyBuilder.WithOrigins(builder.Configuration["AllowedCORSOrigins"] ?? string.Empty).AllowAnyMethod()
            .AllowAnyHeader());
    options.AddPolicy("AllowAllOrigins",
        policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOutputCache();

builder.Services.AddScoped<IGenreRepository, GenreRepository>();

#endregion

var app = builder.Build();

#region =========Pipeline && Middleware=========

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors();
app.UseOutputCache();

app.MapGroup("/genres").MapGenreEndpoints().WithTags("Genres");

#endregion

app.Run();