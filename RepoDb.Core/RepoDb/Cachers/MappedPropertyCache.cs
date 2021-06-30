using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the mapped properties of the data entity.
    /// </summary>
    public static class MappedPropertyCache
    {
        private static readonly ConcurrentDictionary<int, Dictionary<string, ClassProperty>> mappedCache = new();

        #region Methods

        /// <summary>
        /// Gets the cached <see cref="ClassProperty"/> object of the data entity (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The mapped name of the property.</param>
        /// <returns>The instance of cached <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get<TEntity>(string propertyName)
            where TEntity : class =>
            Get(typeof(TEntity), propertyName);

        /// <summary>
        /// Gets the cached <see cref="ClassProperty"/> object of the data entity (via property name).
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyName">The mapped name of the property.</param>
        /// <returns>The instance of cached <see cref="ClassProperty"/> object.</returns>
        public static ClassProperty Get(Type entityType, string propertyName)
        {
            // Validate the presence
            ThrowNullReferenceException(propertyName, "PropertyName");
            
            ClassProperty classProperty = null;
            Get(entityType)?.TryGetValue(propertyName, out classProperty);
            
            // Return the value
            return classProperty;
        }

        private static Dictionary<string, ClassProperty> Get(Type entityType)
        {
            if (entityType?.IsClassType() != true)
            {
                return null;
            }

            // Variables
            var key = GenerateHashCode(entityType);

            // Try get the value
            if (mappedCache.TryGetValue(key, out var properties) == false)
            {
                properties = PropertyCache.Get(entityType)
                    .ToDictionary(p => p.GetMappedName(), p => p, StringComparer.OrdinalIgnoreCase);
                
                mappedCache.TryAdd(key, properties);
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
            mappedCache.Clear();

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
