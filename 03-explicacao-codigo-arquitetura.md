# 03 — Explicação do código e da arquitetura

Este documento explica a arquitetura usada no projeto **Employee Schedule** e o papel de cada arquivo.

O projeto foi desenhado como um MVP didático: simples o bastante para aprender o fluxo completo, mas organizado o suficiente para não virar um arquivo único cheio de responsabilidades misturadas.

## 1. Decisões principais

### 1.1. O que entra no MVP

```txt
API REST com controllers
Entity Framework Core
PostgreSQL local
Npgsql
Swagger
Angular standalone
HttpClient
Reactive Forms
DTOs
Repository Pattern simples
Service Layer
CORS local
Semantic Commits e Semantic Branches
```

### 1.2. O que fica fora do MVP

```txt
Docker
NGINX
Autenticação/autorização real
SignalR/Hub
Múltiplos CRUDs
Microserviços
CQRS
MediatR
Clean Architecture completa
DDD completo
Testes automatizados obrigatórios
Deploy em produção
```

Esses itens podem ser adicionados depois, mas entrariam cedo demais para o objetivo atual: aprender o fluxo completo com apenas o CRUD `Schedule`.

## 2. Visão da arquitetura

```txt
Frontend Angular
  Componentes de página
  Services HTTP
  Models TypeScript

Backend ASP.NET Core
  Controllers
  Services
  Repositories
  DTOs
  Entities
  DbContext

Banco PostgreSQL
  Tabela schedules
```

Fluxo de leitura:

```txt
ScheduleListComponent
  -> ScheduleService Angular
    -> GET http://localhost:5000/api/schedules
      -> ScheduleController.GetAll
        -> ScheduleService.GetAllAsync
          -> ScheduleRepository.GetAllAsync
            -> AppDbContext.Schedules
              -> PostgreSQL
```

Fluxo de criação:

```txt
ScheduleFormComponent
  -> ScheduleService Angular
    -> POST http://localhost:5000/api/schedules
      -> ScheduleController.Create
        -> ScheduleService.CreateAsync
          -> valida dados
          -> converte DTO para Entity
          -> define CreatedAt
          -> ScheduleRepository.CreateAsync
            -> AppDbContext.SaveChangesAsync
              -> INSERT na tabela schedules
```

Fluxo de edição:

```txt
ScheduleFormComponent
  -> PUT /api/schedules/{id}
    -> Controller
      -> Service
        -> busca entidade existente
        -> valida dados
        -> atualiza campos
        -> define UpdatedAt
        -> Repository
          -> SaveChangesAsync
```

Fluxo de exclusão:

```txt
ScheduleListComponent
  -> DELETE /api/schedules/{id}
    -> Controller
      -> Service
        -> Repository
          -> Remove + SaveChangesAsync
```

## 3. Backend em camadas

## 3.1. `Program.cs`

O `Program.cs` é o ponto de entrada da API.

Ele faz quatro coisas principais:

```txt
1. Registra controllers e configura JSON.
2. Registra CORS.
3. Registra serviços da aplicação e banco.
4. Configura middleware e mapeia controllers.
```

Trecho principal:

```csharp
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
```

Esse trecho faz com que o enum `ScheduleStatus` apareça como string no JSON:

```json
"status": "Scheduled"
```

em vez de:

```json
"status": 1
```

Isso deixa a API mais legível e compatível com o Angular.

## 3.2. Pasta `Config`

A pasta `Config` separa configurações que poderiam deixar o `Program.cs` grande demais.

### `CorsConfig.cs`

Responsável por permitir que o Angular acesse a API.

A API roda em:

```txt
http://localhost:5000
```

O Angular roda em:

```txt
http://localhost:4200
```

Como são origens diferentes, o navegador bloqueia a chamada se a API não liberar CORS.

### `DependencyInjectionConfig.cs`

Responsável por registrar dependências:

```txt
AppDbContext
IScheduleRepository -> ScheduleRepository
IScheduleService -> ScheduleService
```

Isso aplica inversão de dependência de forma simples: controller depende da interface `IScheduleService`, não diretamente da classe concreta `ScheduleService`.

### `SwaggerConfig.cs`

Responsável por habilitar Swagger no ambiente de desenvolvimento.

Com isso, você testa a API em:

```txt
http://localhost:5000/swagger
```

## 3.3. Pasta `Context`

### `AppDbContext.cs`

O `AppDbContext` é a ponte entre C# e PostgreSQL.

Ele expõe:

```csharp
public DbSet<Schedule> Schedules => Set<Schedule>();
```

Isso representa a tabela `schedules`.

Também configura nomes de tabela e colunas:

