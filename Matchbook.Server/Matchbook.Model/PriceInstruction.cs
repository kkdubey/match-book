namespace Matchbook.Model
{
    public enum PriceInstruction 
    {
        GoodFirTradingDay,
        GoodThroughDate,
        GoodUntilCanceled,
        MarketOnOpen,
        MarketOnClose,
        HighOfTheDay,
        LowOfTheDay,
        Limit
    }
}
