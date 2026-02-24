# Services

Esta pasta contém a lógica de negócios e processamento assíncrono da aplicação.

## Principais Serviços

- **VendasHookWorker.cs**:
  - Implementa um `BackgroundService` para processamento de vendas em segundo plano.
  - Utiliza o padrão **Producer-Consumer** com `System.Threading.Channels`.
  - Gerencia o fluxo de envio de hooks, tratamento de erros e retentativas (máximo 3).
  - Inclui logs visuais periódicos e contador de atividade (0-30s).
  - Utiliza `VendasHookRepository` para interagir com o banco de dados.

## Dependências

- `VendasHookRepository`: Para operações de leitura e escrita de status de vendas.
- `IHttpClientFactory`: Para envio de requisições HTTP externas.
- `ILogger`: Para registro de atividades e erros.
