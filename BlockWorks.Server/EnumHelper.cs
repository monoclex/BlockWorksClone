using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorks.Server
{
	public static class EnumHelper
	{
		public static string ToFriendlyName(this WorldColor wc)
			=> $"{wc} World";
			// => _friendlyName[wc];

		/*

		if we ever need to fine tune the world names, we got this at least

		private static Dictionary<WorldColor, string> _friendlyName = new Dictionary<WorldColor, string> {
			[WorldColor.Gray] = "Gray World",
			[WorldColor.Red] = "Red World",
			[WorldColor.Yellow] = "Yellow World",
			[WorldColor.Green] = "Green World",
			[WorldColor.Blue] = "Blue World",
		};

		*/
	}
}