```csharp
entity.ToTable("schedules");

entity.Property(schedule => schedule.EmployeeName)
    .HasColumnName("employee_name")
    .HasMaxLength(150)
    .IsRequired();
```

Essa escolha mantém:

```txt
C# com PascalCase: EmployeeName
PostgreSQL com snake_case: employee_name
```

## 3.4. Pasta `Entity`

### `Schedule.cs`

Representa a entidade principal do sistema.

```csharp
public class Schedule
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeRegistration { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string ShiftName { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly WorkDate { get; set; }
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

Ela não deve receber lógica de HTTP, JSON ou tela. Ela representa o dado persistido.

### `ScheduleStatus.cs`

Define os status possíveis:

```csharp
public enum ScheduleStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    Absent = 4
}
```

Usar enum evita strings soltas no backend, como:

```txt
"Schedulled"
"Canceled"
"Cancelado"
```

Com enum, o C# ajuda a restringir os valores possíveis.

## 3.5. Pasta `Dtos`

DTO significa **Data Transfer Object**.

DTO não é tabela. DTO é contrato de entrada ou saída da API.

### `CreateScheduleDto`

Usado no `POST /api/schedules`.

Contém os dados necessários para criar uma escala.

### `UpdateScheduleDto`

Usado no `PUT /api/schedules/{id}`.

Contém os dados necessários para atualizar uma escala.

### `ScheduleResponseDto`

Usado nas respostas da API.

Ele evita expor diretamente a entidade `Schedule`.

Benefícios dos DTOs:

```txt
1. Evitam acoplamento direto entre banco e API.
2. Permitem mudar a entidade sem quebrar o contrato externo.
3. Permitem formatar datas e horários para o frontend.
4. Permitem ter DTOs diferentes para criação, atualização e resposta.
```

## 3.6. Pasta `Repository`

O repository centraliza acesso ao banco.

### `IScheduleRepository.cs`

Define o contrato:

```csharp
Task<List<Schedule>> GetAllAsync();
Task<Schedule?> GetByIdAsync(int id);
Task<Schedule?> GetTrackedByIdAsync(int id);
Task<Schedule> CreateAsync(Schedule schedule);
Task UpdateAsync(Schedule schedule);
Task DeleteAsync(Schedule schedule);
```

### `ScheduleRepository.cs`

Implementa o contrato usando EF Core.

Exemplo:

```csharp
return await _context.Schedules
    .AsNoTracking()
    .OrderBy(schedule => schedule.WorkDate)
    .ThenBy(schedule => schedule.StartTime)
    .ToListAsync();
