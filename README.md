# BankingAPI

Este projeto simula o processo de criação e transações de uma conta bancária. O sistema inclui autenticação e funcionalidades para saque, depósito e transferência.

Para o desenvolvimento deste projeto foram utilizadas as tecnologias .NET 8 e ASP.NET Core, e o banco de dados SQL Server, executado em um container Docker.

Juntamente com o ASP.NET Core, utilizou-se o Entity Framework Core como ORM para as consultas e atualizações dentro do banco de dados. O Identity também foi adicionado ao projeto para gerenciar usuários e Claims dentro do sistema.

A arquitetura adotada é baseada no padrão *Repository*, com repositórios alocados dentro do **Unit of Work** para evitar a criação de múltiplas instâncias e problemas de concorrência.

Abaixo estão as instruções para a execução do projeto.

## Passo 1: Configurar banco de dados e Migrações

Para executar o banco de dados, é necessário ter o Docker instalado na máquina. Dentro da pasta do projeto, execute o seguinte comando:

```bash
docker compose up
```

Após a instância do banco de dados ser iniciada com sucesso, execute o comando a seguir para aplicar as migrações das tabelas:

```bash
dotnet ef database update
``` 

**Observação:** O arquivo de configuração do projeto já contém as configurações do banco de dados. Caso faça alterações no arquivo `docker-compose.yml`, lembre-se de atualizá-las também no `appsettings.json`.

## Passo 2: Execução do projeto

Para compilar e executar o projeto, na pasta do mesmo, execute o seguinte comando:

```bash
dotnet run
```  

Isso iniciará a aplicação no host `http://localhost:5293`. Você pode consultar todos os endpoints disponíveis e documentados através do OpenAPI (Swagger) usando a URL `http://localhost:5293/swagger`.

## Endpoints da Aplicação

A seguir, estão listados todos os endpoints disponíveis na API para a realização de operações bancárias. As rotas com um cadeado (🔒) indicam que é necessário autenticar-se com um *Bearer Token*. Se tentar acessar essas rotas sem autenticação, você receberá uma mensagem de `Não autorizado`.

### POST /api/account
Cria uma conta bancária para o usuário após o fornecimento dos dados pessoais para cadastro.

**Corpo da requisição:**

```json
{
    "name": "My name",
    "cpf": "12345678901",
    "password": "MyPassword",
    "initialBalance": 1000
}
```

Após a criação da conta, será retornado um identificador único da conta bancária, que pode ser usado para realizar outras operações..

**Exemplo de resposta:**

```json
"3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

### POST /api/auth/login
Após criar a conta bancária, o usuário deve se autenticar para receber um token de autenticação que permitirá o acesso às demais rotas da API.

**Corpo da requisição:**

```json
{
    "cpf": "12345678901",
    "password": "MyPassword"
}
```

**Exemplo de resposta:**
```json
{
    "token": "string",
    "expiration": "2024-07-17T13:27:40.495Z"
}
```

### GET /api/account/balance 🔒

Retorna o saldo atual da conta bancária após informar um identificador único. O identificador deve ser enviado como um *Query Parameter* com a chave **accountId**:

```
GET /api/account/balance?accountId=3fa85f64-5717-4562-b3fc-2c963f66afa6
```

**Exemplo de resposta:**

```json
{
    "balance": 1200
}
``` 

### POST /api/account/transfer 🔒

Realiza a transferência entre duas contas bancárias, fornecendo os identificadores únicos das contas e o valor desejado.

**Corpo da requisição:**

```json
{
    "origin": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "target": "7e0dbbdb-bdd7-4510-b987-90634b292a76",
    "amount": 10
}
``` 

Após a operação, os saldos das contas envolvidas serão atualizados e um histórico da transação será criado.

**Exemplo de resposta:**

```json
{
  "date": "2024-07-17T13:44:51.439Z",
  "type": "Transfer",
  "amount": 10,
  "originId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "destinyId": "7e0dbbdb-bdd7-4510-b987-90634b292a76"
}
```

### POST /api/account/deposit🔒

Realiza o depósito de fundos em uma conta bancária, fornecendo o valor e a conta destino.

**Corpo da requisição:**
```json
{
    "target": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "amount": 10
}
``` 

Após a operação, o saldo da conta será atualizado e um histórico da transação será criado.

**Exemplo de resposta:**

```json
{
  "date": "2024-07-17T13:44:51.439Z",
  "type": "Deposit",
  "amount": 10,
  "originId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "destinyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### POST /api/account/withdraw🔒

Realiza o saque da conta bancária do usuário autenticado.

**Corpo da requisição**
```json
{ 
    "amount": 10
}
```

Após a operação, o saldo da conta será atualizado e um histórico do saque será retornado.

```json 
{
  "date": "2024-07-17T13:44:51.439Z",
  "type": "Withdraw",
  "amount": 10,
  "originId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "destinyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
``` 

### GET /api/account/historic🔒
Retorna o histórico das transações realizadas pelo usuário autenticado, como saques, transferências e depósitos.

Para exibir o histórico, informe a página e o limite de dados desejados.

**Exemplo de requisição:**
``` 
GET /api/account/historic?page=1&pageLimit=10
```

Após a consulta, será retornada uma lista contendo todas as transações realizadas ou recebidas pelo usuário.

**Exemplo de resposta:**
```json
[
    {
        "date": "2024-07-17T13:44:51.439Z",
        "type": "Withdraw",
        "amount": 10,
        "originId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "destinyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    },
    {
        "date": "2024-07-17T13:44:51.439Z",
        "type": "Deposit",
        "amount": 10,
        "originId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "destinyId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    },
    {
        "date": "2024-07-17T13:44:51.439Z",
        "type": "Transfer",
        "amount": 10,
        "originId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "destinyId": "7e0dbbdb-bdd7-4510-b987-90634b292a76"
    },
]
``` 