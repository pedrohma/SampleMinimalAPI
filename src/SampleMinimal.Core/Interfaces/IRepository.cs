namespace SampleMinimal.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> QueryAsync(Expression<Func<T, bool>> filter);
        Task<T> QuerySingleAsync(Expression<Func<T, bool>> filter);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByGuidAsync(Guid guid);
        Task<T> GetByNameAsync(string name);
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<bool> SaveAsync();
    }
}
