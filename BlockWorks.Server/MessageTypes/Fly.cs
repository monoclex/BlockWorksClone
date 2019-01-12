using PlayerIO.GameLibrary;

namespace BlockWorks.Server.MessageTypes
{
	public class Fly : IMessage
	{
		public const string Type = "fly";

		public bool FlyState { get; set; }

		public int Id { get; set; }

		public bool Unpack(Message e)
		{
			if (e.Type == Type &&
				e.Count == 1 &&
				e[0] is bool flyState)
			{
				FlyState = flyState;

				return true;
			}
			else return false;
		}

		public Message Pack(Target pet)
		{
			return pet == Target.Sending ?
						Message.Create(Fly.Type, Id, FlyState)
						: Message.Create(Fly.Type, FlyState);
		}
	}
}