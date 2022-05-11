namespace MarketCrud.Domain.Data.DTOs
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class MarketInputDTO
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("industry")]
        public string Industry { get; set; } = null;

        [JsonProperty("ipoYear")]
        public int? IpoYear { get; set; } = null;

        [JsonProperty("marketCap")]
        public long MarketCap { get; set; }

        [JsonProperty("sector")]
        public string Sector { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("netChange")]
        public double NetChange { get; set; }

        [JsonProperty("netChangePercent")]
        public double NetChangePercent { get; set; }

        [JsonProperty("lastPrice")]
        public double LastPrice { get; set; } = 0;
    }
}
