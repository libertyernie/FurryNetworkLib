using System;
using System.Collections.Generic;
using System.Text;

namespace FurryNetworkLib {
    public class Avatars {
		public string Avatar { get; set; }
		public string Original { get; set; }
		public string Small { get; set; }
		public string Tiny { get; set; }

		public string GetLargest() {
			return Original ?? Avatar ?? Small ?? Tiny;
		}
	}
}
