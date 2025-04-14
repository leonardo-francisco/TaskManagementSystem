# TaskManagement.API

API RESTful para gerenciamento de projetos, tarefas e colaboradores.

---

## 🔧 Tecnologias Utilizadas

- ASP.NET Core 9
- MongoDB
- Docker
- Scalar Open Api
- FluentValidation
- xUnit + Moq (testes)

---

## 🚀 Como Executar o Projeto via Docker

1. **Clonar o repositório**

```bash
git clone https://github.com/seu-usuario/TaskManagement.API.git
cd TaskManagement.API
```

## ▶️ Subindo com Docker (MongoDB + API)

Crie um arquivo docker-compose.yml (caso ainda não tenha):
```bash
version: '3.8'

services:
  mongo:
    image: mongo
    container_name: mongodb
    ports:
      - "27017:27017"
    volumes:
      - mongo-data:/data/db

  api:
    build: .
    container_name: taskmanagement-api
    ports:
      - "5000:80"
    depends_on:
      - mongo
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - MongoDb__ConnectionString=mongodb://mongo:27017
      - MongoDb__Database=TaskManagementDb

volumes:
  mongo-data:
```

Executar Docker Compose:
```bash
docker-compose up --build
```

## ▶️ Executar Manualmente (Sem Docker)

Ajuste a connection string no appsettings.Development.json:
```bash
"MongoDb": {
  "ConnectionString": "mongodb://localhost:27017",
  "Database": "TaskManagementDb"
}
```

Execute o projeto com:
```bash
dotnet build
dotnet run --project TaskManagement.API
```

🔍 Acessar Scalar

Após a execução, acesse:
```bash
http://localhost:5000/scalar
```


## 🔄 Fase 2: Refinamento — Perguntas para o PO

Durante a próxima etapa de melhorias, seriam importantes as seguintes perguntas:

💡 Regras de Prioridade: A regra de não permitir alteração de Priority é definitiva? Pode ser liberada com permissões específicas?

🧑‍🤝‍🧑 Perfis de Usuário: Haverá diferenciação de permissões entre usuários (ex: Admin, Gerente, Membro)?

📆 Histórico de Comentários: Deve ser possível editar ou excluir comentários após adicionados?

🔔 Notificações: O sistema deverá disparar notificações quando uma tarefa for atualizada ou atribuída?

📊 Métricas: Há interesse em relatórios de produtividade, tarefas por status, etc.?

🌍 Internacionalização: A aplicação precisará suportar múltiplos idiomas no futuro?

🔒 Segurança: O sistema deverá ter autenticação/autorizacão via JWT ou outro provedor?

📱 Aplicação mobile/web: Existe plano de criar front-end com Angular, React ou app mobile?


## 🚀 Fase 3: Melhorias e Visão Arquitetural

Abaixo, sugestões para evolução do projeto:

🔧 Melhorias Técnicas

🧱 Arquitetura

✅ Separacão clara de camadas (Domain, Application, Infra)

✅ Uso de DTOs para isolar o modelo da API


## Sugestões:
☁️ Visão Cloud / DevOps


📄 Licença

MIT © Leonardo da Silva Francisco
