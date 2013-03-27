using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalShare
{
	class Program
	{
		static void Main(string[] args)
		{
			Summary.main();			
			MonoidExample.main();
			NullBusinessLogic.main();
			MaybeExample.mainBind();
			IdentityExample.main();
			MaybeExample.main();
			EitherExample.main();
			WriterExample.main();
			ListExample.main();

			TreeExample.main();
		}
	}
}
