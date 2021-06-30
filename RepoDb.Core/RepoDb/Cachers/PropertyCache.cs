using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the properties of the data entity.
    /// </summary>
    public static class PropertyCache
    {
        private static readonly ConcurrentDictionary<int, IEnumerable<ClassProperty>> cache = new();

        #region Methods

        /// <summary>
        /// Gets the cached <see cref="ClassProperty"/> object of the data entity (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The instance of cached <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get<TEntity>(Expression<Func<TEntity, object>> expression)
            where TEntity : class =>
            Get(typeof(TEntity), ExpressionExtension.GetProperty(expression));

        /// <summary>
        /// Gets the cached <see cref="ClassProperty"/> object of the data entity (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The instance of cached <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get<TEntity>(string propertyName)
            where TEntity : class =>
            Get(typeof(TEntity), propertyName);

        /// <summary>
        /// Gets the cached <see cref="ClassProperty"/> object of the data entity (via property name).
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The instance of cached <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get(Type entityType, string propertyName)
        {
            // Validate the presence
            ThrowNullReferenceException(propertyName, "PropertyName");

            // Return the value
            return Get(entityType)?
                .FirstOrDefault(p =>
                    string.Equals(p.GetMappedName(), propertyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the cached <see cref="ClassProperty"/> object of the data entity (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The instance of the <see cref="Field"/> object.</param>
        /// <returns>The instance of cached <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get<TEntity>(Field field)
            where TEntity : class =>
            Get(typeof(TEntity), field);

        /// <summary>
        /// Gets the cached <see cref="ClassProperty"/> object of the data entity (via <see cref="Field"/> object).
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="field">The instance of the <see cref="Field"/> object.</param>
        /// <returns>The instance of cached <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get(Type entityType, Field field)
        {
            // Validate the presence
            ThrowNullReferenceException(field, "Field");

            // Return the value
            return Get(entityType)?
                .FirstOrDefault(p =>
                    string.Equals(p.GetMappedName(), field?.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the cached <see cref="ClassProperty"/> object of the data entity (via <see cref="PropertyInfo"/> object).
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of the <see cref="PropertyInfo"/> object.</param>
        /// <returns>The instance of cached <see cref="ClassProperty"/> object.</returns>
        internal static ClassProperty Get(Type entityType, PropertyInfo propertyInfo)
        {
            // Validate the presence
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Return the value
            return Get(entityType)?
                .FirstOrDefault(p => p.PropertyInfo == propertyInfo || 
                    string.Equals(p.GetMappedName(), PropertyMappedNameCache.Get(entityType, propertyInfo), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the cached list of <see cref="ClassProperty"/> objects of the data entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <returns>The cached list <see cref="ClassProperty"/> objects.</returns>
        public static IEnumerable<ClassProperty> Get<TEntity>()
            where TEntity : class =>
            Get(typeof(TEntity));

        /// <summary>
        /// Gets the cached list of <see cref="ClassProperty"/> objects of the data entity.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <returns>The cached list <see cref="ClassProperty"/> objects.</returns>
        public static IEnumerable<ClassProperty> Get(Type entityType)
        {
            if (entityType?.IsClassType() != true)
            {
                return null;
            }

            // Variables
            var key = GenerateHashCode(entityType);

            // Try get the value
            if (cache.TryGetValue(key, out var properties) == false)
            {
                properties = entityType.GetClassProperties().AsList();
                cache.TryAdd(key, properties);
            }

            // Return the value
            return properties;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached enumerable of <see cref="ClassProperty"/> objects.
        /// </summary>
        public static void Flush() =>
            cache.Clear();

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        private static int GenerateHashCode(Type type) =>
            TypeExtension.GenerateHashCode(type);

        /// <summary>
        /// Validates the target object presence.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        /// <param name="argument">The name of the argument.</param>
        private static void ThrowNullReferenceException<T>(T obj,
            string argument)
        {
            if (obj == null)
            {
                throw new NullReferenceException($"The argument '{argument}' cannot be null.");
            }
        }

        #endregion
    }
}
