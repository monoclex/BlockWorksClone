using PlayerIO.GameLibrary;

namespace BlockWorks.Server.MessageTypes
{
	public interface IMessage
	{
		Message Pack(Target pet);

		bool Unpack(Message e);
	}

	public enum Target
	{
		// if the server is sending it
		Sending = 0,

		// if the server is recieving it
		Recieving = 1
	}
}