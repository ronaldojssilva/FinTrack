namespace FinTrack.Shared.Events;

public record TransferInitiated(
    Guid TransferId,
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount,
    DateTime OccurredAt);

public record TransferCompleted(
    Guid TransferId,
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount,
    DateTime OccurredAt);

public record TransferFailed(
    Guid TransferId,
    Guid FromAccountId,
    Guid ToAccountId,
    decimal Amount,
    string Reason,
    DateTime OccurredAt);

public record AccountCreated(
    Guid AccountId,
    Guid UserId,
    string OwnerName,
    DateTime OccurredAt);
