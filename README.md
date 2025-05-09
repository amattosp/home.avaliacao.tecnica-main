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

---

## 🛠 Tecnologias Utilizadas

O projeto foi desenvolvido utilizando as seguintes tecnologias:

* .NET 8
* ASP.NET Core
* Entity Framework Core
* Docker (para containerização)
* XUnit (para testes automatizados)
* Test Containers (para testes de integração) 

---


