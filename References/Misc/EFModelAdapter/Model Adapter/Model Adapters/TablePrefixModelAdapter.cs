using System.Xml.Linq;

namespace BrandonHaynes.ModelAdapter.EntityFramework
{
	/// <summary>
	/// Adapts a workspace by attaching a specified table prefix where applicable
	/// </summary>
	public class TablePrefixModelAdapter : BaseDecoratorModelAdapter
	{
		/// <summary>
		/// The table prefix to use during adaptation
		/// </summary>
		protected string Prefix { get; set; }

		/// <summary>
		/// Instantiates a TablePrefixModelAdapter using the given prefix
		/// </summary>
		/// <param name="prefix">The table prefix to use during adaptation</param>
		public TablePrefixModelAdapter(string prefix)
			: base()
		{ Prefix = prefix; }

		/// <summary>
		/// Instantiates a TablePrefixModelAdapter using the given prefix AND decorates the passed-in
		/// IModelAdapter
		/// </summary>
		/// <param name="prefix">The prefix to use during adaptation</param>
		/// <param name="decoratedModel">The IModelAdapter to decorate</param>
		public TablePrefixModelAdapter(string prefix, IModelAdapter decoratedModel)
			: base(decoratedModel)
		{ Prefix = prefix; }

		#region IModelAdapter Members

		public override void AdaptStoreEntitySet(XElement storeEntitySet)
		{
			var xAttribute = storeEntitySet.Attribute("Name");
			AdaptDefiningQuery(storeEntitySet, xAttribute);
			AdaptAttribute(xAttribute);

			base.AdaptStoreEntitySet(storeEntitySet);
		}

		public override void AdaptStoreAssociationEnd(XElement associationEnd)
		{
			AdaptAttribute(associationEnd.Attribute("EntitySet"));
			base.AdaptStoreAssociationEnd(associationEnd);
		}

		public override void AdaptStoreFunction(XElement function)
		{
			AdaptAttribute(function.Attribute("Name"));
			base.AdaptStoreAssociationEnd(function);
		}

		public override void AdaptMappingStoreEntitySetAttribute(XAttribute attribute)
		{
			AdaptAttribute(attribute);
			base.AdaptMappingStoreEntitySetAttribute(attribute);
		}

		public override void AdaptMappingFunctionNameAttribute(XAttribute attribute)
		{
			var dotIndex = attribute.Value.LastIndexOf('.');
			attribute.Value = attribute.Value.Insert(dotIndex + 1, Prefix);
			
			base.AdaptMappingStoreEntitySetAttribute(attribute);
		}

		#endregion

		/// <summary>
		/// Given an XAttribute, prepends the instance prefix to that attribute
		/// </summary>
		private void AdaptAttribute(XAttribute attribute)
		{ attribute.Value = Prefix + attribute.Value; }

		private void AdaptDefiningQuery(XElement storeEntitySet, XAttribute xAttribute)
		{
			if (storeEntitySet.HasElements)
			{
				var xElement = storeEntitySet.Element(ConnectionAdapter.DefaultNamespace + "DefiningQuery");
				if (xElement != null)
				{
					var fromName = "[" + xAttribute.Value + "]";
					var toName = "[" + Prefix + xAttribute.Value + "]";
					xElement.Value = xElement.Value.Replace(fromName, toName);
				}
			}
		}
	}
}
