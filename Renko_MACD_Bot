using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class RenkoMACDSupertrendBot : Robot
    {
        [Parameter("MACD Fast EMA", DefaultValue = 12)]
        public int MacdFast { get; set; }

        [Parameter("MACD Slow EMA", DefaultValue = 26)]
        public int MacdSlow { get; set; }

        [Parameter("MACD Signal SMA", DefaultValue = 9)]
        public int MacdSignal { get; set; }

        [Parameter("Supertrend Period", DefaultValue = 10)]
        public int SupertrendPeriod { get; set; }

        [Parameter("Supertrend Multiplier", DefaultValue = 3)]
        public double SupertrendMultiplier { get; set; }

        private MarketSeries _renkoSeries;
        private MacdHistogram _macd;
        private Supertrend _supertrend;

        protected override void OnStart()
        {
            _renkoSeries = MarketData.GetSeries(TimeFrame.Renko);
            _macd = Indicators.MacdHistogram(_renkoSeries.ClosePrices, MacdFast, MacdSlow, MacdSignal);
            _supertrend = Indicators.SuperTrend(_renkoSeries.ClosePrices, SupertrendPeriod, SupertrendMultiplier);
        }

        protected override void OnBar()
        {
            if (_macd.Histogram.LastValue < 0 && _macd.Histogram.Last(2) > 0 &&
                _supertrend.Result.LastValue == -1 && _supertrend.Result.Last(2) == 1 &&
                _renkoSeries.OpenPrices.Last(1) > _renkoSeries.ClosePrices.Last(1) && _renkoSeries.ClosePrices.Last(1) < _renkoSeries.ClosePrices.Last(2))
            {
                ClosePositions();
                ExecuteMarketOrder(TradeType.Sell, Symbol, Symbol.NormalizeVolume(Symbol.QuantityToVolume(Symbol.Quantity)));
            }

            if (_macd.Histogram.LastValue > 0 && _macd.Histogram.Last(2) < 0 &&
                _supertrend.Result.LastValue == 1 && _supertrend.Result.Last(2) == -1 &&
                _renkoSeries.OpenPrices.Last(1) < _renkoSeries.ClosePrices.Last(1) && _renkoSeries.ClosePrices.Last(1) > _renkoSeries.ClosePrices.Last(2))
            {
                ClosePositions();
                ExecuteMarketOrder(TradeType.Buy, Symbol, Symbol.NormalizeVolume(Symbol.QuantityToVolume(Symbol.Quantity)));
            }
        }

        private void ClosePositions()
        {
            foreach (var position in Positions)
            {
                ClosePosition(position);
            }
        }
    }
}