﻿using System;
using System.Collections.Generic;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class resgetprovince
	{
		public bool status { get; set; }
		public int errorCode { get; set; }
		public string description { get; set; }
		public List<Responsegetprovince> response { get; set; }
		public string datetime { get; set; }
	}
}