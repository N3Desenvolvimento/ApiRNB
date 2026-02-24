# Models

Esta pasta contém as classes de modelo (POCOs) que representam as entidades de domínio da aplicação.

## Modelos

- **ProdutoModel.cs**: Representa um produto com seus atributos básicos (Id, Descricao, Preco, Estoque).
- **VendaHookModel.cs**: Representa uma venda com seus detalhes de hook (Nome, Email, Enviado, DataEnvio, Status, Tentativas, LogErro).
- **VendaHookHistoricoModel.cs**: Representa o histórico de envio de um hook de venda, com detalhes da requisição/resposta e sucesso/erro.

## Observações

- Os modelos devem refletir a estrutura do banco de dados, mas não conter lógica de negócio.
- Utilizam propriedades públicas com getters/setters.
- Podem conter anotações de validação ou serialização se necessário.
