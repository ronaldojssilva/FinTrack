// TODO: Account Service - Fase 2
// Responsabilidades:
//   - Criação de conta bancária
//   - Consulta de saldo
//   - Débito/crédito via eventos RabbitMQ (TransferCompleted)
//   - Publica AccountCreated no RabbitMQ

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "account-service" }));

await app.RunAsync();
