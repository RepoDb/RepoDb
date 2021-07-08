﻿using System.Collections.Generic;
using System.Linq.Expressions;

namespace RepoDb.DataProviderFunctions {
    /// <summary>
    /// An object that represents a data provider-level function that is intended 
    /// to be applied to the column/field itself during statement-generation.
    /// </summary>
    public class DataProviderFunction {

        /// <summary>
        /// Name of server-side SQL function
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Optional input arguments for the data provider function
        /// </summary>
        public readonly IEnumerable<Expression> Arguments;

        /// <summary>
        /// Creates a new instance of <see cref="DataProviderFunction"/> object.
        /// </summary>
        public DataProviderFunction(string functionName, IEnumerable<Expression> arguments) : this(functionName) {
            Arguments = arguments;
        }


        /// <summary>
        /// Creates a new instance of <see cref="DataProviderFunction"/> object.
        /// </summary>
        public DataProviderFunction(string functionName) {
            Name = functionName;
        }
    }

}