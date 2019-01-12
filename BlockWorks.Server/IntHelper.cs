using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorks.Server
{
	public static class UIntHelper
	{
		public static bool InRange(this uint item, uint min, uint max)
			=>	min <= item &&
				max > item;
	}
}
