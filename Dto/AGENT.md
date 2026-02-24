# Dto (Data Transfer Objects)

Camada responsável por definir objetos de transferência de dados, usados para comunicação externa (API/Webhook) e desacoplamento do modelo de domínio.

## Arquivos e Responsabilidades

- **[WebhookPayloadDtoInput.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Dto/WebhookPayloadDtoInput.cs)**
  - Estrutura do JSON enviado para o Webhook externo.
  - Propriedades: `Cpf`, `Nome`, `Email`, `Telefone`, `Produtos`, etc.

- **[VendaHookStatusDtoInput.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Dto/VendaHookStatusDtoInput.cs)**
  - Dados para atualização de status da venda no repositório.

- **[VendaHookHistoricoDtoInput.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Dto/VendaHookHistoricoDtoInput.cs)**
  - Dados para inserção no histórico de tentativas de envio.

- **[ProdutoDtoOutput.cs](file:///c:/Users/kawan/OneDrive/Área%20de%20Trabalho/Projetos%20Atualizados/_RNB/API_RNB/Dto/ProdutoDtoOutput.cs)**
  - Saída formatada para o endpoint de produtos (se utilizado).
