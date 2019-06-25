﻿using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the update operation arguments.
    /// </summary>
    internal class UpdateRequest : BaseRequest, IEquatable<UpdateRequest>
    {
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="UpdateRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public UpdateRequest(Type type,
            IDbConnection connection,
            QueryGroup where = null,
            IEnumerable<Field> fields = null,
            IStatementBuilder statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                  connection,
                  where,
                  fields,
                  statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="UpdateRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public UpdateRequest(string name,
            IDbConnection connection,
            QueryGroup where = null,
            IEnumerable<Field> fields = null,
            IStatementBuilder statementBuilder = null)
            : base(name,
                  connection,
                  statementBuilder)
        {
            Where = where;
            Fields = fields;
        }

        /// <summary>
        /// Gets the query expression used.
        /// </summary>
        public QueryGroup Where { get; }

        /// <summary>
        /// Gets the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="UpdateRequest"/>.
        /// </summary>
        /// <returns>The hashcode value.</returns>
        public override int GetHashCode()
        {
            // Make sure to return if it is already provided
            if (!ReferenceEquals(null, m_hashCode))
            {
                return m_hashCode.Value;
            }

            // Get first the entity hash code
            var hashCode = TypeNameHashCode;
            hashCode += ".Update".GetHashCode();

            // Get the properties hash codes
            if (Where != null)
            {
                hashCode += Where.GetHashCode();
            }

            // Get the qualifier <see cref="Field"/> objects
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    hashCode += field.GetHashCode();
                }
            }

            // Set back the hash code value
            m_hashCode = hashCode;

            // Return the actual value
            return hashCode;
        }

        /// <summary>
        /// Compares the <see cref="UpdateRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="UpdateRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(UpdateRequest other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="UpdateRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="UpdateRequest"/> object.</param>
        /// <param name="objB">The second <see cref="UpdateRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(UpdateRequest objA, UpdateRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="UpdateRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="UpdateRequest"/> object.</param>
        /// <param name="objB">The second <see cref="UpdateRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(UpdateRequest objA, UpdateRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
