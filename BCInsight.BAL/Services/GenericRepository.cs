using BCInsight.BAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data;


namespace BCInsight.BAL.Services
{
    public abstract class GenericRepository<C, T> :
         IRepository<T> where T : class where C : DbContext, new()
    {
        private C _entities = new C();
        public C Context
        {
            get { return _entities; }
            set { _entities = value; }
        }

        public void Add(T entity)
        {
            _entities.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _entities.Set<T>().Remove(entity);
        }

        public void Edit(T entity)
        {
            _entities.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _entities.Set<T>().Where(predicate);
            return query;
        }

        public IQueryable<T> GetAll()
        {
            IQueryable<T> query = _entities.Set<T>();
            return query;
        }

        public virtual void Save()
        {
            _entities.SaveChanges();
        }

        public int Count(IEnumerable<T> entities)
        {
            return entities.Count();
        }

        public IEnumerable<T> GetRange(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        IList<Expression<Func<T, object>>> includes = null, int? page = null, int? pageSize = null)
        {
            var query = _entities.Set<T>().AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query.ToList();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _entities.Set<T>().AddRange(entities);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _entities.Set<T>().RemoveRange(entities);
        }

    }
}
