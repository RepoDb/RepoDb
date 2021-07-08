using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using RepoDb.DataProviderFunctions.DataProviderFunctionBuilders;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="QueryField"/> object.
    /// </summary>
    public static class QueryFieldExtension
    {
        /// <summary>
        /// Converts an instance of a query field into an enumerable list of query fields.
        /// </summary>
        /// <param name="queryField">The query field to be converted.</param>
        /// <returns>An enumerable list of query fields.</returns>
        public static IEnumerable<QueryField> AsEnumerable(this QueryField queryField)
        {
            yield return queryField;
        }

        /// <summary>
        /// Resets all the instances of <see cref="QueryField"/>.
        /// </summary>
        /// <param name="queryFields">The list of <see cref="QueryField"/> objects.</param>
        public static void ResetAll(this IEnumerable<QueryField> queryFields)
        {
            foreach (var queryField in queryFields)
            {
                queryField.Reset();
            }
        }

        // AsField
        internal static string AsField(this QueryField queryField,
            IDbSetting dbSetting)
        {
            var result = queryField.Field.Name.AsField(dbSetting);
            ApplyServerFunctionsIfNeeded(queryField, dbSetting, ref result);
            return result;
        }

        // AsParameter
        internal static string AsParameter(this QueryField queryField,
            int index,
            IDbSetting dbSetting)
        {
            return queryField.Parameter.Name.AsParameter(index, dbSetting);
        }

        // AsParameterAsField
        internal static string AsParameterAsField(this QueryField queryField,
            int index,
            IDbSetting dbSetting)
        {
            return string.Concat(queryField.AsParameter(index, dbSetting), " AS ", queryField.AsField(dbSetting));
        }

        // AsBetweenParameter
        internal static string AsBetweenParameter(this QueryField queryField,
            int index,
            IDbSetting dbSetting)
        {
            return string.Concat(queryField.Parameter.Name.AsParameter(index, dbSetting), "_Left AND ", queryField.Parameter.Name.AsParameter(index, dbSetting), "_Right");
        }

        // AsInParameter
        internal static string AsInParameter(this QueryField queryField,
            int index,
            IDbSetting dbSetting)
        {
            var array = ((Array)queryField.Parameter.Value);
            var values = array
                .OfType<object>()
                .Select((value, valueIndex) =>
                    string.Concat(queryField.Parameter.Name.AsParameter(index, dbSetting), "_In_", valueIndex))
                .Join(", ");
            return string.Concat("(", values, ")");
        }

        // AsFieldAndParameter
        internal static string AsFieldAndParameter(this QueryField queryField,
            int index,
            IDbSetting dbSetting)
        {
            if (queryField.Operation == Operation.Equal && queryField.Parameter.Value == null)
            {
                return string.Concat(queryField.AsField(dbSetting), " IS NULL");
            }
            else if (queryField.Operation == Operation.NotEqual && queryField.Parameter.Value == null)
            {
                return string.Concat(queryField.AsField(dbSetting), " IS NOT NULL");
            }
            else
            {
                if (queryField.Operation == Operation.Between || queryField.Operation == Operation.NotBetween)
                {
                    return string.Concat(queryField.AsField(dbSetting), " ", queryField.GetOperationText(), " ", queryField.AsBetweenParameter(index, dbSetting));
                }
                else if (queryField.Operation == Operation.In || queryField.Operation == Operation.NotIn)
                {
                    return string.Concat(queryField.AsField(dbSetting), " ", queryField.GetOperationText(), " ", queryField.AsInParameter(index, dbSetting));
                }
                else
                {
                    return string.Concat(queryField.AsField(dbSetting), " ", queryField.GetOperationText(), " ", queryField.AsParameter(index, dbSetting));
                }
            }
        }

        // AsDbParameter
        internal static IDbDataParameter AsDbParameter(this QueryField queryField,
            IDbCommand command)
        {
            return AsDbParameter(queryField, command.CreateParameter());
        }

        internal static IDbDataParameter AsDbParameter(this QueryField queryField,
            IDbDataParameter parameter)
        {
            parameter.ParameterName = queryField.Field.Name;
            parameter.Value = queryField.Parameter.Value ?? DBNull.Value;
            return parameter;
        }

        // AsFieldsAndParameters
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<QueryField> queryFields,
            IDbSetting dbSetting)
        {
            return queryFields.Select(field => field.AsFieldAndParameter(0, dbSetting));
        }

        // AsObject
        internal static object AsObject(this IEnumerable<QueryField> queryFields)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            foreach (var queryField in queryFields)
            {
                expandoObject.Add(queryField.Field.Name, queryField.Parameter.Value);
            }
            return expandoObject;
        }

        private static void ApplyServerFunctionsIfNeeded(QueryField queryField, IDbSetting dbSetting, ref string fieldName) {
            // TODO: Inject provider-specific IDataProviderFunctionBuilder  
            IDataProviderFunctionBuilder dataProviderFunctionBuilder = new SqlServerFunctionBuilder(fieldName, queryField.Field.DataProviderFunctions);
            fieldName = dataProviderFunctionBuilder.Build();
        }
    }
}
