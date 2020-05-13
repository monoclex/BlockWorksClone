using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BlockWorks.ClientCloner
{
	public static class Helpers
	{
		public static MemoryStream ToMemoryStream(this Stream stream)
		{
			var ms = new MemoryStream();

			var curPos = stream.Position;

			stream.Position = 0;
			stream.CopyTo(ms);
			stream.Position = curPos;

			return ms;
		}
	}
}
