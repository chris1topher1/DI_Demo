namespace ALK.TripInsight.GpsStreamConsumer.Configuration
{
    internal class KinesisConfiguration
    {
        public bool UseKinesConfigFromAppConfig { get; set; }
        public string AWSKey { get; set; }
        public string AWSSecret { get; set; }
        public string AWSRegion { get; set; }
        public string StreamName { get; set; }
    }
}