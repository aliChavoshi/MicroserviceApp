using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities.Base;
using Ordering.Core.Repositories.Base;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        protected readonly OrderContext Context;

        public Repository(OrderContext context)
        {
            Context = context;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await Context.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await Context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null,
            bool disableTracking = true)
        {
            IQueryable<T> queryable = Context.Set<T>();
            if (disableTracking)
                queryable = queryable.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(includeString))
                queryable = queryable.Include(includeString);
            if (predicate != null)
                queryable = queryable.Where(predicate);
            if (orderBy != null)
                return await orderBy(queryable).ToListAsync();
            return await queryable.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null, string includeString = null, bool disableTracking = true)
        {
            IQueryable<T> queryable = Context.Set<T>();
            if (disableTracking)
                queryable = queryable.AsNoTracking();
            if (includes != null)
                queryable = includes.Aggregate(queryable, (current, include) => current.Include(include));
            if (!string.IsNullOrWhiteSpace(includeString))
                queryable = queryable.Include(includeString);
            if (predicate != null)
                queryable = queryable.Where(predicate);
            if (orderBy != null)
                return await orderBy(queryable).ToListAsync();
            return await queryable.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await Context.Set<T>().AddAsync(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            Context.Set<T>().Remove(entity);
            await Context.SaveChangesAsync();
        }
    }
}