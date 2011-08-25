using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Channels.Tests
{
	[TestFixture]
	class TestSelect
	{

		[Test]
		public void dualChannelPrinter()
		{
			var intChan = new Channel<int>();
			var stringChan = new Channel<string>();

			Spawn.action(() => sendIntegers(intChan));
			Spawn.action(() => sendStrings(stringChan));

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
