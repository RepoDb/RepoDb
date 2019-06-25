﻿using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.Requests
{
    /// <summary>
    /// A class that holds the value of the query operation arguments.
    /// </summary>
    internal class QueryRequest : BaseRequest, IEquatable<QueryRequest>
    {
        private int? m_hashCode = null;

        /// <summary>
        /// Creates a new instance of <see cref="QueryRequest"/> object.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="orderBy">The list of order fields.</param>
        /// <param name="top">The filter for the rows.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public QueryRequest(Type type,
            IDbConnection connection,
            IEnumerable<Field> fields = null,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : this(ClassMappedNameCache.Get(type),
                  connection,
                  fields,
                  where,
                  orderBy,
                  top,
                  hints,
                  statementBuilder)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of <see cref="QueryRequest"/> object.
        /// </summary>
        /// <param name="name">The name of the request.</param>
        /// <param name="connection">The connection object.</param>
        /// <param name="fields">The list of the target fields.</param>
        /// <param name="where">The query expression.</param>
        /// <param name="orderBy">The list of order fields.</param>
        /// <param name="top">The filter for the rows.</param>
        /// <param name="hints">The hints for the table.</param>
        /// <param name="statementBuilder">The statement builder.</param>
        public QueryRequest(string name,
            IDbConnection connection,
            IEnumerable<Field> fields = null,
            QueryGroup where = null,
            IEnumerable<OrderField> orderBy = null,
            int? top = null,
            string hints = null,
            IStatementBuilder statementBuilder = null)
            : base(name,
                  connection,
                  statementBuilder)
        {
            Fields = fields;
            Where = where;
            OrderBy = orderBy;
            Top = top;
            Hints = hints;
        }

        /// <summary>
        /// Gets the list of the target fields.
        /// </summary>
        public IEnumerable<Field> Fields { get; set; }

        /// <summary>
        /// Gets the query expression used.
        /// </summary>
        public QueryGroup Where { get; }

        /// <summary>
        /// Gets the list of the order fields.
        /// </summary>
        public IEnumerable<OrderField> OrderBy { get; }

        /// <summary>
        /// Gets the filter for the rows.
        /// </summary>
        public int? Top { get; }

        /// <summary>
        /// Gets the hints for the table.
        /// </summary>
        public string Hints { get; }

        // Equality and comparers

        /// <summary>
        /// Returns the hashcode for this <see cref="QueryRequest"/>.
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
            hashCode += ".Query".GetHashCode();

            // Get the qualifier <see cref="Field"/> objects
            if (Fields != null)
            {
                foreach (var field in Fields)
                {
                    hashCode += field.GetHashCode();
                }
            }

            // Add the expression
            if (!ReferenceEquals(null, Where))
            {
                hashCode += Where.GetHashCode();
            }

            // Add the order fields
            if (!ReferenceEquals(null, OrderBy))
            {
                foreach (var orderField in OrderBy)
                {
                    hashCode += orderField.GetHashCode();
                }
            }

            // Add the filter
            if (!ReferenceEquals(null, Top))
            {
                hashCode += Top.GetHashCode();
            }

            // Add the hints
            if (!ReferenceEquals(null, Hints))
            {
                hashCode += Hints.GetHashCode();
            }

            // Set back the hash code value
            m_hashCode = hashCode;

            // Return the actual value
            return hashCode;
        }

        /// <summary>
        /// Compares the <see cref="QueryRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="obj">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equals.</returns>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the <see cref="QueryRequest"/> object equality against the given target object.
        /// </summary>
        /// <param name="other">The object to be compared to the current object.</param>
        /// <returns>True if the instances are equal.</returns>
        public bool Equals(QueryRequest other)
        {
            return other?.GetHashCode() == GetHashCode();
        }

        /// <summary>
        /// Compares the equality of the two <see cref="QueryRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryRequest"/> object.</param>
        /// <param name="objB">The second <see cref="QueryRequest"/> object.</param>
        /// <returns>True if the instances are equal.</returns>
        public static bool operator ==(QueryRequest objA, QueryRequest objB)
        {
            if (ReferenceEquals(null, objA))
            {
                return ReferenceEquals(null, objB);
            }
            return objB?.GetHashCode() == objA.GetHashCode();
        }

        /// <summary>
        /// Compares the inequality of the two <see cref="QueryRequest"/> objects.
        /// </summary>
        /// <param name="objA">The first <see cref="QueryRequest"/> object.</param>
        /// <param name="objB">The second <see cref="QueryRequest"/> object.</param>
        /// <returns>True if the instances are not equal.</returns>
        public static bool operator !=(QueryRequest objA, QueryRequest objB)
        {
            return (objA == objB) == false;
        }
    }
}
