using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
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

var genreMapGroup = app.MapGroup("/genres");
// .WithTags("Genres")
// .WithOpenApi();

genreMapGroup.MapGet("/", GetGenres).RequireCors("AllowAllOrigins")
    .CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60)).Tag("Get-Genres"));
genreMapGroup.MapGet("/{id:long}", GetGenreById).CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60)));
genreMapGroup.MapPost("/", GenreCreate);
genreMapGroup.MapPut("/{id:long}", GenreUpdate);
genreMapGroup.MapDelete("/{id:long}", GenreDelete);

#endregion

app.Run();
return;

static async Task<Ok<List<Genre>>> GetGenres(IGenreRepository genreRepository)
{
    return TypedResults.Ok(await genreRepository.GetAll());
}

static async Task<Results<Ok<Genre>, NotFound>> GetGenreById(long id, IGenreRepository genreRepository)
{
    var data = await genreRepository.GetById(id);
    return data == null ? TypedResults.NotFound() : TypedResults.Ok(data);
}

static async Task<Created<long>> GenreCreate(Genre genre, IGenreRepository genreRepository, IOutputCacheStore store)
{
    var id = await genreRepository.Add(genre);
    await store.EvictByTagAsync("Get-Genres", CancellationToken.None);
    return TypedResults.Created($"/genres/{id}", id);
}

static async Task<Results<NoContent, NotFound>> GenreUpdate(long id, Genre genre, IGenreRepository genreRepository,
    IOutputCacheStore store)
{
    if (!await genreRepository.IsExist(id))
    {
        return TypedResults.NotFound();
    }

    genre.Id = id;
    await genreRepository.Update(genre);
    await store.EvictByTagAsync("Get-Genres", CancellationToken.None);
    return TypedResults.NoContent();
}

static async Task<Results<NoContent, NotFound>> GenreDelete(long id, IGenreRepository genreRepository,
    IOutputCacheStore store)
{
    if (!await genreRepository.IsExist(id))
    {
        return TypedResults.NotFound();
    }

    await genreRepository.Delete(id);
    await store.EvictByTagAsync("Get-Genres", CancellationToken.None);
    return TypedResults.NoContent();
}