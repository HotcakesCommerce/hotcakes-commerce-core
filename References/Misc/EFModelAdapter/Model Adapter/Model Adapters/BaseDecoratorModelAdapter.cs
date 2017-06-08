using System.Xml.Linq;

namespace BrandonHaynes.ModelAdapter.EntityFramework
{
	/// <summary>
	/// An abstract base class for IModelAdapters that implement the decorator pattern
	/// </summary>
	public abstract class BaseDecoratorModelAdapter : IModelAdapter
	{
		/// <summary>
		/// The underlying decorated IModelAdapter that this object decorates
		/// </summary>
		protected IModelAdapter DecoratedModel { get; private set; }

		protected BaseDecoratorModelAdapter()
			: base()
		{ }

		/// <summary>
		/// Creates an DecoratorModelAdapter by decorating the underlying decorated IModelAdapter
		/// </summary>
		/// <param name="decoratedModel">The IModelAdapter to decorate</param>
		protected BaseDecoratorModelAdapter(IModelAdapter decoratedModel)
			: this()
		{ DecoratedModel = decoratedModel; }

		#region IModelAdapter Members

		public virtual void AdaptStoreEntitySet(XElement storeEntitySet)
		{
			if (DecoratedModel != null) DecoratedModel.AdaptStoreEntitySet(storeEntitySet);
		}

		public virtual void AdaptStoreAssociationEnd(XElement associationEnd)
		{
			if (DecoratedModel != null) DecoratedModel.AdaptStoreAssociationEnd(associationEnd);
		}

		public virtual void AdaptStoreFunction(XElement function)
		{
			if (DecoratedModel != null) DecoratedModel.AdaptStoreAssociationEnd(function);
		}

		public virtual void AdaptMappingStoreEntitySetAttribute(XAttribute attribute)
		{
			if (DecoratedModel != null) DecoratedModel.AdaptMappingStoreEntitySetAttribute(attribute);
		}

		public virtual void AdaptMappingFunctionNameAttribute(XAttribute attribute)
		{
			if (DecoratedModel != null) DecoratedModel.AdaptMappingStoreEntitySetAttribute(attribute);
		}

		#endregion
	}
}
