using System.Data.Entity.Core.EntityClient;

namespace BrandonHaynes.ModelAdapter.EntityFramework
	{
	/// <summary>
	/// An interface for adapting an Entity Framework connection
	/// </summary>
	public interface IConnectionAdapter
		{
		/// <summary>
		/// Adapts a connection given the given entity connection
		/// </summary>
		/// <param name="connection">The connection to adapt</param>
		/// <returns></returns>
		EntityConnection AdaptConnection(EntityConnection connection);

		/// <summary>
		/// Adapts a connection given the given connection string
		/// </summary>
		/// <param name="connection">The connection to adapt</param>
		/// <returns></returns>
		EntityConnection AdaptConnection(string connectionString);
		}
	}
