using MinimalApiMovies.Dtos.Genre;

namespace MinimalApiMovies.Mappings;

public static class GenreMapping
{
    public static GetGenreDto? FromEntity(this Entities.Genre? entity)
    {
        if (entity == null) return null;

        return new GetGenreDto
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
}
