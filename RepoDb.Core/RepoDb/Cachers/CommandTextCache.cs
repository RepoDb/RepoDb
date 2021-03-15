using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Requests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to cache the already-built command texts.
    /// </summary>
    public static class CommandTextCache
    {
        private static readonly ConcurrentDictionary<BaseRequest, string> cache = new ConcurrentDictionary<BaseRequest, string>();

        #region GetAverageText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetAverageText(AverageRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateAverage(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetAverageAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetAverageAllText(AverageAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateAverageAll(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetBatchQueryText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetBatchQueryText(BatchQueryRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetActualFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                commandText = GetBatchQueryTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetBatchQueryTextAsync(BatchQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetActualFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                commandText = GetBatchQueryTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        internal static string GetBatchQueryTextInternal(BatchQueryRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateBatchQuery(new QueryBuilder(),
                request.Name,
                fields,
                request.Page,
                request.RowsPerBatch,
                request.OrderBy,
                request.Where,
                request.Hints);
        }

        #endregion

        #region GetCountText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetCountText(CountRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateCount(new QueryBuilder(),
                    request.Name,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetCountAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetCountAllText(CountAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateCountAll(new QueryBuilder(),
                    request.Name,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetDeleteText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetDeleteText(DeleteRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateDelete(new QueryBuilder(),
                    request.Name,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetDeleteAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetDeleteAllText(DeleteAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateDeleteAll(new QueryBuilder(),
                    request.Name,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetExistsText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetExistsText(ExistsRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateExists(new QueryBuilder(),
                    request.Name,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetInsertText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetInsertText(InsertRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetActualFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                commandText = GetInsertTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetInsertTextAsync(InsertRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetActualFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                commandText = GetInsertTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetInsertTextInternal(InsertRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            var primaryField = GetPrimaryField(request);
            var identityField = GetIdentityField(request);
            return statementBuilder.CreateInsert(new QueryBuilder(),
                request.Name,
                fields,
                primaryField,
                identityField,
                request.Hints);
        }

        #endregion

        #region GetInsertAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetInsertAllText(InsertAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetActualFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                commandText = GetInsertAllTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetInsertAllTextAsync(InsertAllRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetActualFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                commandText = GetInsertAllTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetInsertAllTextInternal(InsertAllRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            var primaryField = GetPrimaryField(request);
            var identityField = GetIdentityField(request);
            return statementBuilder.CreateInsertAll(new QueryBuilder(),
                request.Name,
                fields,
                request.BatchSize,
                primaryField,
                identityField,
                request.Hints);
        }

        #endregion

        #region GetMaxText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetMaxText(MaxRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMax(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetMaxAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetMaxAllText(MaxAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMaxAll(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetMergeText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetMergeText(MergeRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetActualFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                commandText = GetMergeTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetMergeTextAsync(MergeRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetActualFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                commandText = GetMergeTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetMergeTextInternal(MergeRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            var primaryField = GetPrimaryField(request);
            var identityField = GetIdentityField(request);
            return statementBuilder.CreateMerge(new QueryBuilder(),
                request.Name,
                fields,
                request.Qualifiers,
                primaryField,
                identityField,
                request.Hints);
        }

        #endregion

        #region GetMergeAllText

        /// <summary>
        /// Gets the cached command text for the 'MergeAll' operation.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>The cached command text.</returns>
        internal static string GetMergeAllText(MergeAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetActualFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                commandText = GetMergeAllTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetMergeAllTextAsync(MergeAllRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetActualFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                commandText = GetMergeAllTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetMergeAllTextInternal(MergeAllRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            var primaryField = GetPrimaryField(request);
            var identityField = GetIdentityField(request);
            return statementBuilder.CreateMergeAll(new QueryBuilder(),
                request.Name,
                fields,
                request.Qualifiers,
                request.BatchSize,
                primaryField,
                identityField,
                request.Hints);
        }

        #endregion

        #region GetMinText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetMinText(MinRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMin(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetMinAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetMinAllText(MinAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateMinAll(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetQueryText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetQueryText(QueryRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetActualFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                commandText = GetQueryTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetQueryTextAsync(QueryRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetActualFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                commandText = GetQueryTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetQueryTextInternal(QueryRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateQuery(new QueryBuilder(),
                request.Name,
                fields,
                request.Where,
                request.OrderBy,
                request.Top,
                request.Hints);
        }

        #endregion

        #region GetQueryAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetQueryAllText(QueryAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetActualFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                commandText = GetQueryAllTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetQueryAllTextAsync(QueryAllRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetActualFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                commandText = GetQueryAllTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetQueryAllTextInternal(QueryAllRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateQueryAll(new QueryBuilder(),
                request.Name,
                fields,
                request.OrderBy,
                request.Hints);
        }

        #endregion

        #region GetQueryMultipleText

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetQueryMultipleText<TEntity>(QueryMultipleRequest request)
            where TEntity : class
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetActualFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                commandText = GetQueryMultipleTextInternal<TEntity>(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetQueryMultipleTextAsync<TEntity>(QueryMultipleRequest request,
            CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetActualFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                commandText = GetQueryMultipleTextInternal<TEntity>(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetQueryMultipleTextInternal<TEntity>(QueryMultipleRequest request,
            IEnumerable<Field> fields)
            where TEntity : class
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            return statementBuilder.CreateQuery(new QueryBuilder(),
                request.Name,
                fields,
                request.Where,
                request.OrderBy,
                request.Top,
                request.Hints);
        }

        #endregion

        #region GetSumText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetSumText(SumRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateSum(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Where,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetSumAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetSumAllText(SumAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateSumAll(new QueryBuilder(),
                    request.Name,
                    request.Field,
                    request.Hints);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetTruncateText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetTruncateText(TruncateRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
                commandText = statementBuilder.CreateTruncate(new QueryBuilder(),
                    request.Name);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        #endregion

        #region GetUpdateText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetUpdateText(UpdateRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetActualFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                commandText = GetUpdateTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetUpdateTextAsync(UpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetActualFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                commandText = GetUpdateTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetUpdateTextInternal(UpdateRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            var primaryField = GetPrimaryField(request);
            var identityField = GetIdentityField(request);
            return statementBuilder.CreateUpdate(new QueryBuilder(),
                request.Name,
                fields,
                request.Where,
                primaryField,
                identityField,
                request.Hints);
        }

        #endregion

        #region GetUpdateAllText

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static string GetUpdateAllText(UpdateAllRequest request)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = GetActualFields(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction);
                commandText = GetUpdateAllTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async Task<string> GetUpdateAllTextAsync(UpdateAllRequest request,
            CancellationToken cancellationToken = default)
        {
            if (cache.TryGetValue(request, out var commandText) == false)
            {
                var fields = await GetActualFieldsAsync(request.Connection,
                    request.Name,
                    request.Fields,
                    request.Transaction,
                    cancellationToken);
                commandText = GetUpdateAllTextInternal(request, fields);
                cache.TryAdd(request, commandText);
            }
            return commandText;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetUpdateAllTextInternal(UpdateAllRequest request,
            IEnumerable<Field> fields)
        {
            var statementBuilder = EnsureStatementBuilder(request.Connection, request.StatementBuilder);
            var primaryField = GetPrimaryField(request);
            var identityField = GetIdentityField(request);
            return statementBuilder.CreateUpdateAll(new QueryBuilder(),
                request.Name,
                fields,
                request.Qualifiers,
                request.BatchSize,
                primaryField,
                identityField,
                request.Hints);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Flushes all the existing cached command texts.
        /// </summary>
        public static void Flush() =>
            cache.Clear();

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static IEnumerable<Field> GetActualFields(IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields,
            IDbTransaction transaction)
        {
            if (fields?.Any() != true)
            {
                return null;
            }
            var dbFields = DbFieldCache.Get(connection, tableName, transaction);
            return GetActualFieldsInternal(fields, dbFields, connection.GetDbSetting());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<Field>> GetActualFieldsAsync(IDbConnection connection,
            string tableName,
            IEnumerable<Field> fields,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            if (fields?.Any() != true)
            {
                return null;
            }
            var dbFields = await DbFieldCache.GetAsync(connection, tableName, transaction, cancellationToken);
            return GetActualFieldsInternal(fields, dbFields, connection.GetDbSetting());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="dbFields"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static IEnumerable<Field> GetActualFieldsInternal(IEnumerable<Field> fields,
            IEnumerable<DbField> dbFields,
            IDbSetting dbSetting)
            => dbFields?.Any() == true ?
                fields.Where(f => dbFields.FirstOrDefault(df =>
                    string.Equals(df.Name.AsUnquoted(true, dbSetting), f.Name.AsUnquoted(true, dbSetting), StringComparison.OrdinalIgnoreCase)) != null) :
                fields;

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static DbField GetPrimaryField(BaseRequest request)
        {
            if (request.Type != null && request.Type != StaticType.Object)
            {
                var primaryProperty = PrimaryCache.Get(request.Type);
                if (primaryProperty != null)
                {
                    var identityProperty = IdentityCache.Get(request.Type);
                    var isIdentity = false;
                    if (identityProperty != null)
                    {
                        isIdentity = string.Equals(identityProperty.GetMappedName(), primaryProperty.GetMappedName(), StringComparison.OrdinalIgnoreCase);
                    }
                    return new DbField(primaryProperty.GetMappedName(),
                        true,
                        isIdentity,
                        false,
                        primaryProperty.PropertyInfo.PropertyType,
                        null,
                        null,
                        null,
                        null);
                }
            }
            return DbFieldCache.Get(request.Connection, request.Name, request.Transaction)?.FirstOrDefault(f => f.IsPrimary);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static DbField GetIdentityField(BaseRequest request)
        {
            if (request.Type != null && request.Type != StaticType.Object)
            {
                var identityProperty = IdentityCache.Get(request.Type);
                if (identityProperty != null)
                {
                    var primaryProperty = PrimaryCache.Get(request.Type);
                    var isPrimary = false;
                    if (primaryProperty != null)
                    {
                        isPrimary = string.Equals(primaryProperty.GetMappedName(), identityProperty.GetMappedName(), StringComparison.OrdinalIgnoreCase);
                    }
                    return new DbField(identityProperty.GetMappedName(),
                        isPrimary,
                        true,
                        false,
                        identityProperty.PropertyInfo.PropertyType,
                        null,
                        null,
                        null,
                        null);
                }
            }
            return DbFieldCache.Get(request.Connection, request.Name, request.Transaction)?.FirstOrDefault(f => f.IsIdentity);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static IStatementBuilder EnsureStatementBuilder(IDbConnection connection,
            IStatementBuilder builder) =>
            builder ?? connection.GetStatementBuilder();

        #endregion
    }
}
