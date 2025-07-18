using MinimalApiMovies.Entities.Base;

namespace MinimalApiMovies.Entities;

public class Genre : BaseEntity
{
    public required string Name { get; set; }
}