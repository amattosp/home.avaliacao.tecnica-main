# API Envio de Pedidos 

Esta API e responsavel por enviar o pedidos do Sistema A para o Work Service que sera responsavel por receber o pedido e realizar o calculo de imposto 
apos calculo realizado envia o resultado do calculo para o sistema B.
Foi utilizado um BackGroundService, mas poderia ser substituido por um MessageRelay ou Event Hub que ficaria escutando
o topico para processamento e calculo dos pedidos.

# Versão 1.0.0

Esta e a versao inicial da API de envio de pedidos 


# Contribuindo

Contribuições são bem-vindas! Se você encontrou um bug ou tem uma ideia para uma nova funcionalidade, sinta-se à vontade para abrir uma issue ou enviar um pull request.

> **IMPORTANTE**  
>  
> Todo desenvolvimento de nova feature ou correção de bug deve ser feito através da criação de uma nova branch a partir da branch **main**.
>  
> Em todo pull request, no momento de sua conclusão, deve ser marcada a opção **delete source branch**, a fim de garantir que o repositório não fique poluído com branches antigas de desenvolvimentos já concluídos.

# Processo para realizaçao de teste exploratorio envio de pedido para calculo do imposto
1. Acesar o Postman ou Swagger da aplicacao Pedido.Producer endpoint: http://localhost:5093/api/pedidos/enviar
2. Enviar o seguinte request: 
{
  "pedidoId": 1,
  "clienteId": 1,
  "itens": [
    {
      "produtoId": 1001,
      "quantidade": 2,
      "valor": 52.70
    }
  ]
}

3. A API sera responsavel por enviar o pedido para o Work de Processamento (BackGroundService) 
4. O Worker ira receber o pedido do topico pedidos, subscription processador
5. Ira calcular o valor do imposto de acordo com a feature flag UsarReformaTributaria 
   5.1 - false - ira calcular o valor vigente
   5.2 - true - ira calcular o valor com a regra da reforma tributaria
6. Ao final do calculo o pedido sera enviado para o topico pedidos-processados
7. O sistema B possui um Consumer (Pedido.Consumer) que estara ouvindo a fila de pedidos processados subscription sistemaB
8. O Consumer ira receber os pedidos processados com o imposto calculado, conforme feature flag do arquivo appsettings.json

**OBSERVACOES** 
1. Foi utilizado uma Shared Access Policy de modo que os topicos pudessem ser consumidas por qualquer pessoa em qualquer ambiente
2. O correto seria usar o Azure KeyVault e conceder acessos usando Management Identity aos topicos
   Na minha atual subscriiption nao tenho acesso a uso de Azure KeyVault e Management Identity por isso nao foi possivel
   usar o Azure KeyVault com autenticao com Management Identity

# Processo para realizacao de teste exploratorio consumo api de listar pedido por status 
1. Enviar um pedido pelo postman ou pelo swagger conforme descrito no procedimento de envio de pedido
2. Acessar no swagger o endpoint http://localhost:5093/api/pedidos
3. Digitar o no parametro o status Criado
4. O response dever vir um respose similar ao abaixo, sendo que a data de envio sera a data vigente no momento do teste exploratorio
   [
  {
    "pedidoId": 1,
    "clienteId": 1,
    "status": "Criado",
    "enviadoEm": "2025-05-06T04:54:44.1955764Z",
    "itens": [
      {
        "produtoId": 1001,
        "quantidade": 2,
        "valor": 52.7
      }
    ]
  }
]
5. **OBSERVACAO** Foi utilizado o banco EntityFrameWork In-memory 

> **IMPORTANTE** 
>
>A etapa de criação de tags é crucial no processo de versionamento e release de software. Para garantir a qualidade e a rastreabilidade do código, é fundamental que as tags sejam criadas somente após a conclusão bem-sucedida de Pull Requests (PRs) para a branch main.

# Convenções de Tag
- Versões Oficiais: Seguem a semântica de versionamento SemVer (Major.Minor.Patch). Exemplo: 1.0.2
- Release Candidates: Utilizam o formato Major.Minor.Patch-rcN, onde N representa um número incremental para cada iteração do release candidate. Exemplo: 2.0.0-rc1, 2.0.0-rc2

