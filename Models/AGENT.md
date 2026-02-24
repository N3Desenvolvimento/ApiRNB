# Models

Camada responsável pelas definições das entidades de domínio e mapeamento do banco de dados.

## Arquivos e Responsabilidades

- **[ProdutoModel.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Models/ProdutoModel.cs)**
  - Representa um produto retornado pela procedure `SP_WEB_API_PRODUTO`.

- **[VendaHookModel.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Models/VendaHookModel.cs)**
  - Representa uma venda na tabela `VENDAS_HOOK`.
  - Contém lista de `Produtos` (`VendaItemModel`).

- **[VendaItemModel.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Models/VendaItemModel.cs)**
  - Representa um item de venda.

- **[VendaHookHistoricoModel.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Models/VendaHookHistoricoModel.cs)**
  - Representa o histórico de envio na tabela `VENDAS_HOOK_HISTORICO`.
