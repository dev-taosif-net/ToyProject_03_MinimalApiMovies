using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiMovies.Dtos.Genre;
using MinimalApiMovies.Entities;
using MinimalApiMovies.Services.Genre;

namespace MinimalApiMovies.Endpoints;

public static class GenreEndpoint
{
    public static RouteGroupBuilder MapGenreEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetGenres)
            .RequireCors("AllowAllOrigins")
            .CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60)).Tag("Get-Genres"));

        group.MapGet("/{id:long}", GetGenreById)
            .CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60)));

        group.MapPost("/", GenreCreate);

        group.MapPut("/{id:long}", GenreUpdate);

        group.MapDelete("/{id:long}", GenreDelete);

        return group;
    }
    static async Task<Ok<List<GetGenreDto>>> GetGenres(IGenreRepository genreRepository)
    {
        var data = await genreRepository.GetAll();

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
}