```

`AsNoTracking()` é usado em consultas de leitura para evitar tracking desnecessário do EF Core.

Para edição e exclusão, o projeto usa:

```csharp
GetTrackedByIdAsync
```

porque a entidade precisa ser alterada ou removida.

## 3.7. Pasta `Services`

A camada de service guarda regras de negócio.

### `IScheduleService.cs`

Contrato consumido pelo controller.

### `ScheduleService.cs`

Aplica as regras:

```txt
employeeName obrigatório
employeeRegistration obrigatório
employeeRegistration não pode ser vazio
Department obrigatório
shiftName obrigatório
startTime obrigatório
endTime obrigatório
workDate obrigatório
status obrigatório
endTime não pode ser igual a startTime
createdAt preenchido ao criar
updatedAt atualizado ao editar
```

Exemplo:

```csharp
if (endTime == startTime)
{
    throw new ArgumentException("endTime não pode ser igual a startTime.");
}
```

O projeto permite `endTime` menor que `startTime` para não bloquear escalas noturnas. A regra pedida só impede igualdade.

## 3.8. `ScheduleController.cs`

O controller é a borda HTTP da API.

Ele define rotas:

```csharp
[Route("api/schedules")]
```

E actions:

```txt
GET     /api/schedules
GET     /api/schedules/{id}
POST    /api/schedules
PUT     /api/schedules/{id}
DELETE  /api/schedules/{id}
```

O controller não acessa o `DbContext` diretamente. Ele chama o service:

```csharp
private readonly IScheduleService _scheduleService;
```

Isso mantém responsabilidades separadas.

## 4. Banco de dados

A tabela gerada pela migration será parecida com:

```txt
schedules
- id
- employee_name
- employee_registration
- department
- shift_name
- start_time
- end_time
- work_date
- status
- notes
- created_at
- updated_at
```

Tipos conceituais:

```txt
id                       inteiro / chave primária
employee_name            texto obrigatório
employee_registration    texto obrigatório
Department               texto obrigatório
shift_name               texto obrigatório
start_time               horário obrigatório
end_time                 horário obrigatório
work_date                data obrigatória
status                   texto obrigatório
notes                    texto opcional
created_at               timestamp obrigatório
updated_at               timestamp opcional
```

## 5. Migrations

Migration é a forma de versionar o banco pelo código.

Comando para criar:

```bash
dotnet ef migrations add InitialCreate
```

Comando para aplicar:

```bash
dotnet ef database update
```

A migration lê o `AppDbContext` e cria/atualiza a tabela no PostgreSQL.

## 6. Frontend Angular

## 6.1. Estrutura por feature

A feature `schedules` fica em:

```txt
src/app/modules/schedules
```

Ela contém:

```txt
models
services
pages
```

Isso evita jogar todos os arquivos diretamente em `app`.

## 6.2. `schedule.model.ts`

Define os tipos TypeScript usados pelo frontend.

```ts
export type ScheduleStatus = 'Scheduled' | 'Completed' | 'Cancelled' | 'Absent';
```

Esse tipo espelha o enum do backend.

A interface `Schedule` representa a resposta da API.

A interface `CreateScheduleRequest` representa o corpo enviado no POST e PUT.

## 6.3. `schedule.service.ts`

É o serviço HTTP do Angular.

Ele centraliza as chamadas:

```ts
getAll(): Observable<Schedule[]> {
  return this.http.get<Schedule[]>(this.apiUrl);
}
```

O componente não precisa saber a URL da API. Ele chama métodos do service.

Benefícios:

```txt
1. Componentes ficam mais limpos.
2. URL da API fica centralizada.
3. Trocar endpoint depois fica mais fácil.
4. Tipos TypeScript protegem o consumo da API.
```

## 6.4. `app.config.ts`

Configura providers globais:

```ts
provideRouter(routes),
provideHttpClient()
```

Sem `provideHttpClient()`, o Angular não consegue injetar `HttpClient` no `ScheduleService`.

## 6.5. `app.routes.ts`

Define a navegação:

```txt
/schedules           lista escalas
/schedules/new       cria escala
/schedules/:id/edit  edita escala
```

A rota raiz redireciona para `/schedules`.

## 6.6. `MainLayoutComponent`

Fornece uma estrutura visual comum:

```txt
Header
Menu
Área principal com router-outlet
```

O `router-outlet` dentro do layout recebe as páginas de schedules.

## 6.7. `ScheduleListComponent`

Responsável por:

```txt
1. Carregar schedules.
2. Mostrar tabela.
3. Mostrar estado de carregamento.
4. Mostrar erro simples.
5. Navegar para edição.
6. Excluir schedule.
```

Ele chama:

```ts
this.scheduleService.getAll()
```

E renderiza os dados na tabela.

## 6.8. `ScheduleFormComponent`

Responsável por criação e edição.

Ele decide se está em edição olhando a rota:

```ts
const idParam = this.route.snapshot.paramMap.get('id');
```

Se existe `id`, carrega o registro e faz `patchValue` no formulário.

O formulário usa Reactive Forms:

```ts
form = this.formBuilder.nonNullable.group({
  employeeName: ['', Validators.required],
  employeeRegistration: ['', Validators.required],
  department: ['', Validators.required],
  shiftName: ['', Validators.required],
  startTime: ['', Validators.required],
  endTime: ['', Validators.required],
  workDate: ['', Validators.required],
  status: ['Scheduled' as ScheduleStatus, Validators.required],
  notes: ['']
});
```

Antes de enviar, o frontend também valida:

```ts
if (request.startTime === request.endTime) {
  this.errorMessage = 'O horário final não pode ser igual ao horário inicial.';
  return;
}
```

A API valida de novo. Isso é importante porque validação de frontend melhora experiência, mas não protege a API sozinha.

## 7. Por que validar no frontend e no backend?

Frontend:

```txt
Melhora a experiência do usuário.
Mostra erro antes de enviar.
Reduz requisições inválidas.
```

Backend:

```txt
Protege a regra de negócio.
Garante consistência mesmo se alguém chamar a API por Postman/cURL.
Impede dados inválidos no banco.
```

A regra importante é: **nunca confie apenas no frontend**.

## 8. Como o status viaja entre backend e frontend

No C#:

```csharp
public enum ScheduleStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    Absent = 4
}
```

No JSON:

```json
"status": "Scheduled"
```

No TypeScript:

```ts
export type ScheduleStatus = 'Scheduled' | 'Completed' | 'Cancelled' | 'Absent';
```

O `JsonStringEnumConverter` faz o enum virar string no JSON.

## 9. Separação de responsabilidades

### Controller não deve:

```txt
Acessar banco diretamente
Montar queries complexas
Aplicar todas as regras de negócio
Conhecer detalhes do EF Core
```

### Service não deve:

```txt
Conhecer HTTP
Retornar IActionResult
Depender diretamente do Angular
Manipular Response/Request
```

### Repository não deve:

```txt
Aplicar regras de negócio
Conhecer DTOs HTTP
Conhecer componentes Angular
```

### DTO não deve:

```txt
Ter lógica de banco
Ter métodos complexos
Substituir a entidade
```

## 10. Por que usar interfaces?

Exemplo:

```csharp
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
}
```

O controller depende de uma abstração.

Benefícios:

```txt
1. Facilita testes futuros.
2. Reduz acoplamento.
3. Permite trocar implementação sem alterar o controller.
4. Aplica DIP, o princípio de inversão de dependência do SOLID.
```

Para um projeto pequeno, isso já é suficiente. Não precisa adicionar arquitetura pesada.

## 11. Sobre SOLID neste projeto

O projeto aplica SOLID de forma simples:

```txt
S — Single Responsibility Principle
Cada classe tem uma responsabilidade principal.

