using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StandPoint.Models;

namespace StandPoint.Data.EF
{
    public class EntityRepository<TObject, TKey> where TObject : class, IEntityBase<TKey>
    {
        protected DbContext DB;

        public EntityRepository(DbContext db)
        {
            DB = db;
        }

        public ICollection<TObject> GetAll()
        {
            return DB.Set<TObject>().ToList();
        }

        public async Task<ICollection<TObject>> GetAllAsync()
        {
            return await DB.Set<TObject>().ToListAsync();
        }

        public TObject Get(TKey id)
        {
            return DB.Set<TObject>().Find(id);
        }

        public async Task<TObject> GetAsync(long id)
        {
            return await DB.Set<TObject>().FindAsync(id);
        }

        public TObject Find(Expression<Func<TObject, bool>> match)
        {
            return DB.Set<TObject>().SingleOrDefault(match);
        }

        public async Task<TObject> FindAsync(Expression<Func<TObject, bool>> match)
        {
            return await DB.Set<TObject>().SingleOrDefaultAsync(match);
        }

        public ICollection<TObject> FindAll(Expression<Func<TObject, bool>> match)
        {
            return DB.Set<TObject>().Where(match).ToList();
        }

        public async Task<ICollection<TObject>> FindAllAsync(Expression<Func<TObject, bool>> match)
        {
            return await DB.Set<TObject>().Where(match).ToListAsync();
        }

        public TObject Add(TObject t)
        {
            DB.Set<TObject>().Add(t);
            DB.SaveChanges();
            return t;
        }

        public async Task<TObject> AddAsync(TObject t)
        {
            DB.Set<TObject>().Add(t);
            await DB.SaveChangesAsync();
            return t;
        }

        public TObject Update(TObject updated)
        {
            if (updated == null || !updated.HasId)
                return null;

            var existing = DB.Set<TObject>().Find(updated.Id);
            if (existing != null)
            {
                DB.Entry(existing).CurrentValues.SetValues(updated);
                DB.SaveChanges();
            }
            return existing;
        }

        public async Task<TObject> UpdateAsync(TObject updated)
        {
            if (updated == null || !updated.HasId)
                return null;

            var existing = await DB.Set<TObject>().FindAsync(updated.Id);
            if (existing != null)
            {
                DB.Entry(existing).CurrentValues.SetValues(updated);
                await DB.SaveChangesAsync();
            }
            return existing;
        }

        public void Delete(TObject t)
        {
            DB.Set<TObject>().Remove(t);
            DB.SaveChanges();
        }

        public async Task<int> DeleteAsync(TObject t)
        {
            DB.Set<TObject>().Remove(t);
            return await DB.SaveChangesAsync();
        }

        public int Count()
        {
            return DB.Set<TObject>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await DB.Set<TObject>().CountAsync();
        }
    }
}
