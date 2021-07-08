﻿using RepoDb.DataProviderFunctions.DataProviderFunctionBuilders;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RepoDb.DataProviderFunctions {
    /// <summary>
    /// Visitor class that traverses a MethodCall expression tree and extracts functions from the call-chain
    /// that are classified as data-provider functions based on a configurable Vocabulary. 
    /// The extracted functions will then be translated to data-provider syntax during SQL-generation
    /// but the translation itself is not part of this type's responsibility.
    /// </summary>
    public class DataProviderFunctionExtractor : ExpressionVisitor {

        private Stack<DataProviderFunction> m_dataProviderFunctions = new Stack<DataProviderFunction>();

        /// <summary>
        /// Given Extract({x.Customer.ToLower().Contains('a')}), MemberExpression should return { x.Customer }
        /// </summary>
        public Expression MemberExpression { get; protected set; }

        /// <summary>
        /// Helper interface for classifying data provider-supported functions
        /// </summary>
        public IDataProviderFunctionBuilder DataProviderFunctionBuilder { get; protected set; }
        
        /// <summary>
        /// Output for extracted data provider functions from input MethodCallExpression
        /// </summary>
        public IList<DataProviderFunction> ExtractedDataProviderFunctions;


        /// <summary>
        /// 
        /// </summary>
        public DataProviderFunctionExtractor(IDataProviderFunctionBuilder dataProviderFunctionBuilder) {
            ExtractedDataProviderFunctions = new List<DataProviderFunction>();
            DataProviderFunctionBuilder = dataProviderFunctionBuilder;
        }

        /// <summary>
        /// Entry point
        /// </summary>
        /// <returns></returns>
        public void Extract(MethodCallExpression node) {
            ExtractedDataProviderFunctions.Clear();
            
            if (node.Object is MemberExpression) {
                MemberExpression = node.Object;
            }
            
            Visit(node);

            while (m_dataProviderFunctions.Count > 0) {
                ExtractedDataProviderFunctions.Add(m_dataProviderFunctions.Pop());
            }
        }

        /// <summary>
        /// Extract method calls (and any arguments) that are classified as data provider functions
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node) {
            if (DataProviderFunctionBuilder.IsDataProviderFunction(node.Method.Name)) {
                m_dataProviderFunctions.Push(new DataProviderFunction(node.Method.Name, node.Arguments));
            }
            return base.VisitMethodCall(node);
        }


        /// <summary>
        /// See MemberExpression comments
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node) {
            if (MemberExpression == null) {
                MemberExpression = node;
            }
            return base.VisitMember(node);
        }
    }
}