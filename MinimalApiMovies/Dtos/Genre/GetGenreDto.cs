using MinimalApiMovies.Entities;

namespace MinimalApiMovies.Dtos.Genre;

public class GetGenreDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
}


public static class GenreExtensions
{
    public static GetGenreDto FromEntity(this Entities.Genre entity)
    {
        return new GetGenreDto
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
}