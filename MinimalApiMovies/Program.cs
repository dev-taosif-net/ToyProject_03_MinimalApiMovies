using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalApiMovies;
using MinimalApiMovies.Entities;
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


app.MapGet("/genres",
        [EnableCors("AllowAllOrigins")] async (IGenreRepository genreRepository) => await genreRepository.GetAll())
    .CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60)).Tag("Get-Genres"));

app.MapGet("/genres/{id:long}", (long id, IGenreRepository genreRepository) =>
    {
        return genreRepository.GetById(id)
            .ContinueWith(task => task.Result is not null ? Results.Ok(task.Result) : Results.NotFound());
    })
    .CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60)));

app.MapPost("/genres", async (Genre genre, IGenreRepository genreRepository, IOutputCacheStore store) =>
{
    var id = await genreRepository.Add(genre);
    await store.EvictByTagAsync("Get-Genres", CancellationToken.None);
    return Results.Created($"/genres/{id}", id);
});

app.MapPut("/genres/{id:long}",
    async (long id, Genre genre, IGenreRepository genreRepository, IOutputCacheStore store) =>
    {
        if (!await genreRepository.IsExist(id))
        {
            return Results.NotFound();
        }

        genre.Id = id;
        await genreRepository.Update(genre);
        await store.EvictByTagAsync("Get-Genres", CancellationToken.None);
        return Results.NoContent();
    });

app.MapDelete("/genres/{id:long}",
    async (long id, IGenreRepository genreRepository, IOutputCacheStore store) =>
    {
        if (!await genreRepository.IsExist(id))
        {
            return Results.NotFound();
        }

        await genreRepository.Delete(id);
        await store.EvictByTagAsync("Get-Genres", CancellationToken.None);
        return Results.NoContent();
    });

#endregion

app.Run();