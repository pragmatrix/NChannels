using System;
using System.Threading.Tasks;

namespace Channels
{
	public static class Spawn
	{
		public static void action(Action action)
		{
			Task.Factory.StartNew(action);
		}
	}
}
