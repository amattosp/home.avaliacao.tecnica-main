---

# 🏠 Home.AvaliacaoTecnica

Este repositório contém a implementação de uma avaliação técnica para uma oportunidade de emprego. A versão inicial do projeto está estruturada utilizando o modelo Git Flow, com branches específicas para desenvolvimento e funcionalidades.

---

## 🏗️ Estrutura da Solução

Este projeto segue os princípios da Clean Architecture, com separação clara de responsabilidades entre as camadas de domínio, aplicação, infraestrutura e interface (API). Abaixo está a estrutura dos diretórios do projeto, acompanhada de uma breve descrição de cada componente:

## 🗂️ Estrutura do Projeto

- **Home,AvaliacaoTecnica.sln**: Arquivo de solução do projeto.
- **.github/**: Workflows e configurações do GitHub.
- **Home.AvaliacaoTecnica.Application:**: Camada de aplicação (use cases, serviços de orquestração).
- **Home.AvaliacaoTecnica.Consumer:**: Serviço que consome mensagens (ex: Azure Service Bus).
- **Home.AvaliacaoTecnica.Contracts:**: DTOs, comandos, eventos e contratos entre camadas.
- **Home.AvaliacaoTecnica.Domain:**: Lógica de domínio (entidades, regras de negócio).
- **Home.AvaliacaoTecnica.Domain.Unit:**: Testes unitários da camada de domínio.
- **Home.AvaliacaoTecnica.Tests.Integrations:**: Testes de integração e2e.
- **Home.AvaliacaoTecnica.Infra.Data:**: Implementações de repositórios, acesso a dados (EF Core).
- **Home.AvaliacaoTecnica.ProcessorService:**: Serviço worker para tarefas contínuas ou agendadas.
- **Home.AvaliacaoTecnica.WebApi:**: API HTTP (ponto de entrada principal).
- **bin:**: Arquivos compilados.
- **obj:**: Arquivos intermediários de build.
- **.dockerignore**: Arquivos ignorados durante o build Docker.
- **.gitattributes / .gitignore**: Arquivos de configuração do Git.
- **README.md**: Instruções do projeto.

---

### 🧾 Descrição das Camadas

- **WebApi**: Ponto de entrada da aplicação via HTTP.
- **Application**: Contém a lógica de orquestração entre as camadas.
- **Domain**: Regras de negócio puras e entidades.
- **Infra.Data**: Persistência e implementação de repositórios.
- **Contracts**: Objetos de troca de dados entre camadas.
- **Consumer**: Leitura e processamento de mensagens assíncronas.
- **ProcessorService**: Worker background para tarefas em lote ou programadas.
- **Domain.Unit**: Testes da camada de domínio, garantindo robustez nas regras de negócio.
- **Tests.Integrations**: Teste de integração, para validar o comportamento da aplicação em todas as camadas.


## 📦 Estrutura de Branches (Git Flow)

O projeto segue a estrutura de branches do Git Flow:

* `main`: Branch principal contendo a versão estável e pronta para produção.
* `develop`: Branch de desenvolvimento, onde as funcionalidades são integradas.
* `feature/*`: Branches específicas para o desenvolvimento de novas funcionalidades.

---

## 🚀 Como Executar o Projeto

### Pré-requisitos

Certifique-se de ter as seguintes ferramentas instaladas:

* [Git](https://git-scm.com/)
* [Visual Studio](https://visualstudio.microsoft.com/) (para projetos .NET)
* [Docker](https://www.docker.com/) (se aplicável)

### ⚠️ Configuração Obrigatória: Azure Service Bus

Antes de executar a aplicação, é necessário configurar a chave de acesso ao Azure Service Bus.

1. Obtenha a connection string válida do Azure Service Bus.
2. No arquivo `appsettings.json` (ou o usado para o ambiente), adicione:

"AzureServiceBus": {
  "ConnectionString": "sua-connection-string-aqui"
 }

 A string de conexão do service bus deverá ser inserida nos seguintes projetos:
 - **Home.AvaliacaoTecnica.WebApi:**
 - **Home.AvaliacaoTecnica.Consumer:**
 - **Home.AvaliacaoTecnica.ProcessorService:**
   
 Sem essa configuração, o projeto não será capaz de se conectar à fila do Azure Service Bus e falhará na inicialização.
 
 ---

 ### ⚠️ Tópicos e Subscriptions usadas neste projeto 
 - tópico: pedidos - subscription: processador
 - tópico: pedidos-processados - subscription: sistemaB

 Os tópicos mencionados são utilizados respectivamente pela API, Processador Service e Consumer

 ---
## 🚀 Feature Flags
O projeto foi desenvolvido para permitir realizar o calculo do imposto vigente ou calculo do imposto da reforma tributaria,
mediante ativacao da feature flag, conforme detalhado logo abaixo:
- UsarReformaTributaria : false - calculo do imposto vigente
- UsarReformaTributaria : true - calculo do imposto regra reforma tributaria

 Esta feature flag deve ser configurada no arquivo appsettings.json no seguinte projeto:
- **Home.AvaliacaoTecnica.ProcessorService:**:

  "FeatureManagement": {
      "UsarReformaTributaria": true
  },

Conforme modelo arquitetural detalhado no diagrama C4 o processador service e responsavel pelo calculo e envio
do pedido com o imposto calculado para o servico externo B.

  --- 

### ⚠️ Configuração Opcional: Azure Application insights
1. Obtenha a connection string válida do Applications Insights.
2. No arquivo `appsettings.json` (ou o usado para o ambiente), adicione:

"ApplicationInsights": {
  "ConnectionString": "sua-connection-string-aqui"
}

A string de conexão do Applications Insights deverá ser inserida apenas no projeto:
 - **Home.AvaliacaoTecnica.WebApi:**

Sem essa configuração, os logs serão exibidos na console e registrados em um arquivo de log, mas não será registrado
no application Insights no Azure.

### Observação

As connections string para o Azure Service Bus e Application Insights serão fornecidas pelo Desenvolvedor.


### Passos para Execução

1. Clone o repositório:

   ```bash
   git clone https://github.com/amattosp/home.avaliacao.tecnica-main.git
   cd home.avaliacao.tecnica-main
   ```

2. Abra o projeto no Visual Studio:

   ```bash
   Home.AvaliacaoTecnica.sln
   ```

3. Restaure as dependências:

   * No Visual Studio, clique com o botão direito na solução e selecione "Restaurar pacotes NuGet".

4. Execute o projeto:

   * No Visual Studio, pressione `F5` para iniciar a aplicação.

---

## 🧪 Como Executar os Testes

Para garantir que todas as funcionalidades estão funcionando corretamente:

1. Abra o projeto no Visual Studio.
2. No menu superior, clique em "Testar" > "Executar todos os testes" ou pressione `Ctrl + R, A`.

Isso executará todos os testes automatizados presentes no projeto, garantindo a integridade do código.
Foram criados também teste de integração, usando Test Container, para avaliar o comportamento da API de consulta de pedido por status e por Id.

**Sugestão:** Primeiro executar os testes unitários e depois os testes de integração, 

devido a montagem do banco pelo test container ser mais demorada. 

---

## 🧪 Teste exploratório
**1. Processo para realizaçao de teste exploratorio envio de pedido para calculo do imposto**
1. Acesar o Postman ou Swagger da API de envio (Pedido-Producer) de pedidos endpoint: http://localhost:5093/api/pedidos Operação de Post
2. Enviar o request do pedido conforme contrato definido no requisto
3. Response:
{
  "pedidoId": 1,
  "status": "Criado"
}   
5. A API sera responsavel por enviar o pedido para o Work de Processamento (BackGroundService Processor Service) 
6. O Worker ira receber o pedido do topico pedidos, subscription processador
7. Ira calcular o valor do imposto de acordo com a feature flag UsarReformaTributaria 

   5.1 - false - ira calcular o valor vigente

   5.2 - true - ira calcular o valor com a regra da reforma tributaria
   
9. Ao final do calculo o pedido sera enviado para o topico pedidos-processados
10. O sistema B possui um Consumer (Pedido-Consumer) que estara ouvindo a fila de pedidos processados subscription sistemaB
11. O Consumer ira receber os pedidos processados com o imposto calculado, conforme feature flag do arquivo appsettings.json
12. O resultado do pedido recebido com o imposto serão exibidos na console de execução.

**2. Processo para realizacao de teste exploratorio consumo api de listar pedido por status** 
1. Enviar um pedido pelo postman ou pelo swagger conforme descrito no procedimento de envio de pedido
2. Acessar no swagger o endpoint http://localhost:5093/api/pedidos?status=Criado operação de GET
3. Digitar no parametro o status Criado
4. O response dever vir um respose similar ao abaixo:
   [
  {
    "pedidoId": 1,
    "clienteId": 1,
    "status": "Criado",
    "itens": [
      {
        "produtoId": 1001,
        "quantidade": 2,
        "valor": 52.7
      }
    ]
  }
]

**3. Processo para realizacao de teste exploratorio consumo api de listar pedido por Id** 
1. Enviar um pedido pelo postman ou pelo swagger conforme descrito no procedimento de envio de pedido para calculo do imposto
2. Acessar no swagger o endpoint http://localhost:5093/api/pedidos/id operação de GET
3. Digitar no parametro o Id 1, por exemplo
4. O response dever vir um respose similar ao pedido que foi enviado na etapa de envio de pedido para cálculo do imposto, similar a este contrato:
 {
  "id": 1,
  "pedidoId": 1,
  "clienteId": 1,
  "imposto": 0,
  "itens": [
    {
      "produtoId": 1001,
      "quantidade": 2,
      "valor": 52.7
    }
  ],
  "status": "Criado"
}

**OBSERVACOES** 
1. Foi utilizado uma Shared Access Policy de modo que os topicos pudessem ser consumidas por qualquer pessoa em qualquer ambiente
2. O correto seria usar o Azure KeyVault e conceder acessos usando Management Identity aos topicos
   Na minha atual subscriiption nao tenho acesso a uso de Azure KeyVault e Management Identity por isso nao foi possivel
   usar o Azure KeyVault com autenticação com Management Identity
3. Foi utilizado o banco EntityFrameWork In-memory   

## 🛠 Tecnologias Utilizadas

O projeto foi desenvolvido utilizando as seguintes tecnologias:

* .NET 8
* ASP.NET Core
* Entity Framework Core
* Docker (para containerização)
* XUnit (para testes automatizados)
* Test Containers (para testes de integração) 

---


