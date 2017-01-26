using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Services.Description;

namespace EmailUploadedDocument.Core.Interface.Data
{
    /// <summary>
    /// An Abstraction of a Data Source
    /// </summary>
    public interface IDataRepository
    {
        /// <summary>
        /// Get all Elements of Type T
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <returns>DbSet of Entities</returns>
        IQueryable<T> Query<T>() where T : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        IQueryable<T> Query<T>(params Expression<Func<T, object>>[] includeProperties) where T : class;

        /// <summary>
        /// Add an Entity to the Persistence Storage
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="item">Entity to be Added</param>
        void Add<T>(T item) where T : class;

        /// <summary>
        /// Delete an Entity from the Persistence Storage
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="item">Entity to be Removed</param>
        void Delete<T>(T item) where T : class;

        /// <summary>
        /// Get a Single Entity from the Persistence Storage using the Entities' ID
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="id">ID of the Entity</param>
        /// <returns>Single Entity or Null</returns>
        T GetById<T>(int id) where T : class;

        /// <summary>
        /// Updates a Specific Entity to the Persistence Storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Update<T>(T item) where T : class;

        /// <summary>
        /// Saves in Memory Changes to the Persistence Storage
        /// </summary>
        /// <returns>Returns True if Successful or False if </returns>
        Operation SaveChanges();
    }

}






