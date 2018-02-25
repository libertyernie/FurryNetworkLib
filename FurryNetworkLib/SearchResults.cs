using Newtonsoft.Json;
using System.Collections.Generic;

namespace FurryNetworkLib {
    public class SearchResults {
        public IEnumerable<Hit> Hits { get; set; }
        public IEnumerable<object> Tags { get; set; }
        public int Total { get; set; }

        public class Hit {
            public string _index { get; set; }
            public string _type { get; set; }
            public int _id { get; set; }
            public Dictionary<string, object> _source { get; set; }
            public IEnumerable<object> _sort { get; set; }

            private string Json => JsonConvert.SerializeObject(_source);
            public Submission Submission =>
                _type == "artwork" ? JsonConvert.DeserializeObject<Artwork>(Json)
                : _type == "journal" ? JsonConvert.DeserializeObject<Journal>(Json)
                : JsonConvert.DeserializeObject<Submission>(Json);
        }
    }
}