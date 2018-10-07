using System;
using System.Collections.Generic;
using Moslem.Modules.GDS.GDSModuleMVC.Models;
namespace Moslem.Modules.GDS.GDSModuleMVC.Components
{
	internal interface IItemManager
	{
		void CreateItem(Item t);
		void DeleteItem(int itemId, int moduleId);
		void DeleteItem(Item t);
		IEnumerable<Item> GetItems(int moduleId);
		Item GetItem(int itemId, int moduleId);
		void UpdateItem(Item t);
	}
}
