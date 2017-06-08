using System.Xml.XPath;
using System.Xml;
using System;
using System.IO;
using System.Reflection;
namespace BrandonHaynes.ModelAdapter.EntityFramework
	{
	/// <summary>
	/// A class used to parse a connection string for Entity Framework metadata artifacts
	/// </summary>
	internal class MetadataArtifacts
		{
		/// <summary>
		/// The token used to seperate metadata artifacts
		/// </summary>
		private const char separatorToken = '|';

		/// <summary>
		/// The conceptual filename (or resource name) for this workspace
		/// </summary>
		public string ConceptualFilename { get; set; }

		/// <summary>
		/// The storage filename (or resource name) for this workspace
		/// </summary>
		public string StorageFilename { get; set; }

		/// <summary>
		/// The mapping filename (or resource name) for this workspace
		/// </summary>
		public string MappingFilename { get; set; }

		/// <summary>
		/// The conceptual XML for this workspace
		/// </summary>
		public IXPathNavigable ConceptualXml { get; private set; }
		
		/// <summary>
		/// The storage XML for this workspace
		/// </summary>
		public IXPathNavigable StorageXml { get; private set; }
		
		/// <summary>
		/// The mapping XML for this workspace
		/// </summary>
		public IXPathNavigable MappingXml { get; private set;}

		private MetadataArtifacts()
			: base()
			{ }

		/// <summary>
		/// Given a workspace source, resolve the source (potentially using the resourceAssembly if the sources
		/// require loading embedded resources).
		/// </summary>
		/// <param name="sources">The Entity Framework workspace sources</param>
		/// <param name="resourceAssembly">The assembly from which to load embedded resources, when needed.</param>
		public MetadataArtifacts(string sources, Assembly resourceAssembly)
			// Split the passed-in connection string by the seperator token and resolve
			: this(sources.Split(new char[] { separatorToken }, System.StringSplitOptions.RemoveEmptyEntries), resourceAssembly)
			{ }

		private MetadataArtifacts(string[] sources, Assembly resourceAssembly)
			: this(sources[0], sources[1], sources[2], resourceAssembly)
			{ }

		/// <summary>
		/// Given conceptual, storage, and mapping filenames, resolve those names (when embedded resources
		/// are required) and load the XML.
		/// </summary>
		/// <param name="conceptualFilename">The CSDL conceptual filename</param>
		/// <param name="storageFilename">The SSDL storage filename</param>
		/// <param name="mappingFilename">The MSL mapping filename</param>
		/// <param name="resourceAssembly">The resource assembly from which to load embedded resources, when the filenames point to an embedded resource.</param>
		public MetadataArtifacts(string conceptualFilename, string storageFilename, string mappingFilename, Assembly resourceAssembly)
			: this()
			{
			//".\\Test Model.csdl|.\\Test Model.ssdl|.\\Test Model.msl"
			//res://*/Test Model.csdl|res://*/Test Model.ssdl|res://*/Test Model.msl
			//res://Assembly Name/Test Model.csdl|res://Assembly Name/Test Model.ssdl|res://Assembly Name/Test Model.msl
			ConceptualFilename = conceptualFilename;
			StorageFilename = storageFilename;
			MappingFilename = mappingFilename;

			ResolveXml(ConceptualFilename, document => ConceptualXml = document, resourceAssembly);
			ResolveXml(StorageFilename, document => StorageXml = document, resourceAssembly);
			ResolveXml(MappingFilename, document => MappingXml = document, resourceAssembly);
			}

		private static void ResolveXml(string filename, Action<XmlDocument> lambda, Assembly resourceAssembly)
			{
			var xml = new XmlDocument();

			// Does our data come from a "wildcard" embedded resource?  If so, load it from the resourceAssembly
			if (filename.StartsWith("res://*/", StringComparison.OrdinalIgnoreCase))
				xml.Load(resourceAssembly.GetManifestResourceStream(filename.Replace("res://*/", string.Empty)));
			// Otherwise, is a specific assembly specified?  If so, load the resource from that named assembly
			else if (filename.StartsWith("res://", StringComparison.OrdinalIgnoreCase))
				xml.Load(System.Reflection.Assembly.Load(filename.Replace("res://", string.Empty).SubstringBefore('/')).GetManifestResourceStream(filename.Replace("res://", string.Empty).SubstringAfter('/')));
			// Otherwise, load the XML from the file system.
			else
				xml.Load(filename);

			// Execute our passed-in lambda on the XML (which just assigns the document to a local property).
			lambda(xml);
			}
		}
	}
