namespace API_RNB.Conexao
{
    using Dapper;
    using FirebirdSql.Data.FirebirdClient;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;




        public class FirebirdDatabase
        {
            private readonly string _connectionString;

            public FirebirdDatabase()
            {
          
            _connectionString= @"User = SYSDBA; Password = masterkey; Database = C:\\Dados\\RNB.FDB; DataSource = n3solucoes.ankhor.com.br; Port = 3050; Dialect = 3; Charset = NONE; Role =; Connection lifetime = 15; Pooling = true; MinPoolSize = 0; MaxPoolSize = 50; Packet Size = 8192; ServerType = 0;";

        }

            private IDbConnection CreateConnection()
            {
                return new FbConnection(_connectionString);
            }

            public async Task<IEnumerable<ProdutoModel>> ExecuteProcedureAsync()
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();

                    // Chama a procedure SP_WEB_API_PRODUTO
                    var result = await connection.QueryAsync<ProdutoModel>(
                        "SP_WEB_API_PRODUTO",
                        commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
        }

        public class ProdutoModel
        {
            public decimal Preco_Venda { get; set; }
            public string Descricao_Produto { get; set; }
            public int IdProduto { get; set; }
        }
 

}
