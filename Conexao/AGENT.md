# Conexao

Esta pasta é responsável exclusivamente pela infraestrutura de conexão com o banco de dados Firebird.

## Responsabilidades

- **FirebirdDatabase.cs**:
  - Contém a string de conexão centralizada.
  - Implementa o método `CreateConnection()` para retornar uma `IDbConnection` (FbConnection).
  - Configurações de pooling, dialeto e charset.

## Notas Técnicas

- A string de conexão aponta para um banco local (`C:\Dados\RNBORRACHAS.FDB`).
- O acesso a dados (Queries/Commands) foi movido para a camada `Repository`.
