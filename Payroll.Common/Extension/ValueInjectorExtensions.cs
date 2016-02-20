using System;
using System.Collections.Generic;
using System.Linq;
using Omu.ValueInjecter;
using Omu.ValueInjecter.Injections;

namespace Payroll.Common.Extension
{
    public static class ValueInjectorExtensions
    {
        /// <summary>
        /// Basic injection mapping with specification of the destination type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="injection"></param>
        /// <returns></returns>
        public static T MapItem<T>( this object source, IValueInjection injection = null )
            where T : class, new()
        {
            return GetInjectedInstance<object, T>( source, injection );
        }

        /// <summary>
        /// Injection mapping with an Action to execute after injection
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="itemFunc"></param>
        /// <param name="injection"></param>
        /// <returns></returns>
        public static T MapItem<TS, T>( this TS source, Action<TS, T> itemFunc, IValueInjection injection = null )
            where T : class, new()
        {
            var dest = source.MapItem<T>( injection );

            itemFunc( source, dest );

            return dest;
        }

        /// <summary>
        /// Injection mapping with an Action to execute after injection, with additional parameter
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="source"></param>
        /// <param name="itemFunc"></param>
        /// <param name="arg"></param>
        /// <param name="injection"></param>
        /// <returns></returns>
        public static T MapItem<TS, T, TArg>( this TS source, Action<TS, T, TArg> itemFunc, TArg arg, IValueInjection injection = null )
            where T : class, new()
        {
            var dest = source.MapItem<T>( injection );

            itemFunc( source, dest, arg );

            return dest;
        }

        /// <summary>
        /// Basic injection mapping for a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="injection"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<T> MapCollection<T>( this IEnumerable<object> source, IValueInjection injection = null )
            where T : new()
        {
            var dest = new List<T>();

            // Null or empty just return empty list
            if( source == null || !source.Any() ) return dest;

            dest.AddRange( source.Select( s => GetInjectedInstance<object, T>( s, injection ) ) );

            return dest;
        }

        /// <summary>
        /// Injection mapping for a collection with an Action to execute after injection
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="itemFunc"></param>
        /// <param name="injection"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<T> MapCollection<TS, T>( this IEnumerable<TS> source, Action<TS, T> itemFunc, IValueInjection injection = null )
            where T : new()
        {
            var dest = new List<T>();

            // Null or empty just return empty list
            if( source == null || !source.Any() ) return dest;

            foreach( var s in source )
            {
                var d = GetInjectedInstance<TS, T>( s, injection );
                itemFunc( s, d );
                dest.Add( d );
            }

            return dest;
        }

        /// <summary>
        /// Injection mapping for a collection with an Action to execute after injection, with additional parameter
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TArg"></typeparam>
        /// <param name="source"></param>
        /// <param name="itemFunc"></param>
        /// <param name="arg"></param>
        /// <param name="injection"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<T> MapCollection<TS, T, TArg>( this IEnumerable<TS> source, Action<TS, T, TArg> itemFunc, TArg arg, IValueInjection injection = null )
            where T : new()
        {
            var dest = new List<T>();

            // Null or empty just return empty list
            if( source == null || !source.Any() ) return dest;

            foreach( var s in source )
            {
                var d = GetInjectedInstance<TS, T>( s, injection );
                itemFunc( s, d, arg );
                dest.Add( d );
            }

            return dest;
        }

        private static T GetInjectedInstance<TS, T>( TS source, IValueInjection injection )
            where T : new()
        {
            if( source == null ) return default( T );

            var d = new T();

            if( injection != null )
            {
                d.InjectFrom( injection, source );
            }
            else
            {
                d.InjectFrom( source );
            }

            return d;
        }

        [Obsolete( "Use MapCollection instead" )]
        public static IEnumerable<TV> MapEnumerable<T, TV>( this IEnumerable<T> source, Action<T, TV> itemFunc = null )
            where TV : new()
        {
            return MapEnumerable( source, null, itemFunc );
        }

        [Obsolete( "Use MapCollection instead" )]
        public static IEnumerable<TV> MapEnumerable<T, TV>( this IEnumerable<T> source, IValueInjection injection, Action<T, TV> itemFunc = null )
            where TV : new()
        {
            var dest = new List<TV>();

            // Null or empty just return empty list
            if( source == null || !source.Any() ) return dest;

            foreach( var s in source )
            {
                var d = new TV();

                if( injection != null )
                {
                    d.InjectFrom( injection, s );
                }
                else
                {
                    d.InjectFrom( s );
                }

                if( itemFunc != null )
                {
                    itemFunc( s, d );
                }

                dest.Add( d );
            }

            return dest;
        }

        [Obsolete( "Use MapCollection instead" )]
        public static IEnumerable<TV> MapEnumerable<T, TV, TArg>( this IEnumerable<T> source, Action<T, TV, TArg> itemFunc = null, TArg arg = default( TArg ) )
            where TV : new()
        {
            return MapEnumerable( source, null, itemFunc, arg );
        }

        [Obsolete( "Use MapCollection instead" )]
        public static IEnumerable<TV> MapEnumerable<T, TV, TArg>( this IEnumerable<T> source, IValueInjection injection, Action<T, TV, TArg> itemFunc = null, TArg arg = default( TArg ) )
            where TV : new()
        {
            var dest = new List<TV>();

            // Null or empty just return empty list
            if( source == null || !source.Any() ) return null;

            foreach( var s in source )
            {
                var d = new TV();

                if( injection != null )
                {
                    d.InjectFrom( injection, s );
                }
                else
                {
                    d.InjectFrom( s );
                }

                if( itemFunc != null )
                {
                    itemFunc( s, d, arg );
                }

                dest.Add( d );
            }

            return dest;
        }

        [Obsolete( "Use MapItem instead" )]
        public static TV Map<T, TV, TArg>( this T source, Action<T, TV, TArg> itemFunc = null, TArg arg = default( TArg ) )
            where TV : class, new()
        {
            return Map( source, null, itemFunc, arg );
        }

        [Obsolete( "Use MapItem instead" )]
        public static TV Map<T, TV, TArg>( this T source, IValueInjection injection, Action<T, TV, TArg> itemFunc = null, TArg arg = default( TArg ) )
            where TV : class, new()
        {
            var dest = new TV();

            if( source == null ) return null;

            if( injection != null )
            {
                dest.InjectFrom( injection, source );
            }
            else
            {
                dest.InjectFrom( source );
            }

            if( itemFunc != null )
            {
                itemFunc( source, dest, arg );
            }

            return dest;
        }

        [Obsolete( "Use MapItem instead" )]
        public static TV Map<T, TV>( this T source, Action<T, TV> itemFunc = null )
            where TV : class, new()
        {
            return source.Map( null, itemFunc );
        }

        [Obsolete( "Use MapItem instead" )]
        public static TV Map<T, TV>( this T source, IValueInjection injection, Action<T, TV> itemFunc = null )
            where TV : class, new()
        {
            var dest = new TV();

            if( source == null ) return null;

            if( injection != null )
            {
                dest.InjectFrom( injection, source );
            }
            else
            {
                dest.InjectFrom( source );
            }

            if( itemFunc != null )
            {
                itemFunc( source, dest );
            }

            return dest;
        }

    }
}
