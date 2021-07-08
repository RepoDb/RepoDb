using System;

namespace RepoDb.DataProviderFunctions.Exceptions {
    /// <summary>
    /// 
    /// </summary>
    public class NotSupportedFunctionException : NotSupportedException {
        /// <summary>
        /// 
        /// </summary>
        public readonly string DataProviderFunctionName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataProviderFunctionName"></param>
        public NotSupportedFunctionException(string dataProviderFunctionName) : base(string.Format("Function {0} not supported by DataProvider",
                                                                                                    dataProviderFunctionName)) {
            DataProviderFunctionName = dataProviderFunctionName;
        }
    }
}
