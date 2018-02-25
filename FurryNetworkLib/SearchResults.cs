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
            
            public Submission Submission =>
                _type == "artwork" ? JsonConvert.DeserializeObject<Artwork>(JsonConvert.SerializeObject(_source))
                : _type == "journal" ? JsonConvert.DeserializeObject<Journal>(JsonConvert.SerializeObject(_source))
                : JsonConvert.DeserializeObject<Submission>(JsonConvert.SerializeObject(_source));
        }
    }
}