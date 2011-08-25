using System;
using NChannels;
using NUnit.Framework;

namespace Channels.Tests
{
	[TestFixture]
	sealed class TestSendReceive
	{
		[Test]
		public void SimpleHelloWorld()
		{
			using (var ch = new Channel<string>())
			{
				Spawn.program(() => printStrings(ch, ""));

				ch.send("Hello");
				ch.send("World!");
			}
		}

		[Test]
		public void OneSenderMultipleReceivers()
		{
			using (var ch = new Channel<string>())
			{
				Spawn.program(() => printStrings(ch, "A: "));
				Spawn.program(() => printStrings(ch, "B: "));

				for (int i = 0; i != 50; ++i)
					ch.send(i.ToString());
			}
		}

		[Test]
		public void MultipleSenderOneReceiver()
		{
			using (var ch = new Channel<string>())
			{
				var sigEnd = new Channel<bool>();

				Spawn.program(() => printStrings(ch, ""));

				Spawn.program(() => sendStrings(ch, sigEnd, "A: "));
				Spawn.program(() => sendStrings(ch, sigEnd, "B: "));

				for (int i = 0; i != 50; ++i)
					ch.send(i.ToString());

				bool x;
				sigEnd.receive(out x);
				sigEnd.receive(out x);
			}
		}

		[Test]
		public void MultipleSenderMultipleReceiver()
		{
			using (var ch = new Channel<string>())
			{
				var sigEnd = new Channel<bool>();

				Spawn.program(() => printStrings(ch, "A: "));
				Spawn.program(() => printStrings(ch, "B: "));
				Spawn.program(() => sendStrings(ch, sigEnd, "from A: "));
				Spawn.program(() => sendStrings(ch, sigEnd, "from B: "));

				for (int i = 0; i != 50; ++i)
					ch.send(i.ToString());

				bool x;
				sigEnd.receive(out x);
				sigEnd.receive(out x);
			}
		}

		void sendStrings(Channel<string> strings, Channel<bool> sig, string prefix)
		{
			for (int i = 0; i!= 100; ++i)
				strings.send(prefix + i);

			sig.send(true);
		}

		void printStrings(Channel<string> strings, string prefix)
		{
			string str;
			while (strings.receive(out str))
			{
				Console.WriteLine(prefix + str);
			}
		}
	}
}