O — Open/Closed Principle
Você pode adicionar novas regras no service sem mudar controller e repository.

L — Liskov Substitution Principle
As interfaces permitem substituir implementações por outras compatíveis.

I — Interface Segregation Principle
As interfaces são pequenas e específicas.

D — Dependency Inversion Principle
Controller depende de IScheduleService; service depende de IScheduleRepository.
```

Sem exagero. A ideia é aprender organização, não criar camadas desnecessárias.

## 12. Por que não usar Hub/SignalR?

SignalR serve para comunicação em tempo real, como:

```txt
notificações instantâneas
chat
painéis atualizados em tempo real
presença online
```

O CRUD `Schedule` não precisa disso.

Por isso a pasta `Hub` fica fora do MVP.

## 13. Por que não usar autenticação?

Autenticação adicionaria:

```txt
usuários
senhas
JWT
guards no Angular
interceptors
políticas de autorização
refresh token
```

Isso mudaria o foco do aprendizado. O objetivo atual é aprender CRUD completo.

## 14. Por que não usar Docker e NGINX?

Docker e NGINX são úteis para empacotamento, deploy e proxy reverso.

Neste MVP, o objetivo é entender:

```txt
Angular chamando API
API aplicando regras
API usando EF Core
EF Core gravando no PostgreSQL local
```

Adicionar Docker e NGINX agora aumentaria o número de problemas possíveis sem melhorar o aprendizado do CRUD.

## 15. Padrão de nomes

No código:

```txt
Classes: PascalCase
Métodos: PascalCase
Propriedades C#: PascalCase
Variáveis locais: camelCase
Arquivos Angular: kebab-case + .component/.service/.model
Colunas PostgreSQL: snake_case
Rotas HTTP: kebab/plural simples
```

Exemplos:

```txt
ScheduleController.cs
ScheduleService.cs
schedule-form.component.ts
employee_registration
/api/schedules
```

## 16. Evoluções futuras sem quebrar o escopo atual

Quando este MVP estiver funcionando, você pode evoluir nesta ordem:

```txt
1. Adicionar testes unitários do ScheduleService.
2. Adicionar tratamento global de erros no backend.
3. Adicionar paginação no GET /api/schedules.
4. Adicionar filtros por data, department e status.
5. Adicionar autenticação.
6. Adicionar interceptors no Angular.
7. Adicionar Docker.
8. Adicionar deploy.
```

Essas evoluções devem vir depois do CRUD estar sólido.

## 17. Checklist de entendimento

Você entendeu a arquitetura se conseguir explicar:

```txt
[ ] Para que serve o Controller
[ ] Para que serve o Service
[ ] Para que serve o Repository
[ ] Para que serve o DTO
[ ] Para que serve a Entity
[ ] Para que serve o DbContext
[ ] Por que o Angular usa HttpClient
[ ] Por que o Angular tem um ScheduleService
[ ] Por que validar no frontend e no backend
[ ] Por que CORS é necessário
[ ] Por que migrations são usadas
[ ] Por que não usamos Hub neste MVP
```

## 18. Referências oficiais usadas

- ASP.NET Core Web API: https://learn.microsoft.com/en-us/aspnet/core/web-api/
- ASP.NET Core CORS: https://learn.microsoft.com/en-us/aspnet/core/security/cors
- EF Core CLI: https://learn.microsoft.com/en-us/ef/core/cli/dotnet
- Npgsql EF Core Provider: https://www.npgsql.org/efcore/
- Angular Routing: https://angular.dev/guide/routing
- Angular HttpClient: https://angular.dev/guide/http/setup
- Angular Reactive Forms: https://angular.dev/guide/forms/reactive-forms
- Conventional Commits: https://www.conventionalcommits.org/en/v1.0.0/
