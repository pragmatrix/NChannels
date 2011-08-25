using System;
using System.Threading.Tasks;

namespace NChannels
{
	public static class Spawn
	{
		public static void program(Action action)
		{
			Task.Factory.StartNew(action);
		}
	}
}
