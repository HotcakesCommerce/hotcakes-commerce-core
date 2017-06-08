using System.Xml.Linq;

namespace BrandonHaynes.ModelAdapter.EntityFramework
{
	/// <summary>
	/// Adapts a workspace by changing the database owner to the specified schema value
	/// </summary>
	public class TableSchemaModelAdapter : BaseDecoratorModelAdapter
	{
		/// <summary>
		/// The database schema to use during adaptation
		/// </summary>
		protected string Schema { get; set; }

		/// <summary>
		/// Instantiates a TableSchemaModelAdapter using the given database schema
		/// </summary>
		/// <param name="suffix">The database schema to use during adaptation</param>
		public TableSchemaModelAdapter(string schema)
			: base()
		{ Schema = schema; }

		/// <summary>
		/// Instantiates a TableSchemaModelAdapter using the given schema AND decorates the passed-in
		/// IModelAdapter
		/// </summary>
		/// <param name="schema">The schema to use during adaptation</param>
		/// <param name="decoratedModel">The IModelAdapter to decorate</param>
		public TableSchemaModelAdapter(string schema, IModelAdapter decoratedModel)
			: base(decoratedModel)
		{ Schema = schema; }

		#region IModelAdapter Members

		public override void AdaptStoreEntitySet(XElement storeEntitySet)
		{
			var xAttribute = storeEntitySet.Attribute(ConnectionAdapter.DefaultNamespace + "Schema");
			if (xAttribute != null)
			{
				xAttribute.Value = Schema;
			}
			else
			{
				xAttribute = storeEntitySet.Attribute(ConnectionAdapter.StoreNamespace + "Schema");

				if (xAttribute != null)
				{
					
					xAttribute.Value = Schema;

					if (storeEntitySet.HasElements)
					{
						var xElement = storeEntitySet.Element(ConnectionAdapter.DefaultNamespace + "DefiningQuery");
						if (xElement != null)
						{
							var fromSchema = "[" + xAttribute.Value + "]";
							var toSchema = "[" + Schema + "]";
							xElement.Value = xElement.Value.Replace(fromSchema, toSchema);
						}
					}
				}
			}

			base.AdaptStoreEntitySet(storeEntitySet);
		}

		#endregion
	}
}
