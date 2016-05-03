using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericPaymentService.DAL
{
    /// <summary>
    /// Entity repository pattern class.  Inherit this to create a simple repository to your entities.
    /// Currently does not support inject of entity context, so it will create its own context.
    /// </summary>
    public abstract class EntityRepository<TEntityContext, TEntity> : IDisposable
        where TEntity : class
        where TEntityContext : DbContext, new()
    {
        protected TEntityContext Context { get; set; }
        protected DbSet<TEntity> DbSet { get; set; }

        public EntityRepository()
        {
            Context = new TEntityContext();
            DbSet = Context.Set<TEntity>();
        }

        public TEntity Get(Func<TEntity, bool> query)
        {
            try
            {
                return DbSet.FirstOrDefault(query);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return null;
        }

        public IEnumerable<TEntity> GetAll(Func<TEntity, bool> query)
        {
            try
            {
                return DbSet.Where(query).ToList();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return null;
        }

        public void Insert(TEntity entity)
        {
            try
            {
                DbSet.Add(entity);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void Update(TEntity entity)
        {
            try
            {
                DbSet.Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void Delete(object id)
        {
            try
            {
                var entity = DbSet.Find(id);
                Delete(entity);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void Delete(TEntity entity)
        {
            try
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
                }
                DbSet.Remove(entity);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <summary>
        /// Persists any changes
        /// </summary>
        public void Save()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        /// <summary>
        /// Allows you to override the exception handling
        /// By default, it will rethrow the original exception.
        /// You may want to log these exceptions in your inheriting classes
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void HandleException(Exception ex)
        {
            throw ex;
        }
    }
}
