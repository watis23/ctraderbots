using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SampleMACDcBotgeht : Robot
    {
        [Parameter("Quantity (Lots)", Group = "Volume", DefaultValue = 1, MinValue = 0.01, Step = 0.01)]
        public double Quantity { get; set; }

        [Parameter("Source", Group = "MACD")]
        public DataSeries Source { get; set; }

        [Parameter("Fast EMA Periods", Group = "MACD", DefaultValue = 12)]
        public int FastPeriods { get; set; }

        [Parameter("Slow EMA Periods", Group = "MACD", DefaultValue = 26)]
        public int SlowPeriods { get; set; }

        [Parameter("Signal Periods", Group = "MACD", DefaultValue = 9)]
        public int SignalPeriods { get; set; }

        [Parameter("Stop Loss (Pips)", Group = "Trading", DefaultValue = 10)]
        public int StopLossPips { get; set; }

        [Parameter("Take Profit (Pips)", Group = "Trading", DefaultValue = 10)]
        public int TakeProfitPips { get; set; }

        [Parameter("Trailing Stop Start (Pips)", Group = "Trading", DefaultValue = 2)]
        public int TrailingStopStart { get; set; }

        [Parameter("Trailing Stop Distance (Pips)", Group = "Trading", DefaultValue = 2)]
        public int TrailingStopDistance { get; set; }

        private MacdHistogram macd;

        protected override void OnStart()
        {
            macd = Indicators.MacdHistogram(Source, FastPeriods, SlowPeriods, SignalPeriods);
        }

        protected override void OnTick()
        {
            // Aktualisiere Trailing Stops für offene Positionen
            UpdateTrailingStops();

            // Prüfe, ob eine Position bereits offen ist
            if (Positions.Count > 0) return;

            // [Rest des Codes für das Öffnen neuer Positionen bleibt unverändert]
        }

        private void UpdateTrailingStops()
        {
            foreach (var position in Positions)
            {
                if (position.SymbolName != SymbolName || position.Label != "SampleMACD")
                    continue;

                double pipsProfit = position.NetProfit / Symbol.PipValue;

                if (pipsProfit >= TrailingStopStart)
                {
                    double newStopLossPrice;

                    if (position.TradeType == TradeType.Buy)
                    {
                        newStopLossPrice = position.EntryPrice + TrailingStopStart * Symbol.PipSize;
                        newStopLossPrice = Math.Max(newStopLossPrice, Symbol.Bid - TrailingStopDistance * Symbol.PipSize);
                    }
                    else
                    {
                        newStopLossPrice = position.EntryPrice - TrailingStopStart * Symbol.PipSize;
                        newStopLossPrice = Math.Min(newStopLossPrice, Symbol.Ask + TrailingStopDistance * Symbol.PipSize);
                    }

                    if (position.StopLoss == null || Math.Abs(newStopLossPrice - position.StopLoss.Value) >= Symbol.PipSize)
                    {
                        ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                    }
                }
            }
        }

        private void Close(TradeType tradeType)
        {
            foreach (var position in Positions.FindAll("SampleMACD", SymbolName, tradeType))
                ClosePosition(position);
        }
    }
}
