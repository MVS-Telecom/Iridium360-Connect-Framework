using System;
using System.Collections.Generic;
using System.Text;

namespace BivyStick.Sources
{
	internal class CommandItem
	/* compiled from: CommandItem.kt */
	{
		public bool isReadCommand;
		public bool isWriteCommand;

		public CommandItem() : this(false, false, 3)
		{
		}

		public CommandItem(bool z, bool z2)
		{
			this.isWriteCommand = z;
			this.isReadCommand = z2;
		}

		public bool ReadCommand
		{
			get
			{
				return this.isReadCommand;
			}
			set
			{
				this.isReadCommand = value;
			}
		}

		public bool WriteCommand
		{
			get
			{
				return this.isWriteCommand;
			}
			set
			{
				this.isWriteCommand = value;
			}
		}



		/* JADX INFO: this call moved to the top of the method (can break code semantics) */
		public CommandItem(bool z, bool z2, int i) : this((i & 1) != 0 ? false : z, (i & 2) != 0 ? false : z2)
		{
		}
	}
}
