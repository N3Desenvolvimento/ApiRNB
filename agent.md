## Visão geral do agente

- Objetivo: manter e evoluir uma API simples de acesso a dados Firebird de um sistema legado.
- Stack principal:
  - ASP.NET Core Web API (.NET 6) — projeto [API_RNB.csproj](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/API_RNB.csproj)
  - Acesso a dados com Dapper e FirebirdSql.Data.FirebirdClient em [FirebirdDatabase.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Conexao/FirebirdDatabase.cs)
  - Swagger/Swashbuckle para documentação automática em tempo de execução.
- Foco principal: simplicidade e funcionalidade. Evite arquiteturas complexas; priorize código direto, claro e fácil de ler.

## Estrutura atual do projeto

- Configuração e bootstrap da aplicação:
  - [Program.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Program.cs)
    - Registra controllers, Swagger e a política de CORS `"AllowAll"`.
    - Registra `FirebirdDatabase` como serviço `Scoped` para injeção de dependência.
- Acesso a dados:
  - [Conexao/FirebirdDatabase.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Conexao/FirebirdDatabase.cs)
    - Encapsula a criação de conexão com Firebird.
    - Expõe métodos para executar procedures e queries (ex: `GetPendingVendasHooksAsync`).
- Controllers:
  - [Controllers/ProdutosController.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Controllers/ProdutosController.cs)
    - Endpoint `GET /Produtos` que retorna a lista de produtos da procedure `SP_WEB_API_PRODUTO`.
  - [Controllers/VendasHookController.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Controllers/VendasHookController.cs)
    - Endpoint `GET /VendasHook/monitorar` para listar vendas enviadas.
  - [Controllers/WeatherForecastController.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Controllers/WeatherForecastController.cs)
    - Controller de exemplo padrão do template do ASP.NET Core.
- Services (Background):
  - [Services/VendasHookWorker.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Services/VendasHookWorker.cs)
    - Monitora a tabela `VENDAS_HOOK` e envia dados para webhook.
    - **Arquitetura de Alta Performance**:
      - Implementa padrão Producer-Consumer com `System.Threading.Channels`.
      - Produtor: Monitora o banco e alimenta uma fila em memória (BoundedChannel).
      - Consumidores: Múltiplas tasks concorrentes processam a fila e enviam requisições HTTP.
    - **Resiliência Simplificada**:
      - Tenta enviar até 3 vezes. Se falhar, registra erro e marca como `falha_envio`.
      - Sem agendamento de retry no banco (retry ocorre nos próximos ciclos do worker se `TENTATIVAS < 3`).
      - Registro detalhado de histórico na tabela `VENDAS_HOOK_HISTORICO`.
      - Controle de estados: `pendente`, `enviando`, `enviado_com_sucesso`, `falha_envio`.
- Modelos:
  - `ProdutoModel` e `VendaHookModel` definidos em `FirebirdDatabase.cs`.
  - `WeatherForecast` em [WeatherForecast.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/WeatherForecast.cs).

## Objetivos de design

- Manter o código simples e funcional:
  - Prefira poucas camadas bem definidas (Controller → Acesso a dados → Banco).
  - Evite abstrações genéricas desnecessárias (por exemplo, repositórios genéricos complexos se não forem realmente necessários).
  - Nomes claros para classes, métodos e variáveis.
- Facilitar manutenção por futuros programadores:
  - Separar responsabilidades:
    - Controllers: orquestram a requisição HTTP e retornam `IActionResult`.
    - Classes de acesso a dados: encapsulam apenas o acesso ao Firebird.
  - Manter cada método com responsabilidade única e tamanho reduzido.
  - Adicionar logs onde ajudarem no diagnóstico (usando `ILogger` padrão do ASP.NET Core).
- Segurança e configuração:
  - Não manter strings de conexão sensíveis diretamente no código.
  - Padrão recomendado: mover a string de conexão para `appsettings.json` / `appsettings.Development.json` ou secret manager do .NET.

