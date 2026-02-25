using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using API_RNB.Conexao;
using API_RNB.Models;
using Dapper;

namespace API_RNB.Repository
{
    public class ProdutoRepository
    {
        private readonly FirebirdDatabase _db;

        public ProdutoRepository(FirebirdDatabase db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ProdutoModel>> GetProdutosAsync()
        {
            using (var connection = _db.CreateConnection())
            {
                connection.Open();

                // Chama a procedure SP_WEB_API_PRODUTO
                var result = await connection.QueryAsync<ProdutoModel>(
                    "SP_WEB_API_PRODUTO",
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
        }

        public async Task<ProdutoModel> GetProdutoByCodigoBarraAsync(string codigoBarra)
        {
            using (var connection = _db.CreateConnection())
            {
                connection.Open();
                var sql =
                    @"SELECT p.IDPRODUTO as Id, p.DESCRICAO, p.REFERENCIA, p.PRECO_VENDA, p.PRECO_PROMOCAO, p.ESTOQUE 
                            FROM PRODUTO p
                            INNER JOIN PRODUTO_CODBARRA pc ON p.IDPRODUTO = pc.IDPRODUTO
                            WHERE pc.CODIGO_BARRA = @CodigoBarra";
                return await connection.QueryFirstOrDefaultAsync<ProdutoModel>(
                    sql,
                    new { CodigoBarra = codigoBarra }
                );
            }
        }
    }
}
