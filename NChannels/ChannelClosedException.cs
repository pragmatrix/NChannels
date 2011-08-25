using System;

namespace NChannels
{
	[Serializable]
	public sealed class ChannelClosedException : Exception
	{
		public ChannelClosedException(string description)
			: base(description + ", channel has been closed")
		{
		}
	}
}
