namespace StandPoint.Bitcoin.Interfaces
{
    internal interface IAssertNetwork
    {
        void AssertNetwork(Network network);
        void AssertNetwork(NBitcoin.Network network);
    }
}