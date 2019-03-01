using BlockWorks.Server.MessageTypes;

using PlayerIO.GameLibrary;

namespace BlockWorks.Server
{
	public class Player : BasePlayer
	{
		public int Id { get; set; }
		public bool Ready { get; set; }

		public string Username
		{
			get
			{
				if (JoinData.TryGetValue("Username", out var uname) && uname.Length > 0)
					return uname;
				return Id.ToString();
			}
		}

		public void Send(IMessage msg) => Send(msg.Pack(Target.Sending));

		public Join GetJoin(bool isYou)
		{
			return new Join
			{
				You = isYou,
				PlayerId = Id,
				AccountId = Username,
				Username = $"GUEST-{Username.ToUpper()}",
				AvatarId = 1,
				StaffRank = -1,
				X = 1 * 24f,
				Y = 98f * 24f
			};
		}
	}
}