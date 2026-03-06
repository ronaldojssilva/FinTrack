// TODO: Notification Service - Fase 4
// Responsabilidades:
//   - Consome TransferCompleted e TransferFailed do RabbitMQ
//   - Consome AccountCreated do RabbitMQ
//   - Envia notificações (email / webhook / log de auditoria)

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "notification-service" }));

await app.RunAsync();
