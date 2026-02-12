using API_RNB.Conexao;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_RNB.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly FirebirdDatabase _firebirdDatabase;

        public ProdutosController(FirebirdDatabase firebirdDatabase)
        {
            _firebirdDatabase = firebirdDatabase;
        }

        [HttpGet]
        public async Task<IActionResult> GetProdutos()
        {
            try
            {
                // Obtém os produtos executando a procedure SP_WEB_API_PRODUTO
                var produtos = await _firebirdDatabase.ExecuteProcedureAsync();

                if (produtos == null)
                {
                    return NotFound("Nenhum produto encontrado.");
                }

                return Ok(produtos);
            }
            catch (System.Exception ex)
            {
                // Retorna um erro 500 caso ocorra algum problema
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}
