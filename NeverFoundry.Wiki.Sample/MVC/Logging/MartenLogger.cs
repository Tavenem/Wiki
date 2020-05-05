using Marten;
using Marten.Services;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Linq;
using System.Text;

namespace NeverFoundry.Wiki.Sample.Logging
{
    public class MartenLogger : IMartenLogger, IMartenSessionLogger
    {
        private readonly ILogger<MartenLogger> _logger;

        /// <summary>
        /// Initialize a new instance of <see cref="MartenLogger"/>.
        /// </summary>
        public MartenLogger(ILogger<MartenLogger> logger) => _logger = logger;

        /// <summary>
        /// Log a command that failed
        /// </summary>
        public void LogFailure(NpgsqlCommand command, Exception ex)
            => _logger.LogError(ex, "Failed Marten command: {CommandText} with parameters {Parameters}", command.CommandText, GetParamaterString(command));

        /// <summary>
        /// Log a command that executed successfully
        /// </summary>
        public void LogSuccess(NpgsqlCommand command)
            => _logger.LogInformation("Successful Marten command: {CommandText} with parameters {Parameters}", command.CommandText, GetParamaterString(command));

        /// <summary>
        /// Called immediately after committing an IDocumentSession
        /// through SaveChanges() or SaveChangesAsync()
        /// </summary>
        public void RecordSavedChanges(IDocumentSession session, IChangeSet commit)
            => _logger.LogInformation("Persisted {UpdateCount} updates, {InsertCount} inserts, and {DeleteCount} deletions", commit.Updated.Count(), commit.Inserted.Count(), commit.Deleted.Count());

        public void SchemaChange(string sql) => _logger.LogInformation("Executing DDL change: {SQL}", sql);

        public IMartenSessionLogger StartSession(IQuerySession session) => this;

        private string GetParamaterString(NpgsqlCommand command)
        {
            var sb = new StringBuilder();
            foreach (var p in command.Parameters.Where(x => x != null))
            {
                sb.Append("  ")
                    .Append(p.ParameterName)
                    .Append(": ")
                    .Append(p.Value);
            }
            return sb.ToString();
        }
    }
}
