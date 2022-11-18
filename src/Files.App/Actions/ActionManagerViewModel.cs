using Files.App.Actionn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.App.Action
{
	public class ActionManagerViewModel
	{


		public class Action
		{
			public string Label => "Rename";
			public HotKey HotKey => HotKey.None;
		}
	}
}
