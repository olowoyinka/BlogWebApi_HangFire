using BlogWebApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BlogWebApi.Application.Interfaces.Persistence
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> Get(Expression<Func<T, bool>> filter = null,
                                            Func<IQueryable<T>,
                                            IOrderedQueryable<T>> orderBy = null,
                                            string includeProperties = "");
        T Get(long id);
        int Insert(T entity);
        bool Update(T entity);
        bool Delete(T entity);
    }
}