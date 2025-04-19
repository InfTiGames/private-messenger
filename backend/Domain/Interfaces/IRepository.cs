namespace Domain.Interfaces;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task DeleteAsync(Guid id);
    Task UpdateAsync(T entity);
}
