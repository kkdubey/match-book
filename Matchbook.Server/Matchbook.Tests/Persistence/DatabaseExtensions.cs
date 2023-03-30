using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Matchbook.Tests.Persistence
{
    public static class DatabaseExtensions
    {
        public static RelationalDataReader ExecuteSqlQuery(this DatabaseFacade databaseFacade, string sql, params object[] parameters)
        {
            var concurrencyDetector = databaseFacade.GetService<IConcurrencyDetector>();

            using (concurrencyDetector.EnterCriticalSection())
            {
                var rawSqlCommand = databaseFacade
                    .GetService<IRawSqlCommandBuilder>()
                    .Build(sql, parameters);

                var paramObject = new RelationalCommandParameterObject(databaseFacade.GetService<IRelationalConnection>(), rawSqlCommand.ParameterValues, null, null);

                return rawSqlCommand
                    .RelationalCommand
                    .ExecuteReader(paramObject);
            }
        }
    }
}
