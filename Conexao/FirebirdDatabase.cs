using System;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace API_RNB.Conexao
{
    public class FirebirdDatabase
    {
        private readonly string _connectionString;

        public FirebirdDatabase()
        {
            _connectionString =
                @"User = SYSDBA; Password = masterkey; Database = C:\\Dados\\RNBORRACHAS.FDB; DataSource = 187.19.154.102; Port = 3050; Dialect = 3; Charset = NONE; Role =; Connection lifetime = 15; Pooling = true; MinPoolSize = 0; MaxPoolSize = 50; Packet Size = 8192; ServerType = 0;";
        }

        public IDbConnection CreateConnection()
        {
            return new FbConnection(_connectionString);
        }
    }
}
