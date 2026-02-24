# Repository

Esta pasta contém a camada de acesso a dados da aplicação, separada por entidade/tabela.

## Responsabilidades

- **ProdutoRepository.cs**:
  - Encapsula as consultas SQL relacionadas a produtos (`PRODUTO`, `PRODUTO_CODBARRA`).
  - Utiliza Dapper para mapeamento ORM.
  - Implementa métodos para listar produtos e buscar por código de barras.

- **VendasHookRepository.cs**:
  - Encapsula as operações relacionadas a hooks de vendas (`VENDAS_HOOK`, `VENDAS_HOOK_HISTORICO`).
  - Implementa métodos para buscar vendas pendentes, atualizar status/retry, e registrar histórico de envio.

## Padrões

- Os repositórios recebem `FirebirdDatabase` via injeção de dependência.
- As conexões são abertas apenas quando necessárias (`using (var connection = ...)`).
- Dapper é utilizado para performance e simplicidade.
