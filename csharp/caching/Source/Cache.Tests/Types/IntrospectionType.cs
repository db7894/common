
namespace SharedAssemblies.General.Caching.Tests.Types
{
	public class IntrospectionType : IExpirationStrategy<IntrospectionType>
	{
		public bool Expire { get; set; }
		public bool IsExpired(CachedValue<IntrospectionType> item)
		{
			return Expire;
		}
	}
}
