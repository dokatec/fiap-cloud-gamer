# 🎮 FIAP Cloud Games - Tech Challenge Fase 1

Este projeto é a entrega da Fase 1 do Tech Challenge da FIAP. A proposta é desenvolver uma API REST em .NET 8 para cadastro de usuários e biblioteca de jogos adquiridos, com foco em boas práticas, qualidade de software e estrutura para expansão futura.

## 📌 Funcionalidades Implementadas

- ✅ Cadastro de usuários com validação de e-mail e senha forte
- ✅ Autenticação via JWT com controle de acesso por **roles** (Usuário / Administrador)
- ✅ Cadastro e listagem de jogos
- ✅ Associação de jogos à biblioteca do usuário
- ✅ Cadastro de promoções
- ✅ Middleware de tratamento de erros e logs estruturados
- ✅ Documentação da API com Swagger
- ✅ Testes unitários com xUnit e aplicação de TDD/BDD

---

## 🧠 Tecnologias Utilizadas

- .NET 8
- Entity Framework Core
- SQL Server
- Swagger
- JWT (JSON Web Token)
- xUnit + Moq
- BDDfy (para testes comportamentais)
- FluentValidation
- Clean Architecture + DDD

---

## 🔐 Perfis de Acesso

- **Usuário Comum**: pode se autenticar e visualizar sua biblioteca de jogos.
- **Administrador**: pode criar jogos, promoções e gerenciar usuários.

---

## 🧪 Testes e Qualidade

- TDD aplicado ao módulo de autenticação (`AuthService`)
- BDD aplicado ao fluxo de cadastro de usuário com `BDDfy`
- Testes unitários com xUnit e mocks com Moq

---

## 🗂 Estrutura do Projeto

```plaintext
src/
├── FiapCloudGames.API            # Camada de apresentação (API Controllers, Middleware)
├── FiapCloudGames.Application   # DTOs, Services, Interfaces
├── FiapCloudGames.Domain        # Entidades, Enums, Interfaces de domínio
├── FiapCloudGames.Infrastructure# Repositórios, contexto EF Core, JWT service
├── FiapCloudGames.Tests         # Testes unitários (TDD/BDD)
