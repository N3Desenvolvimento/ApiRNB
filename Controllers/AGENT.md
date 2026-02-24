# Controllers

Camada responsável por receber requisições HTTP e retornar respostas ao cliente.

## Arquivos e Responsabilidades

- **[ProdutosController.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Controllers/ProdutosController.cs)**
  - Endpoint: `GET /Produtos`
  - Responsabilidade: Retornar a lista de produtos cadastrados.
  - Dependências: `ProdutoRepository`.

- **[WeatherForecastController.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Controllers/WeatherForecastController.cs)**
  - Endpoint: `GET /WeatherForecast`
  - Responsabilidade: Exemplo de endpoint padrão do template.

## Padrões
- Controllers devem ser magros (Thin Controllers), delegando lógica de negócios para Services e acesso a dados para Repositories.
- Retorno deve ser sempre `IActionResult` (ex: `Ok()`, `BadRequest()`).
