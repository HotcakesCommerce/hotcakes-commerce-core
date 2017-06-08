using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;

namespace BrandonHaynes.ModelAdapter.EntityFramework
	{
	/// <summary>
	/// A base class for adapted ObjectContexts; utilizes the passed-in IConnectionAdapter for adaptation
	/// </summary>
	public class AdaptingObjectContext : ObjectContext
		{
		public AdaptingObjectContext(string connectionString, IConnectionAdapter adapter)
			: base(adapter.AdaptConnection(connectionString))
			{ }

		public AdaptingObjectContext(EntityConnection connection, IConnectionAdapter adapter)
			: base(adapter.AdaptConnection(connection))
			{ }

		public AdaptingObjectContext(string connectionString, string defaultContainerName, IConnectionAdapter adapter)
			: base(adapter.AdaptConnection(connectionString), defaultContainerName)
			{ }

		public AdaptingObjectContext(EntityConnection connection, string defaultContainerName, IConnectionAdapter adapter)
			: base(adapter.AdaptConnection(connection), defaultContainerName)
			{ }
		}
	}
