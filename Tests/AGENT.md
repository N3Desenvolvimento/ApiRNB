# Tests

Esta pasta contém os testes unitários e de integração da aplicação.

## Estrutura

- Testes devem estar organizados por funcionalidade (ex: `VendasHookWorkerTests.cs`).
- Utiliza `xUnit` como framework de testes.
- Utiliza `Moq` para criar mocks de dependências (`FirebirdDatabase`, `IHttpClientFactory`, `ILogger`, etc.).

## Notas Técnicas

- Testes de carga podem ser implementados usando ferramentas como k6 ou JMeter.
- Testes de integração devem validar o fluxo completo de venda -> worker -> webhook.
