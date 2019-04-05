using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StandPoint.Models;

namespace StandPoint.Data.EF
{
    public abstract class DataAccessObject : IDataAccessObject, IDisposable
    {
        protected DbContext DbContext;
        protected IDbContextTransaction Transaction;

        protected DataAccessObject(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public EntityRepository<T, TKey> GetRepository<T, TKey>() where T : class, IEntityBase<TKey>
        {
            return new EntityRepository<T, TKey>(DbContext);
        }

        public void StartTransaction()
        {
            Transaction = DbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            Transaction?.Commit();
            Transaction = null;
        }

        public void RollbackTransaction()
        {
            Transaction.Rollback();
            Transaction = null;
        }

        public ICollection<T> GetAll<T, TKey>() where T : class, IEntityBase<TKey>
        {
            return GetRepository<T, TKey>().GetAll();
        }

        public T Get<T, TKey>(TKey id) where T : class, IEntityBase<TKey>
        {
            return GetRepository<T, TKey>().Get(id);
        }

        public void Set<T, TKey>(T entity) where T : class, IEntityBase<TKey>
        {
            var repo = GetRepository<T, TKey>();
            if (entity.HasId)
                repo.Update(entity);
            else
            {
                repo.Add(entity);
            }
        }

        public void Delete<T, TKey>(TKey id) where T : class, IEntityBase<TKey>
        {
            var repo = GetRepository<T, TKey>();
            var entity = repo.Get(id);

            if (entity == null)
                throw new ArgumentException($"Entity {typeof(T).Name} not found by ID {id}");

            Delete<T, TKey>(entity);
        }

        public void Delete<T, TKey>(T entity) where T : class, IEntityBase<TKey>
        {
            GetRepository<T, TKey>().Delete(entity);
        }

        public void Dispose()
        {
            CommitTransaction();
            Transaction?.Dispose();
            DbContext?.Dispose();
        }
    }
}
