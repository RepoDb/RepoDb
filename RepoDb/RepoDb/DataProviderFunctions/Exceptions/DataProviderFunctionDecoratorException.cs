using System;

namespace RepoDb.DataProviderFunctions.Exceptions {

    /// <summary>
    /// 
    /// </summary>
    public class DataProviderFunctionDecoratorException : ApplicationException {
        /// <summary>
        /// 
        /// </summary>
        public readonly string FieldName;
        /// <summary>
        /// 
        /// </summary>
        public readonly string DataProviderFunctionName;
        /// <summary>
        /// 
        /// </summary>
        public readonly Exception Exception;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="dataProviderFunctionName"></param>
        /// <param name="exc"></param>
        public DataProviderFunctionDecoratorException(string fieldName, string dataProviderFunctionName, Exception exc) :
                base(string.Format("Decorator for DataProviderFunction {0} threw an exception while being applied to field {1}: {2}",
                                                    dataProviderFunctionName, fieldName, exc.Message)) {
            FieldName = fieldName;
            DataProviderFunctionName = dataProviderFunctionName;
            Exception = exc;
        }
    }
}
