namespace StandPoint.Bitcoin.States
{
    public enum TransactionBuildState
    {
        NotInProgress,
        GatheringCoinsToSpend,
        BuildingTransaction,
        CheckingTransaction
    }
}