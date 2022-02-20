using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Web_shop.DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> All { get; }

        T Find(int id);

        IEnumerable<T> FindAll(Expression<Func<T, bool>> predicate,
                               Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                               IEnumerable<string> includeProperties = null,
                               bool isTracking = true);

        T FirstOrDefault(Expression<Func<T, bool>> predicate,
                         IEnumerable<string> includeProperties = null,
                         bool isTracking = true);

        void Add(T entity);

        public void AddRange(IEnumerable<T> entities);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entity);

        void Save();

        void Update(T entity);

        IEnumerable<SelectListItem> GetDropdownList(string paramenterType);
    }
}
