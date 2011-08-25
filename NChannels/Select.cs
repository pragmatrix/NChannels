using System;
using System.Threading;

namespace NChannels
{
	public static class Select
	{
		public static void channels(params ChannelDispatcher[] channelsDispatcher)
		{
			var handles = new WaitHandle[channelsDispatcher.Length];
			for (int i = 0; i != channelsDispatcher.Length; ++i)
				handles[i] = channelsDispatcher[i].WaitHandle;

			Action dispatcher;

			do
			{
				var i = WaitHandle.WaitAny(handles);
				var tryGetDispatcher = channelsDispatcher[i].TryGetDispatcher;
				dispatcher = tryGetDispatcher();
			} while (dispatcher == null);

			dispatcher();
		}
	}

	public static class ChannelSelectExtensions
	{
		public static ChannelDispatcher act<TypeT>(this Channel<TypeT> channel, Action<TypeT> dispatcher)
		{
			return new ChannelDispatcher(channel.ReaderWaitHandle, () =>
				{
					TypeT value;
					if (!channel.tryReceive(out value))
						return null;

					return () => dispatcher(value);
				});
		}
	}

	public struct ChannelDispatcher
	{
		public ChannelDispatcher(WaitHandle waitHandle, Func<Action> tryGetDispatcher)
		{
			WaitHandle = waitHandle;
			TryGetDispatcher = tryGetDispatcher;
		}

		public readonly WaitHandle WaitHandle;
		public readonly Func<Action> TryGetDispatcher;
	}

}
