using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerIO.GameLibrary;

namespace BlockWorks.Server.MessageTypes
{
	public class Block : IMessage
	{
		public const string Type = "b";

		public int Id { get; set; }

		public uint Position { get; set; }

		public uint X
		{
			get => UtilityFunctions.GetX(Position);
			set => UtilityFunctions.ToBlocks(value, UtilityFunctions.GetY(Position));
		}

		public uint Y
		{
			get => UtilityFunctions.GetY(Position);
			set => UtilityFunctions.ToBlocks(UtilityFunctions.GetX(Position), value);
		}

		public uint ModifierIdk { get; set; }

		public Message Pack(Target pet)
		{
			return pet == Target.Sending ?
						Message.Create("b", Id, Position, ModifierIdk)
						: Message.Create("b", Position, ModifierIdk);
		}

		public bool Unpack(Message e)
		{
			if (e[0] is uint position &&
				e[1] is uint type)
			{
				Position = position;
				ModifierIdk = type;

				return true;
			}

			return false;
		}
	}
}
