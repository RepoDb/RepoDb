﻿namespace RepoDb.DataProviderFunctions.DataProviderFunctionBuilders {
    /// <summary>
    /// Builder interface for data provider functions
    /// </summary>
    public interface IDataProviderFunctionBuilder {
        /// <summary>
        /// Determines whether a given function name is a supported data provider function.
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        bool IsDataProviderFunction(string functionName);

        /// <summary>
        /// Builds data provider functions by applying decorators to a field
        /// </summary>
        /// <returns></returns>
        string Build();
    }
}