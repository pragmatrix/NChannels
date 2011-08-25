using System;
using System.Diagnostics;
using System.Threading;

namespace Channels
{
	public sealed class Channel<TypeT> : IDisposable
	{
		readonly EventWaitHandle _reader = new AutoResetEvent(false);

		enum State
		{
			FreeToSend,
			Occupied,
			Closed
		}

		readonly object _ = new object();
		State _state;
		TypeT _value;

		public void Dispose()
		{
			lock (_)
			{
				if (_state == State.Closed)
					return;

				waitNotBusy();

				setState(State.Closed);
				_reader.Set();
			}
		}

		public void send(TypeT value)
		{
			lock (_)
			{
				waitNotBusy();

				if (_state == State.Closed)
					throw new ChannelClosedException("sending failed");

				_value = value;
				setState(State.Occupied);

				_reader.Set();

				waitNotBusy();

				if (_state == State.Closed)
					throw new ChannelClosedException("sending failed");

				Debug.Assert(_state == State.FreeToSend);
			}
		}

		void waitNotBusy()
		{
			while (isBusy())
				Monitor.Wait(_);
		}

		// false: closed!

		public bool receive(out TypeT value)
		{
			for (; ; )
			{
				lock (_)
				{
					if (_state == State.Closed)
					{
						value = default(TypeT);
						return false;
					}

					if (_state == State.Occupied)
					{
						value = take();
						return true;
					}
				}
				_reader.WaitOne();
			}
		}

		public WaitHandle ReaderWaitHandle
		{
			get {
				return _reader;
			}
		}

		// throws if the channel is closed (selects with closed channels are not allowed).

		public bool tryReceive(out TypeT value)
		{
			lock (_)
			{
				if (_state == State.Closed)
					throw new ChannelClosedException("tryReceive failed");

				if (_state == State.Occupied)
				{
					value = take();
					return true;
				}

				value = default(TypeT);
				return false;
			}
		}

		TypeT take()
		{
			Debug.Assert(_state == State.Occupied);
			var value = _value;
			_value = default(TypeT);
			setState(State.FreeToSend);

			Monitor.PulseAll(_);
			return value;
		}


		bool isBusy()
		{
			return _state == State.Occupied;
		}

		void setState(State state)
		{
			_state = state;
			Debug.WriteLine(GetHashCode() + " state: " + state);
		}
	}
}
