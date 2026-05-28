# Diagrama de sequência — CRUD Schedule

```mermaid
sequenceDiagram
    autonumber
    actor User as Usuário
    participant Angular as Angular Frontend<br/>Component + Service HTTP
    participant Api as ASP.NET Core API<br/>Controller + Service
    participant RepoEf as Repository + EF Core<br/>AppDbContext + Npgsql
    database Pg as PostgreSQL local

    User->>Angular: Acessa listagem ou envia formulário do Schedule
    Angular->>Angular: Valida formulário e chama o service HTTP
    Angular->>Api: HTTP GET/POST/PUT/DELETE /api/schedules
    Api->>Api: Controller recebe; Service valida regras e mapeia DTO/Entity
    Api->>RepoEf: Chama repository para consultar ou salvar dados
    RepoEf->>Pg: SQL SELECT/INSERT/UPDATE/DELETE em schedules
    Pg-->>RepoEf: Resultado do banco
    RepoEf-->>Api: Entity, lista ou confirmação
    Api-->>Angular: HTTP 200/201/204 ou 400/404 + JSON
    Angular-->>User: Atualiza tabela, redireciona ou mostra erro
```
