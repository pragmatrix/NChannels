using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace Channels.Tests
{
	[TestFixture]
	public class TestPrerequisites
	{
		[Test]
		public void checkMaxThreads()
		{
			int worker;
			int completionPort;
			ThreadPool.GetMaxThreads(out worker, out completionPort);

			Debug.WriteLine("worker threads: " + worker);
			Debug.WriteLine("completion port threads: " + completionPort);
		}
	}
}
