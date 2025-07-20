namespace MinimalApiMovies.Services.Genre;

public interface IGenreRepository
{
    Task<List<Entities.Genre>?> GetAll();
    Task<Entities.Genre?> GetById(long id);
    Task<bool> IsExist(long id);
    Task<long> Add(Entities.Genre genre);
    Task<long> Update(Entities.Genre genre);
    Task<bool> Delete(long id);

}