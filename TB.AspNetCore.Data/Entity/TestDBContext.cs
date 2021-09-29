using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TB.AspNetCore.Data.Entity
{
    public class TestDBContext : DbContext
    {
        public TestDBContext(DbContextOptions<TestDBContext> options)
           : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; } 
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        [StringLength(40)]
        public string DisplayName { get; set; }

        public int DownVotes { get; set; }

        public int UpVotes { get; set; }
        
        public string? AboutMe { get; set; }

        public int? Age { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        public DateTime? LastAccessDate { get; set; }

        public int? Reputation { get; set; }

        public int? Views { get; set; }

        [StringLength(200)]
        public string WebsiteUrl { get; set; }
    }

    public class QueryWithNoLockDbCommandInterceptor : DbCommandInterceptor
    {
        private static readonly Regex TableAliasRegex =
            new Regex(@"(?<tableAlias>AS \[[a-zA-Z]\w*\](?! WITH \(NOLOCK\)))",
                RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            command.CommandText = TableAliasRegex.Replace(
                command.CommandText,
                "${tableAlias} WITH (NOLOCK)"
                );
            return base.ScalarExecuting(command, eventData, result);
        }

        public override Task<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            command.CommandText = TableAliasRegex.Replace(
                command.CommandText,
                "${tableAlias} WITH (NOLOCK)"
                );
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            command.CommandText = TableAliasRegex.Replace(
                command.CommandText,
                "${tableAlias} WITH (NOLOCK)"
                );
            return result;
        }

        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            command.CommandText = TableAliasRegex.Replace(
                command.CommandText,
                "${tableAlias} WITH (NOLOCK)"
                );
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }
    }
}
