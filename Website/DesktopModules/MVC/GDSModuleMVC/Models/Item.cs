using System;
using System.Web.Caching;
using DotNetNuke.ComponentModel.DataAnnotations;
namespace Moslem.Modules.GDS.GDSModuleMVC.Models
{
	[TableName("GDSModuleMVC_Items")]
	[PrimaryKey("ItemId", AutoIncrement = true)]
	[Cacheable("Items", CacheItemPriority.Normal, 20)]
	[Scope("ModuleId")]
	public class Item
	{
		public int ItemId { get; set; } = -1;
		public string ItemName { get; set; }
		public string ItemDescription { get; set; }
		public int AssignedUserId { get; set; }
		public int ModuleId { get; set; }
		public int CreatedByUserId { get; set; } = -1;
		public int LastModifiedByUserId { get; set; } = -1;
		public DateTime CreatedOnDate { get; set; } = DateTime.UtcNow;
		public DateTime LastModifiedOnDate { get; set; } = DateTime.UtcNow;
	}
}
