using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Gets the corresponding <see cref="DbType"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <returns>The instance of the <see cref="DbType"/> object.</returns>
        public static DbType? GetDbType(this Type type) =>
            type != null ? TypeMapCache.Get(type.GetUnderlyingType()) : null;

        /// <summary>
        /// Returns the instance of <see cref="ConstructorInfo"/> with the most argument.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>The instance of <see cref="ConstructorInfo"/> with the most arguments.</returns>
        public static ConstructorInfo GetConstructorWithMostArguments(this Type type) =>
            type.GetConstructors().Where(item => item.GetParameters().Length > 0)
                .OrderByDescending(item => item.GetParameters().Length).FirstOrDefault();

        /// <summary>
        /// Checks whether the current type is of type <see cref="object"/>.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is a <see cref="object"/>.</returns>
        public static bool IsObjectType(this Type type) =>
            type == StaticType.Object;

        /// <summary>
        /// Checks whether the current type is a class.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is a class.</returns>
        public static bool IsClassType(this Type type) =>
            type.IsClass &&
            type.IsObjectType() != true &&
            StaticType.IEnumerable.IsAssignableFrom(type) != true;

        /// <summary>
        /// Checks whether the current type is an anonymous type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is an anonymous class.</returns>
        public static bool IsAnonymousType(this Type type) =>
            type.FullName.StartsWith("<>f__AnonymousType");

        /// <summary>
        /// Checks whether the current type is of type <see cref="IDictionary{TKey, TValue}"/> (with string/object key-value-pair).
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is of type <see cref="IDictionary{TKey, TValue}"/> (with string/object key-value-pair).</returns>
        public static bool IsDictionaryStringObject(this Type type) =>
            type == StaticType.IDictionaryStringObject ||
            type == StaticType.DictionaryStringObject || type == StaticType.ExpandoObject;

        /// <summary>
        /// Checks whether the current type is wrapped within a <see cref="Nullable{T}"/> object.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is wrapped within a <see cref="Nullable{T}"/> object.</returns>
        public static bool IsNullable(this Type type) =>
            Nullable.GetUnderlyingType(type) != null;

        /// <summary>
        /// Checks whether the current type is a plain class type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is a plain class type.</returns>
        internal static bool IsPlainType(this Type type) =>
            (IsClassType(type) || IsAnonymousType(type)) &&
            IsQueryObjectType(type) != true &&
            IsDictionaryStringObject(type) != true &&
            GetEnumerableClassProperties(type).Any() != true;

        /// <summary>
        /// Checks whether the current type is of type <see cref="QueryField"/> or <see cref="QueryGroup"/>.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>Returns true if the current type is of type <see cref="QueryField"/> or <see cref="QueryGroup"/>.</returns>
        internal static bool IsQueryObjectType(this Type type) =>
            type == StaticType.QueryField || type == StaticType.QueryGroup;

        /// <summary>
        /// Converts all properties of the type into an array of <see cref="Field"/> objects.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>A list of <see cref="Field"/> objects.</returns>
        internal static IEnumerable<Field> AsFields(this Type type) =>
            PropertyCache.Get(type).AsFields();

        /// <summary>
        /// Gets the list of enumerable <see cref="ClassProperty"/> objects of the type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>The list of the enumerable <see cref="ClassProperty"/> objects.</returns>
        internal static IEnumerable<ClassProperty> GetEnumerableClassProperties(this Type type) =>
            PropertyCache.Get(type).Where(classProperty =>
            {
                var propType = classProperty.PropertyInfo.PropertyType;
                return
                    propType != StaticType.String &&
                    propType != StaticType.CharArray &&
                    propType != StaticType.ByteArray &&
                    StaticType.IEnumerable.IsAssignableFrom(propType);
            });

        /// <summary>
        /// Converts all properties of the type into an array of <see cref="ClassProperty"/> objects.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>A list of <see cref="ClassProperty"/> objects.</returns>
        public static IEnumerable<ClassProperty> GetClassProperties(this Type type)
        {
            foreach (var property in type.GetProperties())
            {
                yield return new ClassProperty(type, property);
            }
        }

        /// <summary>
        /// Returns the underlying type of the current type. If there is no underlying type, this will return the current type.
        /// </summary>
        /// <param name="type">The current type to check.</param>
        /// <returns>The underlying type or the current type.</returns>
        public static Type GetUnderlyingType(this Type type) =>
            type != null ? (Nullable.GetUnderlyingType(type) ?? type) : null;

        /// <summary>
        /// Returns the property of the type based on the mappings equality.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <param name="mappedName">The name of the property mapping.</param>
        /// <returns>The instance of <see cref="ClassProperty"/>.</returns>
        internal static ClassProperty GetMappedProperty(this Type type, string mappedName) =>
            MappedPropertyCache.Get(type, mappedName);

        /// <summary>
        /// Returns the list of the interface types being implemented by the current type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <returns>The list of the interface types.</returns>
        public static Type[] GetImplementedInterfaces(this Type type) =>
            type?.GetType().GetProperty("ImplementedInterfaces")?.GetValue(type) as Type[];

        /// <summary>
        /// Creates a generic type of the current type based on the generic type available from the source type.
        /// </summary>
        /// <param name="currentType">The current type.</param>
        /// <param name="sourceType">The source type.</param>
        /// <returns>The newly created generic type.</returns>
        public static Type MakeGenericTypeFrom(this Type currentType,
            Type sourceType)
        {
            var genericTypes = sourceType?.GetGenericArguments();
            if (genericTypes?.Length == currentType?.GetGenericArguments().Length)
            {
                return currentType.MakeGenericType(genericTypes);
            }
            return null;
        }

        /// <summary>
        /// Checks whether the current type has implemented the target interface.
        /// </summary>
        /// <param name="currentType">The current type.</param>
        /// <param name="interfaceType">The target interface type.</param>
        /// <returns>True if the current type has implemented the target interface.</returns>
        public static bool IsInterfacedTo(this Type currentType,
            Type interfaceType)
        {
            var targetInterface = currentType?
                .GetImplementedInterfaces()?
                .FirstOrDefault(item =>
                    item.Name == interfaceType.Name && item.Namespace == interfaceType.Namespace);
            interfaceType = interfaceType?.MakeGenericTypeFrom(targetInterface);
            return interfaceType?.IsAssignableFrom(currentType) == true;
        }

        /// <summary>
        /// Checks whether the current class handler type is valid to be used for the target model type.
        /// </summary>
        /// <param name="classHandlerType">The current class handler type type.</param>
        /// <param name="targetModelType">The target model type.</param>
        /// <returns>True if the current class handler type is valid to be used for the target model type.</returns>
        internal static bool IsClassHandlerValidForModel(this Type classHandlerType,
            Type targetModelType)
        {
            var targetInterface = classHandlerType?
                .GetImplementedInterfaces()?
                .FirstOrDefault(item =>
                    item.Name == StaticType.IClassHandler.Name && item.Namespace == StaticType.IClassHandler.Namespace);
            if (targetInterface != null)
            {
                return targetInterface.GetGenericArguments().FirstOrDefault() == targetModelType;
            }
            return false;
        }

        #region Helpers

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="type">The type of the data entity.</param>
        /// <returns>The generated hashcode.</returns>
        internal static int GenerateHashCode(Type type) =>
            type.GetUnderlyingType().GetHashCode();

        /// <summary>
        /// Generates a hashcode for caching.
        /// </summary>
        /// <param name="entityType">The type of the data entity.</param>
        /// <param name="propertyInfo">The instance of <see cref="PropertyInfo"/>.</param>
        /// <returns>The generated hashcode.</returns>
        internal static int GenerateHashCode(Type entityType,
            PropertyInfo propertyInfo) =>
            entityType.GetUnderlyingType().GetHashCode() + propertyInfo.GenerateCustomizedHashCode(entityType);

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on name.
        /// </summary>
        /// <typeparam name="T">The target .NET CLR type.</typeparam>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        internal static PropertyInfo GetProperty<T>(string propertyName)
            where T : class =>
            GetProperty(typeof(T), propertyName);

        /// <summary>
        /// A helper method to return the instance of <see cref="PropertyInfo"/> object based on name.
        /// </summary>
        /// <param name="type">The target .NET CLR type.</param>
        /// <param name="propertyName">The name of the class property to be mapped.</param>
        /// <returns>An instance of <see cref="PropertyInfo"/> object.</returns>
        internal static PropertyInfo GetProperty(Type type,
            string propertyName) =>

            type.GetProperties().FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

        #endregion
    }
}
