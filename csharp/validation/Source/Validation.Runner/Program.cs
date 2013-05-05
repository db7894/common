using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Bashwork.General.Validation;
using Bashwork.General.Validation.Rules;
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;
using System.Threading;

namespace Validation.Runner
{
	/// <summary>
	/// A simple type that we can use for testing
	/// </summary>
	[Validator]
	public class ExampleClassType// : IValidator<ExampleClassType>
	{
		public string Prefix { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int Age { get; set; }
		public bool Insured { get; set; }
		public DateTime Birthday { get; set; }
		public char Letter { get; set; }
		public TimeSpan Time { get; set; }
		public double Balance { get; set; }
		public int? NullableType { get; set; }
		public IEnumerable<char> Collection { get; set; }

		#region IValidator Members

		[ValidatorMethod(typeof(ExampleClassType))]
		public static void MyValidate()
		{
			Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).IsNotNullOrEmpty().Contains("Mr").And()
				.Property(x => x.Age).IsEven().IsLessThan(65).And()
				.Property(x => x.Collection).ContainsAnyOf('a', 'z', 'y').And()
				.Property(x => x.Insured).IsTrue().And()
				.Property(x => x.Birthday).IsLessThan(DateTime.Now).And()
				.Compile();
		}

		#endregion
	}

	class Program
	{
		private static string BuildUniqueName<TType>(string name)
		{
			return typeof(TType).ToString() + "." + (name ?? string.Empty);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			//
			// Example I18N
			//
			//var culture = CultureInfo.GetCultureInfo("es");
			//Thread.CurrentThread.CurrentCulture = culture;
			//Thread.CurrentThread.CurrentUICulture = culture;

			//
			// Example 1
			//
			//var result = ValidationInterfaceFactory.Compile();
			//var result = ValidationAttributeFactory.Compile();
			//Console.WriteLine("Compiled Result: {0}", result);

			//var handle = new ExampleClassType
			//{
			//    FirstName = "Mr. Person",
			//    Age = 24,
			//    Collection = new[] { 'a', 'b', 'c' },
			//    Insured = true,
			//    Birthday = DateTime.Now.AddYears(-1),
			//};
			//var isValid = handle.DoValidate();


			//
			// Example 2
			//
			//var cache1 = new Dictionary<Type, string> { { typeof(string), "string" } };
			//var cache2 = new Dictionary<string, string> { { "System.String.", "string" } };

			//int iteration2 = 1000000;
			//var stopwatch = Stopwatch.StartNew();
			//for (var id = 0; id < iteration2; ++id)
			//{
			//    var value = cache1[typeof(string)];
			//}
			//Console.WriteLine("Type Lookup[{0}]:\t{1}", iteration2, stopwatch.ElapsedMilliseconds);

			//stopwatch = Stopwatch.StartNew();
			//for (var id = 0; id < iteration2; ++id)
			//{
			//    var value = cache2[BuildUniqueName<string>(null)];
			//}
			//Console.WriteLine("String Lookup[{0}]:\t{1}", iteration2, stopwatch.ElapsedMilliseconds);


			//
			// Example 3
			//
			//var handle = new ExampleClassType
			//{
			//    FirstName = "Mr. Person",
			//    Age = 24,
			//    Collection = new[] { 'a', 'X', 'c' },
			//    Insured = true,
			//    Birthday = DateTime.Now.AddYears(-1),
			//};
			//var result = Validate.That(handle)
			//    .EachItem(x => x.Collection).IsLowercase().And()
			//    .Validate();
			//Console.WriteLine("Result: {0}\n", result.IsSuccessful);
			//foreach (var failure in result.Failures)
			//{
			//    Console.WriteLine(failure.ToString());
			//}


			//
			// Example 4
			//
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).IsNotNullOrEmpty().Contains("Mr").And()
				.Property(x => x.Age).IsEven().IsLessThan(65).And()
				.Property(x => x.Collection).ContainsAnyOf('a', 'z', 'y').And()
				.Property(x => x.Insured).IsTrue().And()
				.EachItem(x => x.Collection).IsLowercase().And()
				.Property(x => x.Birthday).IsLessThan(DateTime.Now).And()
				.Compile().Value;
			int iteration1 = 100000000;
			//Console.WriteLine("BaseLine Test[{0}]:\t{1}", iteration1, BaselineTest(iteration1));
			Console.WriteLine("Validation Test[{0}]:\t{1}", iteration1, ValidationTest(iteration1, validator));


