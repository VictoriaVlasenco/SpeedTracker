using System;
using System.Collections.Generic;
using System.Text;

namespace SpeedCheck.DAL.Repositories.Infrastructure
{
    public interface IRepository<TEntity>
    {
        void Insert(TEntity entity);

        IEnumerable<TEntity> SelectPage(DateTime date, int page, int pageSize, out int totalCount);

        long Count(DateTime date);
    }
}
