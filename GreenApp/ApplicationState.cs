using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GreenApp
{
    public class ApplicationState
    {
		private long _userCount;
		public long UserCount
		{
			get => Interlocked.Read(ref _userCount);
			set => Interlocked.Exchange(ref _userCount, value);
		}
	}
}
