namespace FinTrack.API.DTO
{
    public readonly record struct CreateTransactionRequest(Guid SourceAccountId, Guid DestinationAccountId, decimal Amount);
}
