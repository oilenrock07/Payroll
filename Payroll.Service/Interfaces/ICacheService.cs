using System;
using System.Runtime.Caching;

namespace Payroll.Service.Interfaces
{
    public interface ICacheService
    {
        /// <summary>
        /// When overridden in a derived class, gets the specified cache entry from the cache as an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="regionName"></param>
        /// <param name="method"></param>
        /// <param name="timespan"></param>
        /// <returns></returns>
        T Get<T>( string key, TimeSpan? timespan = null, string regionName = null, Func<T> method = null );

        /// <summary>
        /// When overridden in a derived class, gets the specified cache entry from the cache as an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="method">Optional method for populating the value if it does not exist in the cache</param>
        /// <param name="policy">CacheItemPolicy for adding a non-existant item to the cache.</param>
        /// <returns></returns>
        T Get<T>( string key, CacheItemPolicy policy, string regionName = null, Func<T> method = null );

        /// <summary>
        /// When overridden in a derived class, inserts a cache entry into the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="policy">The policy.</param>
        /// <param name="regionName">Name of the region.</param>
        void Set(string key, object value, CacheItemPolicy policy = null, string regionName = null);

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="timespan">The timespan.</param>
        /// <param name="regionName">Name of the region.</param>
        void Set(string key, object value, TimeSpan timespan, string regionName = null);

        /// <summary>
        /// When overridden in a derived class, removes the cache entry from the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="regionName">Name of the region.</param>
        /// <returns>An object that represents the value of the removed cache entry that was specified by the key, or null if the specified entry was not found.</returns>
        T Remove<T>(string key, string regionName = null);
    }
}