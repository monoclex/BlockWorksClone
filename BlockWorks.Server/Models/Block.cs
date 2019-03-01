using System.Linq;
using PlayerIO.GameLibrary;

namespace BlockWorks.Server.Models.Extensions
{
	public static class BlockExtensions
	{
		public const string Type = "b";

		public static uint GetX(this IBlock block)
		{
			return UtilityFunctions.GetX(block.Position);
		}

		public static void SetX(this IBlock block, uint newX)
		{
			block.Position = UtilityFunctions.ToBlocks(newX, UtilityFunctions.GetY(block.Position));
		}

		public static uint GetY(this IBlock block)
		{
			return UtilityFunctions.GetY(block.Position);
		}

		public static void SetY(this IBlock block, uint newY)
		{
			block.Position = UtilityFunctions.ToBlocks(UtilityFunctions.GetX(block.Position), newY);
		}

		public static Message ToMessage(this IBlock block, bool sending)
		{
			var serializedArray = block.Serialize();

			var modified =
				sending
					? serializedArray.Skip(1)
					: serializedArray;

			var array = modified.Concat(new object[] {0u}).ToArray();

			return Message.Create(Type, array);
		}
	}
}