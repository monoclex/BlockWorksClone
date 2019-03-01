namespace BlockWorks.Server
{
	public static class UIntHelper
	{
		public static bool InRange(this uint item, uint min, uint max)
		{
			return min <= item &&
				   max > item;
		}
	}
}