// TODO: Transaction Service - Fase 3
// Responsabilidades:
//   - Transferências entre contas (Command side - CQRS)
//   - Histórico de transações (Query side - CQRS)
//   - Publica TransferInitiated, TransferCompleted, TransferFailed no RabbitMQ
//   - Consome próprios eventos para atualizar Read Model

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "transaction-service" }));

await app.RunAsync();
