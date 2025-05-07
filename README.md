---

# 🏠 Home.AvaliacaoTecnica

Este repositório contém a implementação de uma avaliação técnica para uma oportunidade de emprego. A versão inicial do projeto está estruturada utilizando o modelo Git Flow, com branches específicas para desenvolvimento e funcionalidades.

---

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
2. No arquivo `appsettings.Development.json` (ou o usado para o ambiente), adicione:

"AzureServiceBus": {
  "ConnectionString": "sua-connection-string-aqui"
 }
 
 Sem essa configuração, o projeto não será capaz de se conectar à fila do Azure Service Bus e falhará na inicialização.
 
 ---

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

---


