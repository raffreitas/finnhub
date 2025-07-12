# 🏦 FinnHub - Sistema de Gestão de Portfólios Financeiros

[![.NET](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Compose-blue)](https://docs.docker.com/compose/)
[![Architecture](https://img.shields.io/badge/Architecture-Microservices-green)](https://microservices.io/)
[![License](https://img.shields.io/badge/License-MIT-yellow)](LICENSE)

FinnHub é uma plataforma completa de gestão de portfólios financeiros desenvolvida em .NET 9, seguindo princípios de **Domain-Driven Design (DDD)**, **Clean Architecture** e **Microservices**. O sistema permite aos usuários gerenciar investimentos, acompanhar cotações de mercado e analisar performance de portfólios em tempo real.

## 🚀 Funcionalidades Principais

### 📊 Gestão de Portfólios
- **Criação e gestão de portfólios personalizados**
- **Compra e venda de ativos** com controle de posições
- **Cálculo automático de custo médio** e valor de mercado
- **Histórico completo de transações**
- **Dashboard com métricas de performance**

### 📈 Dados de Mercado
- **Integração com APIs de cotações** em tempo real
- **Ingestão de dados** via workers especializados
- **Armazenamento otimizado** com MongoDB
- **Processamento assíncrono** de market data

### 🔐 Segurança e Autenticação
- **Autenticação JWT** robusta
- **Autorização baseada em roles**
- **Validação de dados** em todas as camadas
- **Rate limiting** e proteção contra ataques

## 🏗️ Arquitetura

O sistema segue uma arquitetura de **microservices** com **bounded contexts** bem definidos:

```
src/
├── building-blocks/          # Componentes compartilhados
│   ├── FinnHub.Shared.Core/     # Types e abstrações principais
│   ├── FinnHub.Shared.Kernel/   # Domain building blocks
│   └── FinnHub.Shared.Infrastructure/ # Extensões e utilitários
├── contexts/
│   ├── market-data/         # Contexto de dados de mercado
│   │   ├── FinnHub.MarketData.Ingestion/    # Worker de ingestão
│   │   ├── FinnHub.MarketData.Processor/    # Worker de processamento
│   │   └── FinnHub.MarketData.WebApi/       # API de cotações
│   └── portfolio-management/ # Contexto de gestão de portfólios
│       ├── FinnHub.PortfolioManagement.Application/    # Casos de uso
│       ├── FinnHub.PortfolioManagement.Domain/         # Lógica de domínio
│       ├── FinnHub.PortfolioManagement.Infrastructure/ # Infraestrutura
│       ├── FinnHub.PortfolioManagement.WebApi/         # API REST
│       └── FinnHub.PortfolioManagement.Worker/         # Workers
```

### 🔧 Stack Tecnológica

| Componente | Tecnologia |
|------------|------------|
| **Runtime** | .NET 9.0 |
| **Banco Relacional** | PostgreSQL |
| **Banco NoSQL** | MongoDB |
| **Cache** | Redis |
| **Message Broker** | RabbitMQ |
| **Observabilidade** | OpenTelemetry + Elastic Stack |
| **Containerização** | Docker & Docker Compose |
| **Testes** | xUnit + Testcontainers |

## 🛠️ Configuração do Ambiente

### Pré-requisitos
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

### 🐳 Executando com Docker

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/raffreitas/finnhub.git
   cd finnhub
   ```

2. **Inicie a infraestrutura:**
   ```bash
   docker-compose up -d
   ```

3. **Verifique os serviços:**
   ```bash
   docker-compose ps
   ```

### 🔧 Desenvolvimento Local

1. **Restaure as dependências:**
   ```bash
   dotnet restore
   ```

2. **Configure as connection strings** nos arquivos `appsettings.Development.json`

3. **Execute as migrações:**
   ```bash
   cd src/contexts/portfolio-management/src/FinnHub.PortfolioManagement.WebApi
   dotnet ef database update
   ```

4. **Execute os projetos:**
   ```bash
   # Portfolio Management API
   cd src/contexts/portfolio-management/src/FinnHub.PortfolioManagement.WebApi
   dotnet run

   # Market Data API
   cd src/contexts/market-data/src/FinnHub.MarketData.WebApi
   dotnet run
   ```

## 📋 APIs Disponíveis

### 🏦 Portfolio Management API
**Base URL:** `https://localhost:7001`

| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/api/v1/portfolios` | GET | Lista portfólios do usuário |
| `/api/v1/portfolios` | POST | Cria novo portfólio |
| `/api/v1/portfolios/{id}` | GET | Detalhes do portfólio |
| `/api/v1/portfolios/{id}/buy` | POST | Registra compra de ativo |
| `/api/v1/portfolios/{id}/sell` | POST | Registra venda de ativo |
| `/api/v1/portfolios/{id}/transactions` | GET | Histórico de transações |

### 📊 Market Data API
**Base URL:** `https://localhost:7113`

| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/api/v1/quotes/{symbol}` | GET | Cotação atual do ativo |
| `/api/v1/assets` | GET | Lista de ativos disponíveis |
| `/api/v1/assets` | POST | Cadastra novo ativo |

## 🧪 Executando Testes

```bash
# Testes unitários
dotnet test src/contexts/portfolio-management/test/FinnHub.PortfolioManagement.UnitTests/

# Testes de integração
dotnet test src/contexts/portfolio-management/test/FinnHub.PortfolioManagement.IntegrationTests/

# Todos os testes
dotnet test
```

## 📊 Monitoramento

O sistema inclui observabilidade completa com:

- **📈 Métricas:** Expostas via OpenTelemetry
- **🔍 Logs:** Centralizados no Elasticsearch
- **📊 Dashboards:** Disponíveis no Kibana (`http://localhost:5601`)
- **🚨 APM:** Monitoramento de performance (`http://localhost:8200`)

## 🎯 Principais Padrões Implementados

- **🏛️ Clean Architecture:** Separação clara de responsabilidades
- **📦 Domain-Driven Design:** Modelagem rica de domínio
- **🔄 CQRS:** Separação de comandos e queries
- **📝 Event Sourcing:** Para auditoria de transações
- **🔧 Repository Pattern:** Abstração de acesso a dados
- **🚀 Unit of Work:** Controle transacional
- **🎯 Mediator Pattern:** Desacoplamento via MediatR
- **🛡️ Result Pattern:** Tratamento de erros funcionais

## 🔐 Configuração de Segurança

### JWT Token
```json
{
  "AuthenticationSettings": {
    "JwtSecret": "your-super-secret-key-here",
    "JwtExpiration": 2
  }
}
```

### Variáveis de Ambiente
```bash
JWT_SECRET=your-jwt-secret
DB_CONNECTION_STRING=your-db-connection
REDIS_CONNECTION_STRING=your-redis-connection
RABBITMQ_CONNECTION_STRING=your-rabbitmq-connection
```

## 🤝 Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

## 📜 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

---

⭐ **Se este projeto foi útil para você, considere dar uma estrela!** ⭐