## Como o fluxo da API funciona hoje

1. O cliente faz uma requisição HTTP (por exemplo, `GET /Produtos`).
2. O ASP.NET Core direciona a requisição para o controller correspondente:
   - `ProdutosController` em [Controllers/ProdutosController.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Controllers/ProdutosController.cs).
3. O controller recebe `FirebirdDatabase` via injeção de dependência (registrado em `Program.cs`).
4. O método `GetProdutos` chama `_firebirdDatabase.ExecuteProcedureAsync()`.
5. `FirebirdDatabase` abre uma conexão Firebird e executa a procedure `SP_WEB_API_PRODUTO` via Dapper.
6. O resultado é mapeado para `ProdutoModel` e retornado no `Ok(...)` do controller.

## Boas práticas específicas para este projeto

- **Simplicidade primeiro**

  - Antes de introduzir novas camadas (por exemplo, Services, Repositories, DTOs complexos), avalie se realmente é necessário.
  - Se um endpoint pode ser implementado com um controller e uma chamada direta a uma classe de acesso a dados, essa é a abordagem preferida.

- **Organização de código**

  - Novos recursos devem seguir padrão semelhante ao de Produtos:
    - Criar um controller em `Controllers/`.
    - Criar uma classe específica de acesso a dados (ou reutilizar uma existente) em uma pasta dedicada (`Conexao/` ou outra pasta de dados).
  - Se a lógica de dados crescer, considere extrair modelos (`Models/`) em arquivos separados.

- **Configuração de banco de dados**

  - A string de conexão atualmente está em `FirebirdDatabase` por simplicidade inicial.
  - Melhoria recomendada: ler a string de conexão de `appsettings.json` via `IConfiguration` e não versionar credenciais sensíveis.

- **CORS**
  - A política `"AllowAll"` em [Program.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Program.cs#L13-L21) está aberta para qualquer origem.
  - Em produção, considere restringir para os domínios que realmente consomem a API.

## Como rodar o projeto

Pré-requisitos:

- .NET 6 SDK instalado.
- A base Firebird configurada e acessível (conforme string de conexão).

Comandos básicos (no diretório do projeto):

- Restaurar dependências:

  ```bash
  dotnet restore
  ```

- Build:

  ```bash
  dotnet build
  ```

- Executar a API:

  ```bash
  dotnet run
  ```

- Ao subir, a API expõe Swagger em `/swagger` (configurado em `Program.cs`).

## Como adicionar um novo endpoint de forma simples

Exemplo de fluxo recomendado:

1. Criar um modelo de dados (se necessário) em um arquivo dedicado dentro de uma pasta `Models/` ou próximo ao ponto de uso, mantendo nomes claros.
2. Criar ou reutilizar uma classe de acesso a dados que:
   - Receba configurações/conexões via injeção.
   - Use Dapper para executar queries ou procedures específicas.
3. Criar um controller em `Controllers/`:
   - Decorar com `[ApiController]` e `[Route("[controller]")]`.
   - Injetar a classe de acesso a dados pelo construtor.
   - Implementar actions (`[HttpGet]`, `[HttpPost]`, etc.) retornando `IActionResult` com `Ok`, `NotFound`, `BadRequest`, etc.
4. Registrar qualquer serviço adicional em `Program.cs` usando `builder.Services`.

## Pontos de atenção para agentes

- Não adicionar frameworks, bibliotecas ou padrões de arquitetura pesados sem necessidade explícita.
- Sempre priorizar:
  - Clareza do código.
  - Baixa quantidade de arquivos/classe para resolver um problema simples.
  - Comentários apenas quando a regra de negócio não for óbvia pelo código.
- Evitar:
  - “Over-engineering” (por exemplo, camadas genéricas que não trazem benefício claro).
  - Quebrar endpoints simples em múltiplas classes quando isso não melhora a legibilidade.
