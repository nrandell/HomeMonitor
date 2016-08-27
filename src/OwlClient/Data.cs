using System;

namespace OwlClient {
    public class Data {
        public string Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int Rssi { get; set; }
        public int Lqi { get; set; }
        public double Battery { get; set; }
        public double Watts { get; set; }
        public double DailyWattHours { get; set; }

        public override string ToString() => $"{Id} {Timestamp} {Rssi} {Lqi} {Battery} {Watts} {DailyWattHours}";
    }
}