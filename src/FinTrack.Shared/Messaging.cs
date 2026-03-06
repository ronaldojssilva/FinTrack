namespace FinTrack.Shared.Messaging;

public static class Exchanges
{
    public const string FinTrack = "fintrack";
}

public static class RoutingKeys
{
    public const string TransferInitiated = "transfer.initiated";
    public const string TransferCompleted = "transfer.completed";
    public const string TransferFailed    = "transfer.failed";
    public const string AccountCreated    = "account.created";
}

public static class Queues
{
    public const string TransactionEvents     = "transaction.events";
    public const string NotificationTransfers = "notification.transfers";
    public const string NotificationAccounts  = "notification.accounts";
}
