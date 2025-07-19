using Microsoft.EntityFrameworkCore;

namespace MinimalApiMovies;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    
}