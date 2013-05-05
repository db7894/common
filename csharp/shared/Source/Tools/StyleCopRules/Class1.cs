using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedAssemblies.Tools.StyleCopRules 
{
	public struct Test  
	{ 
		public string Name { get; set; }
	}

	public class ClassTest
	{ 
		public Test Dotest()
		{
			return new Test {Name = "Jim"};
		}
	}
}
 