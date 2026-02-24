## Visão geral do agente

- **Objetivo**: Manter e evoluir uma API de acesso a dados Firebird e integração de vendas via Webhook.
- **Stack principal**:
  - ASP.NET Core Web API (.NET 6) — projeto [API_RNB.csproj](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/API_RNB.csproj)
  - Acesso a dados com Dapper e FirebirdSql.Data.FirebirdClient em [FirebirdDatabase.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Conexao/FirebirdDatabase.cs)
  - Swagger/Swashbuckle para documentação automática.
  - Background Services para processamento assíncrono.
- **Foco principal**: Simplicidade, funcionalidade e performance.

## Estrutura atual do projeto

- **Configuração e Bootstrap**:
  - [Program.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Program.cs): Configura DI, Swagger, CORS ("AllowAll") e Workers.
- **Acesso a Dados (Data Layer)**:

  - [Conexao/FirebirdDatabase.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Conexao/FirebirdDatabase.cs): Gerencia conexão com Firebird (string de conexão hardcoded atualmente).
  - [Repository/ProdutoRepository.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Repository/ProdutoRepository.cs): Acesso a dados de produtos.
  - [Repository/VendasHookRepository.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Repository/VendasHookRepository.cs): Acesso a dados de vendas para webhook.

- **Controllers (API Layer)**:

  - [Controllers/ProdutosController.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Controllers/ProdutosController.cs): Endpoint `GET /Produtos`.
  - [Controllers/WeatherForecastController.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Controllers/WeatherForecastController.cs): Exemplo padrão.

- **Services (Business/Background Layer)**:

  - [Services/VendasHookWorker.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Services/VendasHookWorker.cs):
    - Serviço de Background (HostedService).
    - Monitora tabela `VENDAS_HOOK` a cada 30s.
    - Implementa padrão Producer-Consumer com `System.Threading.Channels` para alta performance.
    - Envia dados para webhook externo configurado no código.
  - [Services/WebhookService.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Services/WebhookService.cs): Lógica de construção de payload e envio HTTP.

- **Models & DTOs**:
  - [Models/](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Models/): Modelos de domínio (ex: `ProdutoModel`, `VendaHookModel`).
  - [Dto/](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Dto/): Objetos de transferência de dados (ex: `WebhookPayloadDtoInput`).

## Fluxos de Execução

### 1. Consulta de Produtos

1. Cliente chama `GET /Produtos`.
2. `ProdutosController` aciona `ProdutoRepository`.
3. `ProdutoRepository` executa procedure `SP_WEB_API_PRODUTO` via `FirebirdDatabase`.
4. Retorna lista de produtos.

### 2. Integração de Vendas (Webhook)

1. `VendasHookWorker` (Producer) roda em background a cada 30s.
2. Consulta `VENDAS_HOOK` via `VendasHookRepository` buscando vendas não enviadas (`ENVIADO IS NULL` ou `'N'`).
3. Publica vendas encontradas em um `Channel` em memória.
4. Múltiplos Consumers (Tasks) leem do `Channel`.
5. Cada Consumer:
   - Monta payload com `WebhookService`.
   - Envia POST para URL configurada (`https://hooknitro.phdvirtual.com.br/...`).
   - Registra sucesso/falha em `VENDAS_HOOK_HISTORICO` e atualiza status em `VENDAS_HOOK`.

## Objetivos de Design e Boas Práticas

- **Simplicidade**: Manter arquitetura limpa, sem super-engenharia.
- **Performance**: Uso de `Dapper` para banco e `Channels` para processamento assíncrono.
- **Organização**:
  - Controllers: Apenas HTTP.
  - Services: Regras de negócio e orquestração.
  - Repository: Acesso a dados puro.
- **Configuração**:
  - _TODO_: Mover Connection String e Webhook URL para `appsettings.json` (atualmente hardcoded).

## Comandos Disponíveis

- `dotnet run`: Inicia a aplicação.
- `dotnet build`: Compila o projeto.
- Swagger acessível em `/swagger` quando em execução.
