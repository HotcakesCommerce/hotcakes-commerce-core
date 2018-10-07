using System;
using System.Collections.Generic;
using DotNetNuke.Data;
using DotNetNuke.Framework;
using Moslem.Modules.GDS.GDSModuleMVC.Models;
namespace Moslem.Modules.GDS.GDSModuleMVC.Components
{
	internal class ItemManager : ServiceLocator<IItemManager, ItemManager>, IItemManager
	{
		public void CreateItem(Item t)
		{
			using (IDataContext dataContext = DataContext.Instance())
			{
				IRepository<Item> repository = dataContext.GetRepository<Item>();
				repository.Insert(t);
			}
		}
		public void DeleteItem(int itemId, int moduleId)
		{
			Item item = this.GetItem(itemId, moduleId);
			this.DeleteItem(item);
		}
		public void DeleteItem(Item t)
		{
			using (IDataContext dataContext = DataContext.Instance())
			{
				IRepository<Item> repository = dataContext.GetRepository<Item>();
				repository.Delete(t);
			}
		}
		public IEnumerable<Item> GetItems(int moduleId)
		{
			IEnumerable<Item> result;
			using (IDataContext dataContext = DataContext.Instance())
			{
				IRepository<Item> repository = dataContext.GetRepository<Item>();
				result = repository.Get<int>(moduleId);
			}
			return result;
		}
		public Item GetItem(int itemId, int moduleId)
		{
			Item byId;
			using (IDataContext dataContext = DataContext.Instance())
			{
				IRepository<Item> repository = dataContext.GetRepository<Item>();
				byId = repository.GetById<int, int>(itemId, moduleId);
			}
			return byId;
		}
		public void UpdateItem(Item t)
		{
			using (IDataContext dataContext = DataContext.Instance())
			{
				IRepository<Item> repository = dataContext.GetRepository<Item>();
				repository.Update(t);
			}
		}
		protected override Func<IItemManager> GetFactory()
		{
			return () => new ItemManager();
		}
	}
}
