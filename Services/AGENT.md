# Services

Camada responsável pela lógica de negócios e processos em background.

## Arquivos e Responsabilidades

- **[VendasHookWorker.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Services/VendasHookWorker.cs)**
  - Tipo: `BackgroundService` (Hosted Service).
  - Função: Monitorar vendas pendentes e enviá-las para webhook.
  - Padrão: Producer-Consumer com `System.Threading.Channels`.
  - Ciclo: Verifica o banco a cada 30 segundos.
  - Configuração: URL do webhook hardcoded (`https://hooknitro.phdvirtual.com.br/...`).

- **[WebhookService.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Services/WebhookService.cs)**
  - Responsabilidade: Construir o payload JSON a partir do modelo de venda e realizar o envio HTTP POST.
  - Dependências: `IHttpClientFactory`.

## Fluxo de Trabalho
1. `VendasHookWorker` busca vendas na tabela `VENDAS_HOOK`.
2. Vendas são colocadas em um canal em memória.
3. Consumers processam o canal, usando `WebhookService` para envio.
4. Resultado é gravado em `VENDAS_HOOK_HISTORICO`.
