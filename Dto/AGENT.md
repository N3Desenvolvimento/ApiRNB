# Dto

Esta pasta contém os Objetos de Transferência de Dados (DTOs), utilizados para transferir dados entre a API e seus consumidores.

## Padrão de Nomenclatura

- **[Entidade]DtoInput**: DTOs usados para entrada de dados (POST/PUT/PATCH), contendo apenas os campos necessários para a criação ou atualização.
- **[Entidade]DtoOutput**: DTOs usados para saída de dados (GET/Listas), contendo os dados formatados para consumo externo.

## Fluxo de Dados

1.  **Entrada**: Controller recebe `DtoInput` -> Converte para `Model` -> Envia para Repository/Service.
2.  **Saída**: Repository retorna `Model` -> Controller converte para `DtoOutput` -> Retorna para o cliente.

## Observações

- Os DTOs desacoplam o modelo de banco de dados (Models) do contrato da API.
- Permitem versionamento e evolução da API sem quebrar clientes existentes.
- Podem conter validações (DataAnnotations) específicas para entrada de dados.
