using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace NChannels.Tests
{
	[TestFixture]
	class TestSelect
	{

		[Test, ExpectedException(typeof(ChannelClosedException))]
		public void dualChannelPrinter()
		{
			var intChan = new Channel<int>();
			var stringChan = new Channel<string>();

			Spawn.program(() => sendIntegers(intChan));
			Spawn.program(() => sendStrings(stringChan));

			for (; ; )
				Select.channels(
					intChan.act(i => Debug.WriteLine(i)),
					stringChan.act(str => Debug.WriteLine(str))
					);
		}

		void sendIntegers(Channel<int> ints)
		{
			foreach (var i in Enumerable.Range(0, 50))
				ints.send(i);

			ints.Dispose();
		}

		void sendStrings(Channel<string> strings)
		{
			foreach (var i in Enumerable.Range(0, 50))
				strings.send("str: " + i);

			strings.Dispose();
		}
	}
}
