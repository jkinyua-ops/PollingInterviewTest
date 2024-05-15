using PollingInterviewTest.Data;

namespace PollingInterviewTest.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<List<T>> GetAll();
        Task<List<T>> GetAll(Func<T, bool> expression);
        Task<T?> GetById(int id);
        Task Insert(T data);
    }
}