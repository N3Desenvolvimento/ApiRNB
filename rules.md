## Regras gerais de desenvolvimento

- Manter o projeto simples e direto ao ponto.
- Priorizar código legível e fácil de manter para qualquer desenvolvedor futuro.
- Evitar padrões arquiteturais complexos quando uma solução simples atende bem.
- Manter o projeto focado em ser uma API de acesso ao banco Firebird existente.

## Stack e dependências

- Plataforma: ASP.NET Core Web API (.NET 6).
- Acesso a dados: Dapper + FirebirdSql.Data.FirebirdClient.
- Documentação em runtime: Swagger (Swashbuckle.AspNetCore).
- Arquivos principais:
  - [Program.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Program.cs)
  - [Conexao/FirebirdDatabase.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Conexao/FirebirdDatabase.cs)
  - [Controllers/ProdutosController.cs](file:///c:/Users/kawan/OneDrive/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Controllers/ProdutosController.cs)

## Convenções de código

- Nomenclatura:
  - Classes em PascalCase (`ProdutosController`, `FirebirdDatabase`, `ProdutoModel`).
  - Métodos em PascalCase (`GetProdutos`, `ExecuteProcedureAsync`).
  - Variáveis e campos privados em camelCase (`_firebirdDatabase`, `_connectionString`).
- Organização:
  - Controllers em `Controllers/`.
  - Acesso a dados em `Conexao/` (ou pasta equivalente, se for evoluída no futuro).
  - Modelos podem ser mantidos próximos ao uso ou em uma pasta `Models/` se crescerem.
- Comentários:
  - Usar apenas quando a regra de negócio não for óbvia pelo código.
  - Não usar comentários para explicar o óbvio (nome bem escolhido deve ser suficiente).

## Criação de novos endpoints

- Criar uma nova classe de controller em `Controllers/`:
  - Decorar com `[ApiController]` e `[Route("[controller]")]`.
  - Usar injeção de dependência via construtor para acessar classes de serviço/dados.
- Manter actions pequenas, com uma responsabilidade clara.
- Retornar `IActionResult` e utilizar os helpers padrões:
  - `Ok(...)`, `NotFound(...)`, `BadRequest(...)`, `StatusCode(...)` etc.
- Se for necessário tratamento de erro, utilizar `try/catch` no nível do controller quando fizer sentido e considerar logar exceções.

## Acesso a dados e Firebird

- Centralizar lógica de acesso ao banco em classes específicas (como `FirebirdDatabase`).
- Usar Dapper para chamadas simples a:
  - Procedures existentes no sistema legado (como `SP_WEB_API_PRODUTO`).
  - Queries específicas, evitando criar camadas genéricas complexas sem necessidade.
- Manter queries/procedures com nomes claros e aderentes ao legado.
- Evitar duplicar lógica de acesso; se uma mesma procedure/query for usada em vários lugares, extrair para um método reutilizável.

## Configuração e segurança

- Não versionar credenciais sensíveis:
  - Strings de conexão, usuários e senhas não devem ficar hardcoded em código.
  - Padrão recomendado:
    - Definir a string de conexão em `appsettings.json` / `appsettings.Development.json`.
    - Em ambientes sensíveis, usar o Secret Manager do .NET ou variáveis de ambiente.
- Manter a configuração centralizada:
  - Registrar serviços em `Program.cs`.
  - Evitar espalhar lógica de configuração pelo código.

## CORS e exposição da API

- A política atual `"AllowAll"` facilita o desenvolvimento e consumo amplo.
- Em produção, preferir restringir CORS para domínios conhecidos.
- Qualquer alteração de política CORS deve ser feita em `Program.cs` para manter um único ponto de verdade.

## Estilo de arquitetura

- Arquitetura preferida:
  - Controller → Classe de acesso a dados → Banco (procedures/queries).
- Introduzir serviços intermediários apenas quando:
  - Houver regra de negócio significativa a ser isolada.
  - A complexidade do controller estiver crescendo demais.
- Evitar:
  - Camadas e padrões que não estejam trazendo valor claro (ex.: repositórios genéricos, mapeadores complexos) para casos simples.

## Serviços em Background

- Para tarefas periódicas (ex: monitoramento de tabelas), utilize `BackgroundService`.
- Registre o serviço em `Program.cs` com `builder.Services.AddHostedService<T>()`.
- Como `FirebirdDatabase` é um serviço escopado (Scoped), injete `IServiceScopeFactory` no worker e crie um escopo (`using (var scope = _scopeFactory.CreateScope())`) para resolver o serviço de banco de dados dentro do loop de execução.
- Mantenha o loop de execução resiliente a falhas (try/catch) para não derrubar a aplicação.
- **Performance e Escalabilidade**:
  - Para alto volume, utilize o padrão Producer-Consumer com `System.Threading.Channels`.
  - Controle a concorrência de requisições externas (ex: `SemaphoreSlim` ou controle de tasks).
  - Use `BoundedChannel` para evitar consumo excessivo de memória.
- **Resiliência e Retry**:
  - Tenta processar até 3 vezes em caso de falha.
  - Registra histórico de sucesso e falha.
  - Não utiliza agendamento complexo no banco (mantém simplicidade).

## Qualidade e testes

- Garantir que o projeto compila (`dotnet build`) antes de subir alterações.
- Quando possível, validar manualmente endpoints principais (por exemplo, via Swagger, Postman ou ferramentas similares).
- Se forem adicionados testes automatizados no futuro, seguir convenções padrão de testes .NET (xUnit, NUnit ou MSTest, conforme escolhido).
