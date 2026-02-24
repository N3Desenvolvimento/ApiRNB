# Conexao

Camada responsável pela configuração e gerenciamento da conexão com o banco de dados.

## Arquivos e Responsabilidades

- **[FirebirdDatabase.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Conexao/FirebirdDatabase.cs)**
  - Responsabilidade: Criar e fornecer instâncias de `IDbConnection` para o Firebird.
  - Biblioteca: `FirebirdSql.Data.FirebirdClient`.
  - Configuração: A Connection String está definida internamente nesta classe (Hardcoded).
  - Escopo: Registrado como `Scoped` no container de DI.

## Observações
- A Connection String deve ser movida para `appsettings.json` em futuras refatorações para maior segurança e flexibilidade.
