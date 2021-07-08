using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.DataProviderFunctions;
using RepoDb.DataProviderFunctions.DataProviderFunctionBuilders;
using System;
using System.Linq.Expressions;
using RepoDb.Extensions;

namespace RepoDb.UnitTests.DataProviderFunctions {
    [TestClass]
    public class DataProviderFunctionExtratorTest {
        [TestMethod]
        public void TestSingleCall() {
            // arrange
            var mockDataProviderFunctionBuilder = new Mock<IDataProviderFunctionBuilder>();
            mockDataProviderFunctionBuilder.Setup(b => b.IsDataProviderFunction("ToUpper")).Returns(true);

            Expression<Func<string, string>> inputExpr = s => "".ToUpper();
            
            // act
            var dataProviderFunctionExtractor = new DataProviderFunctionExtractor(mockDataProviderFunctionBuilder.Object);
            dataProviderFunctionExtractor.Extract((MethodCallExpression)inputExpr.Body);

            // assert
            Assert.AreEqual(1, dataProviderFunctionExtractor.ExtractedDataProviderFunctions.Count);
        }

        [TestMethod]
        public void TestChainedCallAllProviderFunctions() {
            // arrange
            var mockDataProviderFunctionBuilder = new Mock<IDataProviderFunctionBuilder>();
            mockDataProviderFunctionBuilder.Setup(b => b.IsDataProviderFunction("ToUpper")).Returns(true);
            mockDataProviderFunctionBuilder.Setup(b => b.IsDataProviderFunction("ToLower")).Returns(true);

            Expression<Func<string, string>> inputExpr = s => "".ToUpper().ToLower();

            // act
            var dataProviderFunctionExtractor = new DataProviderFunctionExtractor(mockDataProviderFunctionBuilder.Object);
            dataProviderFunctionExtractor.Extract((MethodCallExpression)inputExpr.Body);

            // assert
            Assert.AreEqual(2, dataProviderFunctionExtractor.ExtractedDataProviderFunctions.Count);
        }

    
        [TestMethod]
        public void TestChainedCallNotAllProviderFunctions() {
            // arrange
            var mockDataProviderFunctionBuilder = new Mock<IDataProviderFunctionBuilder>();
            mockDataProviderFunctionBuilder.Setup(b => b.IsDataProviderFunction("ToUpper")).Returns(true);
            mockDataProviderFunctionBuilder.Setup(b => b.IsDataProviderFunction("ToLower")).Returns(true);
            
            Expression<Func<string, string>> inputExpr = s => "".ToUpper().Substring(0).ToLower();
            // act
            var dataProviderFunctionExtractor = new DataProviderFunctionExtractor(mockDataProviderFunctionBuilder.Object);
            dataProviderFunctionExtractor.Extract((MethodCallExpression)inputExpr.Body);

            // assert
            Assert.AreEqual(2, dataProviderFunctionExtractor.ExtractedDataProviderFunctions.Count);
        }


        [TestMethod]
        public void TestMemberExpression() {
            // arrange
            var mockDataProviderFunctionBuilder = new Mock<IDataProviderFunctionBuilder>();
            mockDataProviderFunctionBuilder.Setup(b => b.IsDataProviderFunction("ToUpper")).Returns(true);
            mockDataProviderFunctionBuilder.Setup(b => b.IsDataProviderFunction("ToLower")).Returns(true);

            var testClass = new DataProviderFunctionExtractorTestClass();
            Expression<Func<string, string>> inputExpr = s => testClass.FirstName.ToUpper().Substring(0).ToLower();

            // act
            var dataProviderFunctionExtractor = new DataProviderFunctionExtractor(mockDataProviderFunctionBuilder.Object);
            dataProviderFunctionExtractor.Extract((MethodCallExpression)inputExpr.Body);

            // assert
            Assert.AreEqual(2, dataProviderFunctionExtractor.ExtractedDataProviderFunctions.Count);
            Assert.IsNotNull(dataProviderFunctionExtractor.MemberExpression);
            Assert.AreEqual("FirstName", dataProviderFunctionExtractor.MemberExpression.ToMember().Member.Name) ;
        }

        [TestMethod]
        public void TestMemberExpressionEmbeddedType() {
            // arrange
            var mockDataProviderFunctionBuilder = new Mock<IDataProviderFunctionBuilder>();
            mockDataProviderFunctionBuilder.Setup(b => b.IsDataProviderFunction("ToUpper")).Returns(true);
            mockDataProviderFunctionBuilder.Setup(b => b.IsDataProviderFunction("ToLower")).Returns(true);

            var testClass = new DataProviderFunctionExtractorEmbeddedTestClass();
            Expression<Func<string, string>> inputExpr = s => testClass.EmbeddedStringType.FirstName.ToUpper().Substring(0).ToLower();

            // act
            var dataProviderFunctionExtractor = new DataProviderFunctionExtractor(mockDataProviderFunctionBuilder.Object);
            dataProviderFunctionExtractor.Extract((MethodCallExpression)inputExpr.Body);

            // assert
            Assert.AreEqual(2, dataProviderFunctionExtractor.ExtractedDataProviderFunctions.Count);
            Assert.IsNotNull(dataProviderFunctionExtractor.MemberExpression);
            Assert.AreEqual("FirstName", dataProviderFunctionExtractor.MemberExpression.ToMember().Member.Name) ;
        }
    }

    internal class DataProviderFunctionExtractorTestClass { 
        public string FirstName { get; set; }
    }

    internal class DataProviderFunctionExtractorEmbeddedTestClass {
        public DataProviderFunctionExtractorTestClass EmbeddedStringType { get; set; }
    }
}
