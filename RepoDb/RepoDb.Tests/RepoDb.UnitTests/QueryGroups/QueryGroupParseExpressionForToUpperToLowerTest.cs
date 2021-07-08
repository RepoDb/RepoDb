using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests {
    public partial class QueryGroupTest
    {
        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsWithToUpperFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(c => c.PropertyString.ToUpper().Contains("A"));
            
            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(UPPER([PropertyString]) LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsWithToLowerFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(c => c.PropertyString.ToLower().Contains("A"));
            
            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(LOWER([PropertyString]) LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsWithToLowerToUpperFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(c => c.PropertyString.ToLower().ToUpper().Contains("A"));
            
            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(UPPER(LOWER([PropertyString])) LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringToUpperFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(c => c.PropertyString.ToUpper() == "A");
            
            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(UPPER([PropertyString]) = @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringToLowerFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(c => c.PropertyString.ToLower() == "A");
            
            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(LOWER([PropertyString]) = @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringToLowerToUpperFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(c => c.PropertyString.ToLower().ToUpper() != "A");
            
            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(UPPER(LOWER([PropertyString])) <> @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
