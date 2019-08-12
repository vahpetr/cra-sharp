using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services {
    public class ResilientTransaction {
        private DbContext _context;

        private ResilientTransaction (DbContext context) =>
            _context = context ??
            throw new ArgumentNullException (nameof (context));

        public static ResilientTransaction New (DbContext context) =>
            new ResilientTransaction (context);

        // https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-resilient-entity-framework-core-sql-connections
        public async Task<T> ExecuteAsync<T> (Func<Task<T>> action, CancellationToken ct) {
            // Use of an EF Core resiliency strategy when using multiple DbContexts 
            // within an explicit BeginTransaction():
            // https://docs.microsoft.com/ef/core/miscellaneous/connection-resiliency
            var strategy = _context.Database.CreateExecutionStrategy ();
            return await strategy.ExecuteAsync (async () => {
                // BUG can not set IsolationLevel.ReadCommitted in BeginTransactionAsync
                // https://docs.microsoft.com/ru-ru/ef/core/saving/transactions
                // may be rewrite to TransactionScope https://docs.microsoft.com/en-us/previous-versions/ms172152(v=vs.90)
                using (var transaction = await _context.Database.BeginTransactionAsync (ct)) {
                    var result = await action ();
                    transaction.Commit ();
                    return result;
                }
            });
        }
    }
}