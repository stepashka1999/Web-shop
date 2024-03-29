﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using Web_shop.DataAccess.Data;
using Web_shop.Utility;

namespace Web_shop.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public IQueryable<T> All => _dbSet;

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public T Find(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, IEnumerable<string> includeProperties = null, bool isTracking = true)
        {
            var result = _dbSet.Where(predicate);

            includeProperties ??= Enumerable.Empty<string>();
            foreach (var prop in includeProperties)
            {
                result.Include(prop);
            }

            result = orderBy is null
                ? result
                : orderBy(result);

            return isTracking ? result : result.AsNoTracking();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate, IEnumerable<string> includeProperties = null, bool isTracking = true)
        {
            var result = _dbSet.Where(predicate);

            includeProperties ??= Enumerable.Empty<string>();
            foreach (var prop in includeProperties)
            {
                result.Include(prop);
            }


            return isTracking ? result.FirstOrDefault() : result.AsNoTracking().FirstOrDefault();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public IEnumerable<SelectListItem> GetDropdownList(string paramenterType)
        {
            return paramenterType switch
            {
                WC.ApplicationTypeName => _dbContext.ApplicationType.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                WC.CategoryName => _dbContext.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                _ => Enumerable.Empty<SelectListItem>(),
            };
        }
    }
}
