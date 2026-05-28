# 02 — Passo a passo de configuração do ambiente

Este documento é focado apenas em **instalar, configurar e rodar** o projeto localmente.

Escopo mantido:

- Sem Docker.
- Sem NGINX.
- Sem autenticação.
- Sem SignalR/Hub.
- Apenas um CRUD: `Schedule`.

## 1. Portas usadas

```txt
Angular:         http://localhost:4200
ASP.NET Core:    http://localhost:5000
PostgreSQL:      localhost:5432
Swagger:         http://localhost:5000/swagger
```

## 2. Pré-requisitos

Instale:

```txt
1. .NET SDK 10 LTS
2. Node.js em versão LTS ativa ou de manutenção
3. Angular CLI
4. PostgreSQL local
5. VS Code
6. Git
7. Extensão REST Client no VS Code, opcional
8. Postman, opcional
```

Verifique no terminal:

```bash
dotnet --version
node --version
npm --version
git --version
psql --version
```

## 3. Instalar Angular CLI

```bash
npm install -g @angular/cli
```

Verifique:

```bash
ng version
```

## 4. Criar banco PostgreSQL local

### 4.1. Usando `createdb`

```bash
createdb -h localhost -p 5432 -U postgres employee_schedule_db
```

### 4.2. Usando `psql`

```bash
psql -h localhost -p 5432 -U postgres
```

Dentro do `psql`:

```sql
CREATE DATABASE employee_schedule_db;
```

Verifique a lista de bancos:

```sql
\l
```

Saia:

```sql
\q
```

## 5. Configurar connection string da API

Arquivo:

```txt
EmployeeSchedule.Api/appsettings.json
```

Exemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=employee_schedule_db;Username=postgres;Password=postgres"
  },
  "Frontend": {
    "AllowedOrigins": [
      "http://localhost:4200"
    ]
  }
}
```

Altere:

```txt
Username=postgres
Password=postgres
```

para o usuário e senha reais do seu PostgreSQL local.

## 6. Configurar CORS

O Angular roda em uma origem diferente da API:

```txt
Angular: http://localhost:4200
API:     http://localhost:5000
```

Por isso a API precisa permitir a origem do Angular.

Arquivo:

```txt
EmployeeSchedule.Api/Config/CorsConfig.cs
```

Configuração usada:

```csharp
policy
    .WithOrigins(allowedOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod();
```

Origem configurada no `appsettings.json`:

```json
"Frontend": {
  "AllowedOrigins": [
    "http://localhost:4200"
  ]
}
```

No `Program.cs`, a ordem fica assim:

```csharp
app.UseRouting();
app.UseCors(CorsConfig.AllowAngularPolicy);
app.UseAuthorization();
app.MapControllers();
```

## 7. Instalar pacotes NuGet da API

Entre na pasta da API:

```bash
cd EmployeeSchedule.Api
```

Instale:

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.8
dotnet add package Swashbuckle.AspNetCore --version 10.1.7
```

Finalidade dos pacotes:

```txt
Npgsql.EntityFrameworkCore.PostgreSQL
Permite usar PostgreSQL com Entity Framework Core.

Microsoft.EntityFrameworkCore.Design
Habilita recursos de design-time, incluindo migrations via dotnet ef.

Swashbuckle.AspNetCore
Gera Swagger UI para testar a API no navegador.
```

## 8. Instalar ferramenta EF Core CLI

```bash
dotnet tool install --global dotnet-ef --version 10.0.8
```

Se já estiver instalada:

```bash
dotnet tool update --global dotnet-ef --version 10.0.8
```

Verifique:

```bash
dotnet ef --version
```

## 9. Restaurar e compilar backend

Dentro da pasta `EmployeeSchedule.Api`:

```bash
dotnet restore
dotnet build
```

## 10. Criar migration

```bash
dotnet ef migrations add InitialCreate
```

Isso cria arquivos dentro da pasta:

```txt
EmployeeSchedule.Api/Migrations
```

## 11. Atualizar banco

```bash
dotnet ef database update
```

Esse comando aplica a migration no banco `employee_schedule_db`.

## 12. Confirmar tabela no PostgreSQL

Entre no banco:

```bash
psql -h localhost -p 5432 -U postgres -d employee_schedule_db
```

Liste as tabelas:

```sql
\dt
```

Você deve ver:

```txt
schedules
```

Veja a estrutura:

```sql
\d schedules
```

Saia:

```sql
\q
```

## 13. Rodar backend

Dentro da pasta `EmployeeSchedule.Api`:

```bash
dotnet run --urls "http://localhost:5000"
```

A API deve ficar acessível em:

```txt
http://localhost:5000
```

Swagger:

```txt
http://localhost:5000/swagger
```

## 14. Testar backend pelo Swagger

Abra:

```txt
http://localhost:5000/swagger
```

Teste primeiro:

```txt
POST /api/schedules
```

Body:

```json
{
  "employeeName": "Ana Souza",
  "employeeRegistration": "EMP001",
  "department": "Operations",
  "shiftName": "Morning Shift",
  "startTime": "08:00:00",
  "endTime": "17:00:00",
  "workDate": "2026-05-28",
  "status": "Scheduled",
  "notes": "Teste pelo Swagger."
}
```

Depois teste:

```txt
GET /api/schedules
GET /api/schedules/1
PUT /api/schedules/1
DELETE /api/schedules/1
```

## 15. Testar backend pelo REST Client

Na raiz da solution, crie:

```txt
requests.http
```

Conteúdo:

```http
### Listar
GET http://localhost:5000/api/schedules

### Criar
POST http://localhost:5000/api/schedules
Content-Type: application/json

{
  "employeeName": "Ana Souza",
  "employeeRegistration": "EMP001",
  "department": "Operations",
  "shiftName": "Morning Shift",
  "startTime": "08:00:00",
  "endTime": "17:00:00",
  "workDate": "2026-05-28",
  "status": "Scheduled",
  "notes": "Criado pelo REST Client."
}

### Buscar por id
GET http://localhost:5000/api/schedules/1

### Atualizar
PUT http://localhost:5000/api/schedules/1
Content-Type: application/json

{
  "employeeName": "Ana Souza",
  "employeeRegistration": "EMP001",
  "department": "Operations",
  "shiftName": "Afternoon Shift",
  "startTime": "13:00:00",
  "endTime": "21:00:00",
  "workDate": "2026-05-29",
  "status": "Completed",
  "notes": "Atualizado pelo REST Client."
}

### Excluir
DELETE http://localhost:5000/api/schedules/1
```

## 16. Configurar frontend Angular

Arquivo:

```txt
employee-schedule-web/src/environments/environment.ts
```

Conteúdo:

```ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

A URL final usada pelo Angular será:

```txt
http://localhost:5000/api/schedules
```

## 17. Instalar dependências do frontend

Dentro da pasta do Angular:

```bash
cd employee-schedule-web
npm install
```

## 18. Rodar frontend

```bash
ng serve
```

Acesse:

```txt
http://localhost:4200
```

## 19. Ordem correta para rodar tudo

Terminal 1:

```bash
cd EmployeeSchedule.Api
dotnet run --urls "http://localhost:5000"
```

Terminal 2:

```bash
cd employee-schedule-web
ng serve
```

Navegador:

```txt
http://localhost:4200
```

## 20. Erros comuns e correções

### 20.1. Erro de conexão com PostgreSQL

Mensagem comum:

```txt
password authentication failed
```

Verifique:

```txt
Username
Password
Database
Port
```

no `appsettings.json`.

### 20.2. Banco não existe

Mensagem comum:

```txt
database "employee_schedule_db" does not exist
```

Crie o banco:

```bash
createdb -h localhost -p 5432 -U postgres employee_schedule_db
```

### 20.3. `dotnet ef` não reconhecido

Instale a ferramenta:

```bash
dotnet tool install --global dotnet-ef --version 10.0.8
```

Feche e abra o terminal. Depois rode:

```bash
dotnet ef --version
```

### 20.4. Erro de CORS no navegador

Mensagem comum no console:

```txt
Access to XMLHttpRequest at ... has been blocked by CORS policy
```

Verifique:

```txt
1. API está rodando em http://localhost:5000
2. Angular está rodando em http://localhost:4200
3. appsettings.json tem http://localhost:4200 em Frontend:AllowedOrigins
4. Program.cs chama app.UseCors antes de app.UseAuthorization
```

### 20.5. Swagger não abre

Verifique se você instalou:

```bash
dotnet add package Swashbuckle.AspNetCore --version 10.1.7
```

E se o `Program.cs` chama:

```csharp
builder.Services.AddSwaggerDocumentation();
app.UseSwaggerDocumentation();
```

### 20.6. Angular não encontra `environment`

Confira o caminho no service:

```ts
import { environment } from '../../../../environments/environment';
```

O arquivo deve existir em:

```txt
src/environments/environment.ts
```

### 20.7. Erro com comando `ng new`

Atualize Angular CLI:

```bash
npm install -g @angular/cli
```

Depois verifique:

```bash
ng version
```

## 21. Checklist de configuração

```txt
[ ] .NET SDK instalado
[ ] Node.js e npm instalados
[ ] Angular CLI instalado
[ ] PostgreSQL instalado e rodando
[ ] Banco employee_schedule_db criado
[ ] Connection string configurada
[ ] Pacotes NuGet instalados
[ ] dotnet-ef instalado
[ ] dotnet build sem erro
[ ] Migration criada
[ ] Database update executado
[ ] API rodando na porta 5000
[ ] Swagger abrindo
[ ] Angular environment apontando para http://localhost:5000/api
[ ] Angular rodando na porta 4200
[ ] CORS funcionando
```

## 22. Referências oficiais usadas

- .NET Support Policy: https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core
- EF Core CLI: https://learn.microsoft.com/en-us/ef/core/cli/dotnet
- Npgsql EF Core Provider: https://www.npgsql.org/efcore/
- ASP.NET Core CORS: https://learn.microsoft.com/en-us/aspnet/core/security/cors
- Angular CLI setup: https://angular.dev/tools/cli/setup-local
- Angular HttpClient setup: https://angular.dev/guide/http/setup
- PostgreSQL CREATE DATABASE: https://www.postgresql.org/docs/current/sql-createdatabase.html
