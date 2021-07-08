using RepoDb.DataProviderFunctions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb.DataProviderFunctions.DataProviderFunctionBuilders {

    /// <summary>
    /// Base class for building data provider functions
    /// </summary>
    public abstract class BaseDataProviderFunctionBuilder : IDataProviderFunctionBuilder {

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<DataProviderFunction> DataProviderFunctions { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string FieldName { get; protected set; }

        /// <summary>
        /// Contains a dictionary of supported data provider functions:
        /// key = function name
        /// value = Decorator delegate that will transform input field.
        /// </summary>
        protected Dictionary<string, Func<string, IEnumerable<Expression>, string>> Vocabulary = new Dictionary<string, Func<string, IEnumerable<Expression>, string>>();

        /// <summary>
        /// Initializes Vocabulary
        /// </summary>
        protected BaseDataProviderFunctionBuilder() {
            Vocabulary.Add("ToUpper", (fieldName, exprArgs) => string.Format("UPPER({0})", fieldName));
            Vocabulary.Add("ToLower", (fieldName, exprArgs) => string.Format("LOWER({0})", fieldName));
            /* Just a proof of concept for accommodating parameterized server-side functions 
            Vocabulary.Add("Substring", (fieldName, exprArgs) => string.Format("SUBSTRING({0}, {1},{2})", fieldName, 
                                                                                    ((ConstantExpression)exprArgs[0]).Value,
                                                                                    ((ConstantExpression)exprArgs[1]).Value);
            */
        }

        /// <summary>
        /// Convenience ctor
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="dataProviderFunctions"></param>
        public BaseDataProviderFunctionBuilder(string fieldName,
            IEnumerable<DataProviderFunction> dataProviderFunctions) : this() {
            FieldName = fieldName;
            DataProviderFunctions = dataProviderFunctions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Build() {
            if (DataProviderFunctions?.Any() == false) {
                return FieldName;
            }
            string fieldName = FieldName;
            foreach (var dataProviderFunc in DataProviderFunctions) {
                VerifyFunctionSupport(dataProviderFunc);
                Decorate(ref fieldName, dataProviderFunc);
            }
            return fieldName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public virtual bool IsDataProviderFunction(string functionName) {
            return Vocabulary.ContainsKey(functionName);
        }

        #region privates
        private void VerifyFunctionSupport(DataProviderFunction dataProviderFunc) {
            if (!IsDataProviderFunction(dataProviderFunc.Name)) {
                // We shouldn't be here if IsDataProviderFunction was also used during extraction of functions.
                throw new NotSupportedFunctionException(dataProviderFunc.Name);
            }
        }

        private void Decorate(ref string fieldName, DataProviderFunction dataProviderFunc) {
            try {
                fieldName = Vocabulary[dataProviderFunc.Name](fieldName, dataProviderFunc.Arguments);
            }
            catch (Exception exc) {
                throw new DataProviderFunctionDecoratorException(fieldName, dataProviderFunc.Name, exc);
            }
        }
        #endregion
    }    
}
