# Repository

Camada responsável pelo acesso direto aos dados (DAL), executando queries e procedures.

## Arquivos e Responsabilidades

- **[ProdutoRepository.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Repository/ProdutoRepository.cs)**
  - Entidade: Produtos.
  - Operações: Leitura de produtos via procedure `SP_WEB_API_PRODUTO`.

- **[VendasHookRepository.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Repository/VendasHookRepository.cs)**
  - Entidade: Vendas (para Webhook).
  - Operações:
    - `GetPendingVendasHooksAsync`: Busca vendas pendentes de envio.
    - `UpdateVendaHookStatusAsync`: Atualiza status da venda.
    - `InsertVendaHookHistoricoAsync`: Registra log de envio.

## Tecnologia
- Utiliza **Dapper** para mapeamento Objeto-Relacional leve e performático.
