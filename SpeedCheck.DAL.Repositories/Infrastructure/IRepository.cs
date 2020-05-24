using System;
using System.Collections.Generic;
using System.Text;

namespace SpeedCheck.DAL.Repositories.Infrastructure
{
    public interface IRepository<TEntity>
    {
        int SaveChanges();

        //TEntity Find(params object[] keyValues);

        //IQueryable<TEntity> SelectQuery(string query, params object[] parameters);

        void Insert(TEntity entity);

        //IEnumerable<TEntity> SelectPage(int page, int pageSize, out int totalCount);
        IEnumerable<TEntity> SelectPage(int page, int pageSize, out int totalCount);

        //void InsertRange(IEnumerable<TEntity> entities);

        //void InsertOrUpdateGraph(TEntity entity);

        //void InsertGraphRange(IEnumerable<TEntity> entities);

        //void Update(TEntity entity);

        //void Delete(object id);

        //void Delete(TEntity entity);

        //IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query);

        //IQueryFluent<TEntity> Query();

        //IQueryable<TEntity> Queryable();

        //IRepository<T> GetRepository<T>() where T : class;
    }
}
