# Controllers

Esta pasta contém os controladores da API, responsáveis por receber as requisições HTTP e retornar as respostas adequadas.

## Estrutura

- **ProdutosController.cs**: Gerencia as operações relacionadas aos produtos, como consulta de lista de produtos.
- **VendasHookController.cs** (Removido): Anteriormente responsável por monitoramento, agora removido conforme solicitação.

## Padrões

- Os controladores devem ser leves e delegar a lógica de negócios para os Serviços ou Repositórios.
- Utilizam Injeção de Dependência para acessar Repositórios.
- Retornam `IActionResult` com os códigos HTTP apropriados (200, 404, 500, etc.).
