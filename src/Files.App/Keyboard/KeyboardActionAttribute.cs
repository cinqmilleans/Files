using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.App.Keyboard
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class KeyboardActionAttribute : Attribute
	{
		public string Code { get; }

		public KeyboardActionAttribute(string code) => Code = code;


	}
}
