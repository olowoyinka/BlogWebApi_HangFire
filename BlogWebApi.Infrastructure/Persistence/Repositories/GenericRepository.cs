using BlogWebApi.Application.Interfaces.Persistence;
using BlogWebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BlogWebApi.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly BlogDbContext context;
        private DbSet<T> entities;

        public GenericRepository(BlogDbContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
        }

        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter = null,
                                            Func<IQueryable<T>, 
                                            IOrderedQueryable<T>> orderBy = null,
                                            string includeProperties = "")
        {
            IQueryable<T> query = entities;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual T Get(long id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }

        public virtual int Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entities.Add(entity);
            context.SaveChanges();

            return entity.Id;
        }

        public virtual bool Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return context.SaveChanges() > 0;
        }

        public virtual bool Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entity.DeleteAt = DateTime.UtcNow;
            entity.IsDeleted = true;

            return Update(entity);
        }
    }
}