using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.EntityClient;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Data.Metadata.Edm;
using System.Data.Mapping;
using System.Data.Objects;

namespace Console_Sample
	{
	class Program
		{
		static void Main(string[] args)
			{
			// The ObjectContext used here -- Sample_ModelEntities -- was developed against the unqualified
			// entities { "Names", "Values" }.  Its base class, however, was adjusted to use an adapting 
			// connection constructed as:

            //    base("name=Sample_ModelEntities", "Sample_ModelEntities", 
			//		new BrandonHaynes.ModelAdapter.EntityFramework.ConnectionAdapter(
			//			new BrandonHaynes.ModelAdapter.EntityFramework.TablePrefixModelAdapter("qualified_"),
			//			System.Reflection.Assembly.GetExecutingAssembly()))

			// This constructor pattern is repeated for each of the three public constructors present in
			// the model designer file SampleModel.Designer.cs.  Note that due to the fact that Visual
			// Studio does not expose any method by which a developer may alter this base class via the
			// user interface, this base class must be adjusted directly in the designer file (and therefore
			// must be repeated each time the model is adjusted).

			// This runtime adaption adjusts the entity names to begin with the table prefix "qualified_" 
			// such that the entities { "qualified_Names", "qualified_Values" } are queried.  Running
			// the sample application demonstrates this; the resultant values are displayed from these tables
			// (instead of the unqualified tables that the model was developed against).

			// Though not demonstrated here, two additional tables ({"guest_Names", "guest_Values"}) may be
			// used to demonstrate non-dbo owners using the TableSchemaModelAdapter.

			using (var context = new Sample_ModelEntities())
				foreach (var name in context.Names)
					{
					 Console.WriteLine(name.Name);
					name.Values.Load();
					foreach (var value in name.Values)
						Console.WriteLine(value.Value.PadLeft(24));
					}

			Console.ReadKey();
			}
		}
	}
