using API_RNB.Dto;
using API_RNB.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_RNB.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutoRepository _produtoRepository;

        public ProdutosController(ProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDtoOutput>>> GetProdutos()
        {
            try
            {
                // Obtém os produtos através do repositório
                var produtos = await _produtoRepository.GetProdutosAsync();

                if (produtos == null)
                {
                    return NotFound("Nenhum produto encontrado.");
                }

                // Mapeia ProdutoModel para ProdutoDtoOutput
                var produtosDto = produtos.Select(p => new ProdutoDtoOutput
                {
                    Id = p.Id,
                    Descricao = p.Descricao,
                    Referencia = p.Referencia,
                    Preco_Venda = p.Preco_Venda,
                    Preco_Promocao = p.Preco_Promocao,
                    Estoque = p.Estoque,
                });

                return Ok(produtosDto);
            }
            catch (System.Exception ex)
            {
                // Retorna um erro 500 caso ocorra algum problema
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}
