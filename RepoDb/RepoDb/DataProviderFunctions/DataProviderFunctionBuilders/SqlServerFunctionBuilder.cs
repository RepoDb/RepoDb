﻿using System.Collections.Generic;

namespace RepoDb.DataProviderFunctions.DataProviderFunctionBuilders {

    /// <summary>
    /// 
    /// </summary>
    public class SqlServerFunctionBuilder : BaseDataProviderFunctionBuilder {
        
        /// <summary>
        /// 
        /// </summary>
        public SqlServerFunctionBuilder() : base() {
            // Extend support for provider-specific capabilities/syntax by adding/updating entries in FunctionBuilders
            // Vocabulary.Add(".NET function here...", <data provider function translation here>);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="dataProviderFunctions"></param>
        public SqlServerFunctionBuilder(string fieldName, 
            IEnumerable<DataProviderFunction> dataProviderFunctions) : base(fieldName, dataProviderFunctions)
        {
        }
    }

    
    /*
     * 
    /// <summary>
    /// POC for a project-specific function builder that can include a .NET extension method + server UDF pair
    /// *** BUT can be a surface vector for SQL injection attacks ***
    /// Otherwise, chosen DataProviderFunctionBuilder assembly can be loaded from configuration via Reflection
    /// so that RepoDb clients can plug in their own function builder if desired.
    /// /// </summary>
    public class MyProjectSpecificFunctionBuilder : SqlServerFunctionBuilder {
        
        /// <summary>
        /// 
        /// </summary>
        public MyProjectSpecificFunctionBuilder() : base() {
            Vocabulary.Add("ApplyServerUdf", (fieldName, exprArgs) => string.Format("dbo.fn_MyUdf({0})", fieldName));
        }
    }

    */
}