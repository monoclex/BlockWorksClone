using System.Linq;
using PlayerIO.GameLibrary;

namespace BlockWorks.Server.Models.Extensions
{
	public static class FlyExtensions
	{
		public const string Type = "fly";


		public static Message ToMessage(this IFly fly, bool sending)
		{
			var serializedArray = fly.Serialize();

			var modified =
				sending
					? serializedArray.Skip(1)
					: serializedArray;

			var array = modified.Concat(new object[] {0u}).ToArray();

			return Message.Create(Type, array);
		}
	}

	/*
		public Message Pack(Target pet)
		{
			return pet == Target.Sending
				? Message.Create(Type, Id, FlyState)
				: Message.Create(Type, FlyState);
		}
	*/
}