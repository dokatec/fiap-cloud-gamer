# 🎮 FIAP Cloud Games - Tech Challenge Fase 2

Este projeto é a entrega da Fase 2 do Tech Challenge da FIAP. A proposta é garantir que a plataforma seja
escalável, confiável e monitorável. Foi estruturado para aplicar os conhecimentos
adquiridos nas disciplinas que vimos, como CI/CD, Docker, Azure DevOps, AWS
e ferramentas de monitoramento.

## 📌 Funcionalidades Implementadas

- ✅ Garantir escalabilidade e resiliência da aplicação
- ✅ Dockerizar a aplicação
- ✅ Monitorar a aplicação
- ✅ Automatizar a entrega com CI/CD

---

## 🧠 Tecnologias Utilizadas

- Github
- Github Actions
- Docker
- Docker Hub
- AWS (ECS, EBS, Load Balancer, Auto Scaling)

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
```
