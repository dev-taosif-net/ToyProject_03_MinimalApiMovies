using MinimalApiMovies.Entities;

namespace MinimalApiMovies.Dtos.Genre;

public class GetGenreDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
}

