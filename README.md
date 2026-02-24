# API_RNB

Este projeto é uma API em .NET 6 que se conecta a um banco de dados Firebird para gerenciar produtos e integrar vendas via webhook.

## Estrutura do Projeto

- **Controllers**: Controladores da API (ex: `ProdutosController`, `VendasHookController`).
- **Conexao**: Configuração e acesso ao banco de dados Firebird (`FirebirdDatabase`).
- **Services**: Serviços de background e lógicas de negócio (`VendasHookWorker`).
- **Models**: Modelos de dados (ex: `ProdutoModel`, `VendaHookModel`).

## Funcionalidades Principais

### 1. Consulta de Produtos
- Rota: `GET /Produtos`
- Descrição: Executa a procedure `SP_WEB_API_PRODUTO` e retorna a lista de produtos.

### 2. Monitoramento de Vendas (Webhook)
- **Serviço**: `VendasHookWorker` (Background Service)
- **Funcionamento**:
  - Monitora a tabela `VENDAS_HOOK` a cada 1 minuto.
  - Busca registros com `ENVIADO` nulo ou 'N'.
  - Envia um POST para `https://teste.com` com `{ "Nome": "...", "Email": "..." }`.
  - Atualiza o registro para `ENVIADO = 'S'` e preenche `DATA_ENVIO`.

### 3. Monitoramento de Envios
- Rota: `GET /VendasHook/monitorar`
- Descrição: Retorna a lista de vendas que já foram enviadas para o webhook.

## Configuração

- A string de conexão com o Firebird está definida em `FirebirdDatabase.cs`.
- A URL do webhook está definida em `VendasHookWorker.cs`.

## Como Rodar

1. Certifique-se de ter o .NET 6 SDK instalado.
2. Configure o acesso ao banco de dados Firebird.
3. Execute o projeto:
   ```bash
   dotnet run
   ```
4. Acesse o Swagger em `https://localhost:7125/swagger` (ou a porta configurada).
