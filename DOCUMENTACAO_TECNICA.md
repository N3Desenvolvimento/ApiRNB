# Documentação Técnica - API RNB

## 1. Visão Geral do Projeto
A **API RNB** é uma aplicação backend desenvolvida em .NET 6 destinada a fornecer uma interface moderna e eficiente para acesso a dados legados armazenados em banco de dados Firebird. Além de expor endpoints para consulta de dados (como produtos), a aplicação possui um sistema robusto de processamento em background para integração de vendas via Webhook, garantindo que eventos de negócio sejam propagados para sistemas externos com resiliência.

**Objetivos Principais:**
- Expor dados do ERP (Firebird) via API REST.
- Monitorar novas vendas em tempo real e enviá-las para plataformas externas (Webhook).
- Garantir alta performance e baixo acoplamento.

## 2. Estrutura de Diretórios e Arquivos

A organização do código segue o padrão de arquitetura em camadas simplificada, facilitando a navegação e manutenção:

```
API_RNB/
├── Conexao/            # Gerenciamento de conexão com banco de dados (FirebirdDatabase)
├── Controllers/        # Pontos de entrada da API (Endpoints HTTP)
├── Dto/                # Objetos de Transferência de Dados (Input/Output models)
├── Models/             # Modelos de domínio representando tabelas/entidades
├── Repository/         # Camada de Acesso a Dados (DAL) usando Dapper
├── Services/           # Lógica de negócios e Background Services (Workers)
├── Tests/              # Estrutura reservada para testes automatizados
├── Program.cs          # Ponto de entrada, configuração de DI e Pipeline HTTP
└── API_RNB.csproj      # Definição do projeto e dependências NuGet
```

**Responsabilidades:**
- **Controllers**: Recebem requisições HTTP, validam e chamam Repositories ou Services.
- **Repository**: Executam SQL/Procedures puros.
- **Services**: Contêm regras de negócio complexas e tarefas agendadas (Workers).

## 3. Setup e Configuração do Ambiente

**Pré-requisitos:**
- SDK do .NET 6.0 instalado.
- Servidor Firebird (versão compatível com 2.5/3.0) acessível.
- Visual Studio 2022 ou VS Code.

**Configuração:**
1. Clone o repositório.
2. Restaure as dependências:
   ```bash
   dotnet restore
   ```
3. **Banco de Dados**:
   - A string de conexão atualmente está definida na classe `Conexao/FirebirdDatabase.cs`.
   - Certifique-se de que o caminho do banco (`C:\Dados\RNBORRACHAS.FDB` ou IP remoto) esteja acessível.
4. **Webhook**:
   - A URL de destino do Webhook está configurada em `Services/VendasHookWorker.cs`.

## 4. Arquitetura do Sistema

O sistema utiliza uma arquitetura **Web API com Background Workers**.

**Padrões Utilizados:**
- **Dependency Injection (DI)**: Gerenciamento de ciclo de vida de objetos (Scoped, Singleton, HostedService).
- **Repository Pattern**: Abstração do acesso a dados.
- **Producer-Consumer**: Implementado no `VendasHookWorker` utilizando `System.Threading.Channels`. Isso permite separar a leitura do banco (Producer) do envio HTTP (Consumer), maximizando o throughput e evitando bloqueios.
- **DTO (Data Transfer Object)**: Separação entre modelos de banco e contratos de API/Webhook.

**Decisões Técnicas:**
- **Dapper**: Escolhido pela alta performance e simplicidade em mapear queries SQL para objetos, ideal para bancos legados onde o EF Core pode ser excessivo ou incompatível.
- **FirebirdClient**: Driver nativo para comunicação com Firebird.
- **BackgroundService**: Uso de `IHostedService` para tarefas contínuas sem depender de agendadores externos (como Windows Task Scheduler).

## 5. Guia de Contribuição

**Convenções de Código:**
- Use PascalCase para Classes, Métodos e Propriedades Públicas.
- Use camelCase para variáveis locais e parâmetros.
- Use `_camelCase` para campos privados.
- Mantenha os Controllers "magros" e mova lógica para Services/Repositories.

**Fluxo de Pull Request:**
1. Crie uma branch a partir da `main` (`feature/nova-funcionalidade`).
2. Implemente a funcionalidade e documente.
3. Abra um PR descrevendo as mudanças.
4. Aguarde Code Review antes do merge.

## 6. Testes, Build e Deployment

**Build:**
Para compilar o projeto:
```bash
dotnet build
```

**Execução Local:**
```bash
dotnet run
```
A API estará disponível em `https://localhost:7125` (ou porta configurada). O Swagger pode ser acessado em `/swagger`.

**Deployment:**
Para gerar os binários de produção:
```bash
dotnet publish -c Release -o ./publish
```
O conteúdo da pasta `./publish` pode ser implantado em um servidor IIS ou rodar como serviço Linux/Windows.

**Testes:**
Atualmente não há cobertura de testes automatizados. Recomenda-se a implementação de testes unitários para os Services e testes de integração para os Repositories.

## 7. Documentação de APIs

A documentação interativa (Swagger UI) é gerada automaticamente.

**Endpoints Principais:**

### Produtos
- **GET /Produtos**
  - **Descrição**: Retorna a lista completa de produtos.
  - **Retorno**: Array de objetos `ProdutoModel`.

### Webhook (Monitoramento)
- A integração de Webhook não possui endpoint de gatilho manual, ela roda automaticamente em background.
- Logs de envio são armazenados na tabela `VENDAS_HOOK_HISTORICO`.

## 8. Tratamento de Erros

**API:**
- Erros não tratados retornam Status 500 (Internal Server Error).
- Recomenda-se verificar os logs do console/servidor para detalhes.

**Background Worker (Webhook):**
- O Worker possui blocos `try/catch` robustos.
- Falhas de envio HTTP:
  - O erro é capturado.
  - O status na tabela `VENDAS_HOOK` permanece pendente (se tentativas < 3).
  - O detalhe do erro é gravado na coluna `LOG_ERRO` e na tabela de histórico.
  - O sistema tenta reenviar no próximo ciclo (até atingir limite de tentativas).

## 9. Dependências

As principais bibliotecas utilizadas são:

| Pacote | Versão | Propósito |
|--------|--------|-----------|
| `Dapper` | 2.1.66 | Micro-ORM para acesso a dados rápido. |
| `FirebirdSql.Data.FirebirdClient` | 10.3.2 | Driver ADO.NET para Firebird. |
| `Swashbuckle.AspNetCore` | 6.5.0 | Geração de documentação Swagger/OpenAPI. |

Recomenda-se manter estas versões ou superiores, testando sempre a compatibilidade com a versão do servidor Firebird.

## 10. Exemplos de Uso

**Consumindo a API de Produtos (cURL):**
```bash
curl -X 'GET' \
  'https://localhost:7125/Produtos' \
  -H 'accept: application/json'
```

**Payload de Exemplo (Webhook Enviado):**
```json
{
  "cpf": "123.456.789-00",
  "nome": "João Silva",
  "email": "joao@email.com",
  "telefone": "(11) 99999-9999",
  "dataDeNascimento": "1990-01-01",
  "valorTotal": 150.50,
  "produtos": [
    {
      "name": "Produto A",
      "descricao": "Descrição detalhada A",
      "valor": 50.25
    },
    {
      "name": "Produto B",
      "descricao": "Descrição detalhada B",
      "valor": 100.25
    }
  ]
}
```
