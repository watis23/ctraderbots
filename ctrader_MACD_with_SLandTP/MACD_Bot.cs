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

        private MacdHistogram macd;

        protected override void OnStart()
        {
            macd = Indicators.MacdHistogram(Source, FastPeriods, SlowPeriods, SignalPeriods);
        }

        protected override void OnTick()
        {
            // Prüfe, ob eine Position bereits offen ist
            if (Positions.Count > 0) return;

            // Berechne die MACD-Linie
            double macdLine = macd.Histogram.LastValue + macd.Signal.LastValue;

            // Prüfe die Kreuzung und die Farbe der letzten Kerze
            if (macd.Signal.Last(1) > macdLine && macd.Signal.Last(2) < (macd.Histogram.Last(2) + macd.Signal.Last(2)) && Bars.LastBar.Close < Bars.LastBar.Open)
            {
                // Verkaufssignal, wenn die Signal-Linie die MACD-Linie von unten kreuzt und die Kerze rot ist
                ExecuteMarketOrder(TradeType.Sell, SymbolName, Symbol.QuantityToVolumeInUnits(Quantity), "SampleMACD", StopLossPips, TakeProfitPips);
            }
            else if (macd.Signal.Last(1) < macdLine && macd.Signal.Last(2) > (macd.Histogram.Last(2) + macd.Signal.Last(2)) && Bars.LastBar.Close > Bars.LastBar.Open)
            {
                // Kaufsignal, wenn die Signal-Linie die MACD-Linie von oben kreuzt und die Kerze grün ist
                ExecuteMarketOrder(TradeType.Buy, SymbolName, Symbol.QuantityToVolumeInUnits(Quantity), "SampleMACD", StopLossPips, TakeProfitPips);
            }
        }

        private void Close(TradeType tradeType)
        {
            foreach (var position in Positions.FindAll("SampleMACD", SymbolName, tradeType))
                ClosePosition(position);
        }
    }
}
