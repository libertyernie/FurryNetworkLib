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
            public Source _source { get; set; }
            public IEnumerable<object> _sort { get; set; }
        }

        public class Source {
            public int Id { get; set; }
            public int CharacterId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int Rating { get; set; }
            public string Status { get; set; }
            public string Created { get; set; }
            public string Updated { get; set; }
            public string Published { get; set; }
            public object Deleted { get; set; }
            public object Hard_deleted { get; set; } // boolean or int
            public bool Processed { get; set; }
            public bool Community_tags_allowed { get; set; }
            public string Record_type { get; set; }
            public object Ticket_id { get; set; }
            public IEnumerable<object> Collection_ids { get; set; }
            //public Character Character { get; set; }
            public IEnumerable<object> Tags { get; set; }
            public bool Promoted { get; set; }
            public int Comments { get; set; }
            public int Favorites { get; set; }
            public int Promotes { get; set; }
            public int Promotes_Week { get; set; }
            public int Promotes_Month { get; set; }
            public int Views { get; set; }
            public bool Favorited { get; set; }
            public object Tag_suggest { get; set; }
            public IEnumerable<object> Promote_array { get; set; }

            // Artwork
            public string Md5 { get; set; }
            public string Url { get; set; }
            public string File_name { get; set; }
            public string Extension { get; set; }
            public string Path { get; set; }
            public string Content_type { get; set; }
            public int Size { get; set; }
            public Images Images { get; set; }
            
            // Journal
            public string Subtitle { get; set; }
            public string Content { get; set; }
        }
    }
}