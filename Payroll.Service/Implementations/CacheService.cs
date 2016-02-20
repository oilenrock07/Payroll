using System;
using System.Runtime.Caching;
using Payroll.Service.Interfaces;

namespace Payroll.Service.Implementations
{
    public class CacheService : ICacheService
    {
        private static ObjectCache _cache;

        private const int DefaultExpiryMinutes = 30;

        public CacheService( ObjectCache cache )
        {
            _cache = cache;
        }

        public T Get<T>(string key, TimeSpan? timespan = null, string regionName = null, Func<T> method = null)
        {
            if( !timespan.HasValue )
            {
                timespan = new TimeSpan( 0, DefaultExpiryMinutes, 0 );
            }

            return Get(key, new CacheItemPolicy { SlidingExpiration = timespan.Value }, regionName, method);
        }

        public T Get<T>( string key, CacheItemPolicy policy, string regionName = null, Func<T> method = null )
        {
            var value = _cache.Get( key, regionName );

            if( value == null && method != null && policy != null )
            {
                value = method();

                if( value != null ) Set( key, value, policy );
            }

            return value == null ? default( T ) : (T) value;
        }

        public void Set( string key, object value, TimeSpan timespan, string regionName = null )
        {
            Set( key, value, new CacheItemPolicy {SlidingExpiration = timespan}, regionName );
        }

        public void Set( string key, object value, CacheItemPolicy policy = null, string regionName = null )
        {
            if (policy == null)
            {
                policy = new CacheItemPolicy
                {
                    SlidingExpiration = new TimeSpan( 0, DefaultExpiryMinutes, 0 )
                };
            }

            _cache.Set(key, value, policy, regionName);
        }

        public T Remove<T>( string key, string regionName = null )
        {
            var value = _cache.Remove( key, regionName );

            return value == null ? default( T ) : (T) value;
        }
    }
}
