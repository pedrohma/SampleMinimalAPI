namespace SampleMinimal.Infra.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _entities;
        public Repository(ApplicationDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("EntitiesContext");

            _context = context;
            _entities = context.Set<T>();
        }
        public async Task<List<T>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task<List<T>> QueryAsync(Expression<Func<T, bool>> filter)
        {
            return await _entities.Where(filter).ToListAsync();
        }

        public async Task<T> QuerySingleAsync(Expression<Func<T, bool>> filter)
        {
            return await _entities.Where(filter).FirstOrDefaultAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<T> GetByGuidAsync(Guid guid)
        {
            return await _entities.FindAsync(guid);
        }

        public async Task<T> GetByNameAsync(string name)
        {
            return await _entities.FindAsync(name);
        }

        public async Task<bool> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _entities.Add(entity);
            var results = await SaveAsync();

            return results;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _entities.Update(entity);
            var results = await SaveAsync();

            return results;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _entities.Remove(entity);
            var results = await SaveAsync();

            return results;
        }

        public async Task<bool> SaveAsync()
        {
            var results = await _context.SaveChangesAsync();

            if (results > 0)
                return true;

            return false;
        }
    }
}
