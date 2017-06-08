using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.EntityClient;
using System.Collections.Concurrent;

namespace BrandonHaynes.ModelAdapter.EntityFramework
{
	/// <summary>
	/// A concrete ConnectionAdapter, used to walk an Entity Framework model and invoke an IModelAdapter
	/// as appropriate.
	/// </summary>
	public class ConnectionAdapter : IConnectionAdapter
	{
		#region Properties
		public static XNamespace DefaultNamespace = "http://schemas.microsoft.com/ado/2009/11/edm/ssdl";
		public static XNamespace StoreNamespace = "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator";

		private static IDictionary<string, MetadataWorkspace> metadataMapping = new ConcurrentDictionary<string, MetadataWorkspace>();

		/// <summary>
		/// A mapping of adapted workspace names to metadata workspaces; 
		/// adapted workspaces are cached for performance here and should be used when populated
		/// </summary>
		protected static IDictionary<string, MetadataWorkspace> MetadataMapping { get { return metadataMapping; } }

		/// <summary>
		/// The model adapter used when walking the entity model
		/// </summary>
		protected IModelAdapter ModelAdapter { get; set; }

		/// <summary>
		/// When a connection indicates an EDMX model should be loaded as an embedded resource, the assembly
		/// containing the resource must be passed in as a constructor.  This property contains a reference
		/// to that assembly.
		/// 
		/// When a connection does not require obtaining an embedded reference, this property is unused.
		/// </summary>
		private Assembly ResourceAssembly { get; set; }
		#endregion

		/// <summary>
		/// Creates a connection adapter which utilizes the passed-in model adapter as the model is walked
		/// </summary>
		/// <param name="modelAdapter">The model adapter to use when the model is walked</param>
		/// <param name="resourceAssembly">A reference to the assembly containing the embedded EDMX resources, if any.</param>
		public ConnectionAdapter(IModelAdapter modelAdapter, Assembly resourceAssembly)
		{
			ModelAdapter = modelAdapter;
			ResourceAssembly = resourceAssembly;
		}

		#region IConnectionAdapter Members

		public EntityConnection AdaptConnection(EntityConnection connection)
		{
			// Convert the passed-in connection string to an adapted connection string
			return AdaptConnection(new EntityConnectionStringBuilder(connection.ConnectionString).ConnectionString);
		}

		public EntityConnection AdaptConnection(string connectionString)
		{
			// Parse the connection string for relevant data
			var connectionData = new EntityConnectionStringBuilder(connectionString);

			// If the connection string contains only a reference to a .config name, load that referenced connection string
			if (!string.IsNullOrEmpty(connectionData.Name))
				connectionData = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings[connectionData.Name].ConnectionString);

			// Create our connection and populate it with the connectionString value
			var connection = DbProviderFactories.GetFactory(connectionData.Provider).CreateConnection();
			connection.ConnectionString = connectionData.ProviderConnectionString;

			// Generate an entity connection with the resultant connection, walk the workspace, and return
			return new EntityConnection(AdaptWorkspace(connectionData), connection);
		}

		#endregion

		#region Private Functions

		/// <summary>
		/// Walks a workspace utilizing the IModelAdapter when appropriate
		/// </summary>
		/// <param name="connectionData"></param>
		/// <returns></returns>
		private MetadataWorkspace AdaptWorkspace(EntityConnectionStringBuilder connectionData)
		{
			MetadataWorkspace workspace;

			// If our metadata dictionary already contains the workspace, use it; otherwise walk
			if (!MetadataMapping.TryGetValue(connectionData.Metadata, out workspace))
			{
				// Load our workspace (utilizing the resourceAssembly, when appropriate)
				var metaDataSource = new MetadataArtifacts(connectionData.Metadata, ResourceAssembly);

				// Create our workspace after splitting into conceptual (CSDL), storage (SSDL), and mapping (MSL) subtrees
				workspace = CreateWorkspace(
					metaDataSource.ConceptualXml.CreateNavigator().ReadSubtree(),
					AdaptStorageMetadata(metaDataSource.StorageXml.CreateNavigator().ReadSubtree()),
					AdaptMappingMetadata(metaDataSource.MappingXml.CreateNavigator().ReadSubtree()));

				// Cache our walked workspace for future performance gains
				MetadataMapping[connectionData.Metadata] = workspace;
			}

			return workspace;
		}

		/// <summary>
		/// Given a EDMX storage subtree, adapt it using the instance IModelAdapter
		/// </summary>
		/// <param name="storageReader">The EDMX storage subtree to walk</param>
		/// <returns></returns>
		private XmlReader AdaptStorageMetadata(XmlReader storageReader)
		{
			var xml = XElement.Load(storageReader);

			// Walk the SSDL EntitySets and adapt
			foreach (var storeEntitySet in xml.Descendants(DefaultNamespace + "EntitySet"))
				ModelAdapter.AdaptStoreEntitySet(storeEntitySet);
			// Walk the associative endpoints and adapt 
			foreach (var associationEnd in xml.Descendants(DefaultNamespace + "AssociationSet").Descendants(DefaultNamespace + "End"))
				ModelAdapter.AdaptStoreAssociationEnd(associationEnd);
			// Walk the Functions and adapt
			foreach (var function in xml.Descendants(DefaultNamespace + "Function"))
				ModelAdapter.AdaptStoreFunction(function);

			return xml.CreateReader();
		}
		/// <summary>
		/// Given a MSL mapping subtree, adapt it using the instance IModelAdapter
		/// </summary>
		/// <param name="mappingReader"></param>
		/// <returns></returns>
		private XmlReader AdaptMappingMetadata(XmlReader mappingReader)
		{
			var xml = XElement.Load(mappingReader);

			// Walk the storage EntitySets and adapt
			foreach (var attribute in xml.Descendants().Attributes("StoreEntitySet"))
				ModelAdapter.AdaptMappingStoreEntitySetAttribute(attribute);

			// Walk the storage FunctionImportMapping and adapt
			foreach (var attribute in xml.Descendants().Attributes("FunctionName"))
				ModelAdapter.AdaptMappingFunctionNameAttribute(attribute);

			return xml.CreateReader();
		}
		/// <summary>
		/// Given an already-adapted conceptual (CSDL), storage (SSDL), and mapping (MSL) model,
		/// reconstruct the subtrees into a valid Entity Framework workspace.
		/// </summary>
		/// <param name="conceptualReader">The conceptual (CSDL) subtree for reconstruction</param>
		/// <param name="storageReader">The storage (SSDL) subtree for reconstruction</param>
		/// <param name="mappingReader">The mapping (MSL) subtree for reconstruction</param>
		/// <returns></returns>
		private static MetadataWorkspace CreateWorkspace(XmlReader conceptualReader, XmlReader storageReader, XmlReader mappingReader)
		{
			var workspace = new MetadataWorkspace();

			// Convert our XML data into workspace collections (the enumerable XmlReaders will be singletons)
			var conceptualCollection = new EdmItemCollection(conceptualReader.ToEnumerable());
			var storageCollection = new StoreItemCollection(storageReader.ToEnumerable());
			var mappingCollection = new StorageMappingItemCollection(conceptualCollection, storageCollection,
				mappingReader.ToEnumerable());

			// Register our collections in the workspace
			workspace.RegisterItemCollection(conceptualCollection);
			workspace.RegisterItemCollection(storageCollection);
			workspace.RegisterItemCollection(mappingCollection);

			return workspace;
		}

		#endregion
	}
}
