using System;
using System.Collections.Generic;
using System.Text;

namespace BivyStick.Sources
{
	internal sealed class WriteCommand : CommandItem
	/* compiled from: CommandItem.kt */
	{
		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @Nullable public byte[] command;
		public byte[] command;
		public bool isCommandMessage;
		public bool isMSG;
		public bool isSosInit;
		public bool isSosMessage;
		public bool isWeatherCommand;
		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @Nullable public com.headgatestudios.bivy.model.Message message;
		public Message message;
		public int totalDataSize;

		public WriteCommand() : this((byte[])null, (Message)null, false, false, false, false, false, 0)
		{
		}

		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: public WriteCommand(@Nullable byte[] bArr, @Nullable com.headgatestudios.bivy.model.Message message2, boolean z, boolean z2, boolean z3, boolean z4, boolean z5, int i)
		public WriteCommand(byte[] bArr, Message message2, bool z, bool z2, bool z3, bool z4, bool z5, int i) : base(true, false, 2)
		{
			this.command = bArr;
			this.message = message2;
			this.isSosMessage = z;
			this.isSosInit = z2;
			this.isCommandMessage = z3;
			this.isWeatherCommand = z4;
			this.isMSG = z5;
			this.totalDataSize = i;
		}

		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @Nullable public final byte[] getCommand()
		public byte[] Command
		{
			get
			{
				return this.command;
			}
			set
			{
				this.command = value;
			}
		}

		//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		//ORIGINAL LINE: @Nullable public final com.headgatestudios.bivy.model.Message getMessage()
		public Message Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		public int TotalDataSize
		{
			get
			{
				return this.totalDataSize;
			}
			set
			{
				this.totalDataSize = value;
			}
		}

		public bool CommandMessage
		{
			get
			{
				return this.isCommandMessage;
			}
			set
			{
				this.isCommandMessage = value;
			}
		}

		public bool MSG
		{
			get
			{
				return this.isMSG;
			}
			set
			{
				this.isMSG = value;
			}
		}

		public bool SosInit
		{
			get
			{
				return this.isSosInit;
			}
			set
			{
				this.isSosInit = value;
			}
		}

		public bool SosMessage
		{
			get
			{
				return this.isSosMessage;
			}
			set
			{
				this.isSosMessage = value;
			}
		}

		public bool WeatherCommand
		{
			get
			{
				return this.isWeatherCommand;
			}
			set
			{
				this.isWeatherCommand = value;
			}
		}






	}

}
