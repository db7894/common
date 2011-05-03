// ------------------------------------------------------------------------- //
// Example Useage
// ------------------------------------------------------------------------- //
public class TestClass
{
	public List<string> NickNames;
	public string FirstName;
	public int Age;
	public DateTime Birthday;
	public bool Insured;
	public List<string> Collection;
}

var results = Validate.That(handle)
	.Property(x => x.FirstName).IsNotNullOrEmpty().Contains("Mr").IsValid(FormValidationType.Name).And()
	.Property(x => x.Age).IsEven().IsLessThan(65).And()
	.Property(x => x.Collection).ContainsOneOf("2", "3", "5").And()
	.Property(x => x.Insured).IsTrue().And()
	.Property(x => x.Birthday).IsPast().And()
	.Validate();

// ------------------------------------------------------------------------- //
// Things That Need To Be Looked At
// ------------------------------------------------------------------------- //
- Overloading messages (error) and Property Names
  - possibly introduce a simple template system
  - Should I get the input predicate values from the expression or store in a predicate context
  - Also, custom validation messages
- When / Unless methods (do predicate only if the following is also true)
  - easiest with a predicate context, but can change property context to maintain list
    of predicates and attempt to update the previous expression (or all of the previous ones)
- Sub Property Context (probably just another validation chain)
- Chaining sub validation contexts (so the results are returned to one single context)
- Treat a property collection as a single element (CollectionContext)