using Microsoft.EntityFrameworkCore;

namespace MinimalApiMovies.Services.Genre;

public class GenreRepository(ApplicationDbContext context) : IGenreRepository
{
    public async Task<List<Entities.Genre>?> GetAll()
    {
        return await context.Genres.ToListAsync();
    }

    public async Task<Entities.Genre?> GetById(long id)
    {
        return await context.Genres.FindAsync(id);
    }

    public async Task<bool> IsExist(long id)
    {
        return await context.Genres.AnyAsync( x=>x.Id == id);
    }

    public async Task<long> Add(Entities.Genre genre)
    {
       await context.Genres.AddAsync(genre);
       await context.SaveChangesAsync();
       return genre.Id;
    }

    public Task<long> Update(Entities.Genre genre)
    {
        context.Genres.Update(genre);
        return context.SaveChangesAsync().ContinueWith(task => genre.Id);
    }

    public async Task<bool> Delete(long id)
    {
        var genre = await GetById(id);
        if (genre is null)
        {
            return false;
        }
        context.Genres.Remove(genre);
        return await context.SaveChangesAsync() > 0;
    }
}