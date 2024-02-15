namespace Journal.DAL.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task<IQueryable<T>> SelectAll();

        Task<bool> Create(T entity);

        Task<bool> Delete(T entity);

        Task<bool> Edit(T entity);
    }
}
