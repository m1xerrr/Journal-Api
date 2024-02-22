namespace Journal.DAL.Interfaces
{
    public interface IBaseRepository<T>
    {
        List<T> SelectAll();

        Task<bool> Create(T entity);

        Task<bool> Delete(T entity);

        Task<bool> Edit(T entity);
    }
}
