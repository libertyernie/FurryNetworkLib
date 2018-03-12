using System;
using System.Collections.Generic;
using System.Text;

namespace FurryNetworkLib {
	public class NewJournal {
		public bool Community_tags_allowed { get; set; } = true;
		public string Content { get; set; }
		public string Description { get; set; }
		public int Rating { get; set; } = 0;
		public string Status { get; set; } = "public";
		public string Subtitle { get; set; }
		public IEnumerable<string> Tags { get; set; }
		public string Title { get; set; }
	}
}
