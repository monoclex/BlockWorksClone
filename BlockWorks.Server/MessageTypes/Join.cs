using PlayerIO.GameLibrary;

using System;

namespace BlockWorks.Server.MessageTypes
{
	public class Join : IMessage
	{
		public const string TypeJoin = "join";
		public const string TypeYou = "you";

		public bool You { get; set; }

		public int PlayerId { get; set; }

		public string AccountId { get; set; }

		public string Username { get; set; }

		public uint AvatarId { get; set; }

		public int StaffRank { get; set; }

		public float X { get; set; }

		public float Y { get; set; }

		public Message Pack(Target pet)
		{
			if (pet != Target.Sending) throw new ArgumentException(nameof(pet));

			return Message.Create(You ? TypeYou : TypeJoin, PlayerId, AccountId, Username, AvatarId, StaffRank, X, Y);
		}

		public bool Unpack(Message e) =>
			// should never need to convert back
			false;
	}
}