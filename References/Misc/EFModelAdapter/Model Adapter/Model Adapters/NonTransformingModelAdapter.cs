using System.Xml.Linq;

namespace BrandonHaynes.ModelAdapter.EntityFramework
{
	/// <summary>
	/// Implements the IModelAdapter interface but does not perform any changes during workspace walking.
	/// </summary>
	public class NonTransformingModelAdapter : IModelAdapter
	{
		#region IModelAdapter Members

		public void AdaptStoreEntitySet(XElement storeEntitySet)
		{ }

		public void AdaptStoreAssociationEnd(XElement associationEnd)
		{ }

		public void AdaptStoreFunction(XElement function)
		{ }

		public void AdaptMappingStoreEntitySetAttribute(XAttribute attribute)
		{ }

		public void AdaptMappingFunctionNameAttribute(XAttribute attribute)
		{ }

		#endregion
	}
}