			//
			// Example 5
			//
			//int iteration2 = 1000000;
			//var stopwatch = Stopwatch.StartNew();
			//for (var id = 0; id < iteration2; ++id) {
			//    TestExpression(o => (int)o % 2 == 0, id);
			//}
			//Console.WriteLine("ExpressionTree[{0}]:\t{1}", iteration2, stopwatch.ElapsedMilliseconds);

			//stopwatch = Stopwatch.StartNew();
			//for (var id = 0; id < iteration2; ++id) {
			//    TestPredicate(o => (int)o % 2 == 0, id);
			//}
			//Console.WriteLine("RawPredicate[{0}]:\t{1}", iteration2, stopwatch.ElapsedMilliseconds);

			////IEnumerable<object> handle = new List<ExampleClassType> { new ExampleClassType() };
			////var converted = handle as IEnumerable<ExampleClassType>;
		}

		public static Dictionary<Type, Predicate<object>> _cache
			= new Dictionary<Type, Predicate<object>>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="functor"></param>
		/// <returns></returns>
		public static bool TestExpression<TObject>(Expression<Predicate<object>> functor, TObject value)
		{
			Predicate<object> f;
			if (!_cache.TryGetValue(typeof(TObject), out f))
			{
				f = functor.Compile();
				_cache.Add(typeof(TObject), f);
			}
			return f(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="functor"></param>
		/// <returns></returns>
		public static bool TestPredicate<TObject>(Predicate<object> functor, TObject value)
		{
			return functor(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="iterations"></param>
		/// <returns></returns>
		public static long ValidationTest(int iterations, IValidator<ExampleClassType> validator)
		{
			var handle = new ExampleClassType
			{
				FirstName = "Mr. Person",
				Age = 24,
				Collection = new[] { 'a', 'b', 'c' },
				Insured = true,
				Birthday = DateTime.Now.AddYears(-1),
			};
			var watch = Stopwatch.StartNew();


			for (var count = 0; count <= iterations; ++count)
			{
				var results = validator.Validate(handle);
				//var results = Validate.That(handle)
				//    .Property(x => x.FirstName).IsNotNullOrEmpty().Contains("Mr").And()
				//    .Property(x => x.Age).IsEven().IsLessThan(65).And()
				//    .Property(x => x.Collection).ContainsAnyOf('a', 'z', 'y').And()
				//    .Property(x => x.Insured).IsTrue().And()
				//    .Property(x => x.Birthday).IsPast().And()
				//    .Validate();
			}
			watch.Stop();

			return watch.ElapsedMilliseconds;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="iterations"></param>
		/// <returns></returns>
		public static long BaselineTest(int iterations)
		{
			var handle = new ExampleClassType
			{
				FirstName = "Mr. Person",
				Age = 24,
				Collection = new[] { 'a', 'b', 'c' },
				Insured = true,
				Birthday = DateTime.Now.AddYears(-1),
			};
			var datenow = DateTime.Now;
			var watch = Stopwatch.StartNew();
			var contains = new[] { 'a', 'z', 'y' };

			for (var count = 0; count <= iterations; ++count)
			{
				var result = (!string.IsNullOrEmpty(handle.FirstName) && handle.FirstName.Contains("Mr"))
					&& ((handle.Age % 2 == 0) && (handle.Age < 65))
					&& (contains.Any(v => handle.Collection.Contains(v)))
					&& (handle.Insured)
					&& (handle.Birthday < datenow);
			}
			watch.Stop();

			return watch.ElapsedMilliseconds;
		}
	}
}
