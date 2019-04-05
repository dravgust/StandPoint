using StandPoint.Models;

namespace StandPoint.Data
{
    public interface IDataAccessObject
    {
        T Get<T, TKey>(TKey id) where T : class, IEntityBase<TKey>;
        void Set<T, TKey>(T entity) where T : class, IEntityBase<TKey>;
        void Delete<T, TKey>(TKey id) where T : class, IEntityBase<TKey>;
        void Delete<T, TKey>(T entity) where T : class, IEntityBase<TKey>;
        //Criteria<TCriteria> Criteria<T, TCriteria>() where T : IdentNULongEntity where TCriteria : ICriteria;
    }
}
