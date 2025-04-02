 

// -----------------------------------------------------------------------
// <autogenerated>
//    This code was generated from a template.
//
//    Changes to this file may cause incorrect behaviour and will be lost
//    if the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using BrightstarDB.Client;
using BrightstarDB.EntityFramework;


namespace BrightstarDB_Demo 
{
    public partial class MyEntityContext : BrightstarEntityContext {
    	
    	static MyEntityContext() 
    	{
            InitializeEntityMappingStore();
        }
        
        /// <summary>
        /// Initialize the internal cache of entity attribute information.
        /// </summary>
        /// <remarks>
        /// This method is normally invoked from the static constructor for the generated context class.
        /// It is provided as a public static method to enable the use of the cached entity attribute 
        /// information without the need to construct a context (typically in test code). 
        /// In normal application code you should never need to explicitly call this method.
        /// </remarks>
        public static void InitializeEntityMappingStore()
        {
    		var provider = new ReflectionMappingProvider();
    		provider.AddMappingsForType(EntityMappingStore.Instance, typeof(BrightstarDBDemo.IPerson));
    		EntityMappingStore.Instance.SetImplMapping<BrightstarDBDemo.IPerson, BrightstarDBDemo.Person>();
    		provider.AddMappingsForType(EntityMappingStore.Instance, typeof(BrightstarDBDemo.ICompany));
    		EntityMappingStore.Instance.SetImplMapping<BrightstarDBDemo.ICompany, BrightstarDBDemo.Company>();
    		provider.AddMappingsForType(EntityMappingStore.Instance, typeof(BrightstarDBDemo.IPublication));
    		EntityMappingStore.Instance.SetImplMapping<BrightstarDBDemo.IPublication, BrightstarDBDemo.Publication>();
    		provider.AddMappingsForType(EntityMappingStore.Instance, typeof(BrightstarDBDemo.ITopic));
    		EntityMappingStore.Instance.SetImplMapping<BrightstarDBDemo.ITopic, BrightstarDBDemo.Topic>();
    		provider.AddMappingsForType(EntityMappingStore.Instance, typeof(BrightstarDBDemo.IResearcher));
    		EntityMappingStore.Instance.SetImplMapping<BrightstarDBDemo.IResearcher, BrightstarDBDemo.Researcher>();
    	}
    	
    	/// <summary>
    	/// Initialize a new entity context using the specified BrightstarDB
    	/// Data Object Store connection
    	/// </summary>
    	/// <param name="dataObjectStore">The connection to the BrightstarDB Data Object Store that will provide the entity objects</param>
    	public MyEntityContext(IDataObjectStore dataObjectStore) : base(dataObjectStore)
    	{
    		InitializeContext();
    	}
    
    	/// <summary>
    	/// Initialize a new entity context using the specified Brightstar connection string
    	/// </summary>
    	/// <param name="connectionString">The connection to be used to connect to an existing BrightstarDB store</param>
    	/// <param name="enableOptimisticLocking">OPTIONAL: If set to true optmistic locking will be applied to all entity updates</param>
        /// <param name="updateGraphUri">OPTIONAL: The URI identifier of the graph to be updated with any new triples created by operations on the store. If
        /// not defined, the default graph in the store will be updated.</param>
        /// <param name="datasetGraphUris">OPTIONAL: The URI identifiers of the graphs that will be queried to retrieve entities and their properties.
        /// If not defined, all graphs in the store will be queried.</param>
        /// <param name="versionGraphUri">OPTIONAL: The URI identifier of the graph that contains version number statements for entities. 
        /// If not defined, the <paramref name="updateGraphUri"/> will be used.</param>
    	public MyEntityContext(
    	    string connectionString, 
    		bool? enableOptimisticLocking=null,
    		string updateGraphUri = null,
    		IEnumerable<string> datasetGraphUris = null,
    		string versionGraphUri = null
        ) : base(connectionString, enableOptimisticLocking, updateGraphUri, datasetGraphUris, versionGraphUri)
    	{
    		InitializeContext();
    	}
    
    	/// <summary>
    	/// Initialize a new entity context using the specified Brightstar
    	/// connection string retrieved from the configuration.
    	/// </summary>
    	public MyEntityContext() : base()
    	{
    		InitializeContext();
    	}
    	
    	/// <summary>
    	/// Initialize a new entity context using the specified Brightstar
    	/// connection string retrieved from the configuration and the
    	//  specified target graphs
    	/// </summary>
        /// <param name="updateGraphUri">The URI identifier of the graph to be updated with any new triples created by operations on the store. If
        /// set to null, the default graph in the store will be updated.</param>
        /// <param name="datasetGraphUris">The URI identifiers of the graphs that will be queried to retrieve entities and their properties.
        /// If set to null, all graphs in the store will be queried.</param>
        /// <param name="versionGraphUri">The URI identifier of the graph that contains version number statements for entities. 
        /// If set to null, the value of <paramref name="updateGraphUri"/> will be used.</param>
    	public MyEntityContext(
    		string updateGraphUri,
    		IEnumerable<string> datasetGraphUris,
    		string versionGraphUri
    	) : base(updateGraphUri:updateGraphUri, datasetGraphUris:datasetGraphUris, versionGraphUri:versionGraphUri)
    	{
    		InitializeContext();
    	}
    	
    	private void InitializeContext() 
    	{
    		Persons = 	new BrightstarEntitySet<BrightstarDBDemo.IPerson>(this);
    		Companies = 	new BrightstarEntitySet<BrightstarDBDemo.ICompany>(this);
    		Publications = 	new BrightstarEntitySet<BrightstarDBDemo.IPublication>(this);
    		Topics = 	new BrightstarEntitySet<BrightstarDBDemo.ITopic>(this);
    		Researchers = 	new BrightstarEntitySet<BrightstarDBDemo.IResearcher>(this);
    	}
    	
    	public IEntitySet<BrightstarDBDemo.IPerson> Persons
    	{
    		get; private set;
    	}
    	
    	public IEntitySet<BrightstarDBDemo.ICompany> Companies
    	{
    		get; private set;
    	}
    	
    	public IEntitySet<BrightstarDBDemo.IPublication> Publications
    	{
    		get; private set;
    	}
    	
    	public IEntitySet<BrightstarDBDemo.ITopic> Topics
    	{
    		get; private set;
    	}
    	
    	public IEntitySet<BrightstarDBDemo.IResearcher> Researchers
    	{
    		get; private set;
    	}
    	
        public IEntitySet<T> EntitySet<T>() where T : class {
            var itemType = typeof(T);
            if (typeof(T).Equals(typeof(BrightstarDBDemo.IPerson))) {
                return (IEntitySet<T>)this.Persons;
            }
            if (typeof(T).Equals(typeof(BrightstarDBDemo.ICompany))) {
                return (IEntitySet<T>)this.Companies;
            }
            if (typeof(T).Equals(typeof(BrightstarDBDemo.IPublication))) {
                return (IEntitySet<T>)this.Publications;
            }
            if (typeof(T).Equals(typeof(BrightstarDBDemo.ITopic))) {
                return (IEntitySet<T>)this.Topics;
            }
            if (typeof(T).Equals(typeof(BrightstarDBDemo.IResearcher))) {
                return (IEntitySet<T>)this.Researchers;
            }
            throw new InvalidOperationException(typeof(T).FullName + " is not a recognized entity interface type.");
        }
    
        } // end class MyEntityContext
        
}
namespace BrightstarDBDemo 
{
    
    public partial class Person : BrightstarEntityObject, IPerson 
    {
    	public Person(BrightstarEntityContext context, BrightstarDB.Client.IDataObject dataObject) : base(context, dataObject) { }
        public Person(BrightstarEntityContext context) : base(context, typeof(Person)) { }
    	public Person() : base() { }
    	public System.String Id { get {return GetKey(); } set { SetKey(value); } }
    	#region Implementation of BrightstarDBDemo.IPerson
    
    	public System.String Name
    	{
            		get { return GetRelatedProperty<System.String>("Name"); }
            		set { SetRelatedProperty("Name", value); }
    	}
    
    	public System.String Email
    	{
            		get { return GetRelatedProperty<System.String>("Email"); }
            		set { SetRelatedProperty("Email", value); }
    	}
    
    	public System.String Organization
    	{
            		get { return GetRelatedProperty<System.String>("Organization"); }
            		set { SetRelatedProperty("Organization", value); }
    	}
    
    	public BrightstarDBDemo.ICompany Employer
    	{
            get { return GetRelatedObject<BrightstarDBDemo.ICompany>("Employer"); }
            set { SetRelatedObject<BrightstarDBDemo.ICompany>("Employer", value); }
    	}
    	public System.Collections.Generic.ICollection<BrightstarDBDemo.IPublication> Publications
    	{
    		get { return GetRelatedObjects<BrightstarDBDemo.IPublication>("Publications"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("Publications", value); }
    								}
    	#endregion
    }
}
namespace BrightstarDBDemo 
{
    
    public partial class Company : BrightstarEntityObject, ICompany 
    {
    	public Company(BrightstarEntityContext context, BrightstarDB.Client.IDataObject dataObject) : base(context, dataObject) { }
        public Company(BrightstarEntityContext context) : base(context, typeof(Company)) { }
    	public Company() : base() { }
    	public System.String Id { get {return GetKey(); } set { SetKey(value); } }
    	#region Implementation of BrightstarDBDemo.ICompany
    
    	public System.String Name
    	{
            		get { return GetRelatedProperty<System.String>("Name"); }
            		set { SetRelatedProperty("Name", value); }
    	}
    	public System.Collections.Generic.ICollection<BrightstarDBDemo.IPerson> Employees
    	{
    		get { return GetRelatedObjects<BrightstarDBDemo.IPerson>("Employees"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("Employees", value); }
    								}
    	#endregion
    }
}
namespace BrightstarDBDemo 
{
    
    public partial class Publication : BrightstarEntityObject, IPublication 
    {
    	public Publication(BrightstarEntityContext context, BrightstarDB.Client.IDataObject dataObject) : base(context, dataObject) { }
        public Publication(BrightstarEntityContext context) : base(context, typeof(Publication)) { }
    	public Publication() : base() { }
    	public System.String Id { get {return GetKey(); } set { SetKey(value); } }
    	#region Implementation of BrightstarDBDemo.IPublication
    
    	public System.String Title
    	{
            		get { return GetRelatedProperty<System.String>("Title"); }
            		set { SetRelatedProperty("Title", value); }
    	}
    
    	public System.DateTime PublicationDate
    	{
            		get { return GetRelatedProperty<System.DateTime>("PublicationDate"); }
            		set { SetRelatedProperty("PublicationDate", value); }
    	}
    
    	public System.String Abstract
    	{
            		get { return GetRelatedProperty<System.String>("Abstract"); }
            		set { SetRelatedProperty("Abstract", value); }
    	}
    	public System.Collections.Generic.ICollection<BrightstarDBDemo.IPerson> Authors
    	{
    		get { return GetRelatedObjects<BrightstarDBDemo.IPerson>("Authors"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("Authors", value); }
    								}
    	public System.Collections.Generic.ICollection<BrightstarDBDemo.ITopic> Topics
    	{
    		get { return GetRelatedObjects<BrightstarDBDemo.ITopic>("Topics"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("Topics", value); }
    								}
    	public System.Collections.Generic.ICollection<BrightstarDBDemo.IPublication> References
    	{
    		get { return GetRelatedObjects<BrightstarDBDemo.IPublication>("References"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("References", value); }
    								}
    	public System.Collections.Generic.ICollection<BrightstarDBDemo.IPublication> CitedBy
    	{
    		get { return GetRelatedObjects<BrightstarDBDemo.IPublication>("CitedBy"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("CitedBy", value); }
    								}
    
    	public System.DateTime Created
    	{
            		get { return GetRelatedProperty<System.DateTime>("Created"); }
            		set { SetRelatedProperty("Created", value); }
    	}
    
    	public System.DateTime LastModified
    	{
            		get { return GetRelatedProperty<System.DateTime>("LastModified"); }
            		set { SetRelatedProperty("LastModified", value); }
    	}
    	#endregion
    }
}
namespace BrightstarDBDemo 
{
    
    public partial class Topic : BrightstarEntityObject, ITopic 
    {
    	public Topic(BrightstarEntityContext context, BrightstarDB.Client.IDataObject dataObject) : base(context, dataObject) { }
        public Topic(BrightstarEntityContext context) : base(context, typeof(Topic)) { }
    	public Topic() : base() { }
    	public System.String Id { get {return GetKey(); } set { SetKey(value); } }
    	#region Implementation of BrightstarDBDemo.ITopic
    
    	public System.String Name
    	{
            		get { return GetRelatedProperty<System.String>("Name"); }
            		set { SetRelatedProperty("Name", value); }
    	}
    
    	public System.String Description
    	{
            		get { return GetRelatedProperty<System.String>("Description"); }
            		set { SetRelatedProperty("Description", value); }
    	}
    
    	public BrightstarDBDemo.ITopic ParentTopic
    	{
            get { return GetRelatedObject<BrightstarDBDemo.ITopic>("ParentTopic"); }
            set { SetRelatedObject<BrightstarDBDemo.ITopic>("ParentTopic", value); }
    	}
    	public System.Collections.Generic.ICollection<BrightstarDBDemo.ITopic> SubTopics
    	{
    		get { return GetRelatedObjects<BrightstarDBDemo.ITopic>("SubTopics"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("SubTopics", value); }
    								}
    	public System.Collections.Generic.ICollection<BrightstarDBDemo.IPublication> Publications
    	{
    		get { return GetRelatedObjects<BrightstarDBDemo.IPublication>("Publications"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("Publications", value); }
    								}
    	#endregion
    }
}
namespace BrightstarDBDemo 
{
    
    public partial class Researcher : BrightstarEntityObject, IResearcher 
    {
    	public Researcher(BrightstarEntityContext context, BrightstarDB.Client.IDataObject dataObject) : base(context, dataObject) { }
        public Researcher(BrightstarEntityContext context) : base(context, typeof(Researcher)) { }
    	public Researcher() : base() { }
    	public System.String Id { get {return GetKey(); } set { SetKey(value); } }
    	#region Implementation of BrightstarDBDemo.IResearcher
    
    	public System.Int32 HIndex
    	{
            		get { return GetRelatedProperty<System.Int32>("HIndex"); }
            		set { SetRelatedProperty("HIndex", value); }
    	}
    
    	public System.String ResearchField
    	{
            		get { return GetRelatedProperty<System.String>("ResearchField"); }
            		set { SetRelatedProperty("ResearchField", value); }
    	}
    	#endregion
    	#region Implementation of BrightstarDBDemo.IPerson
    
    	public System.String Name
    	{
            		get { return GetRelatedProperty<System.String>("Name"); }
            		set { SetRelatedProperty("Name", value); }
    	}
    
    	public System.String Email
    	{
            		get { return GetRelatedProperty<System.String>("Email"); }
            		set { SetRelatedProperty("Email", value); }
    	}
    
    	public System.String Organization
    	{
            		get { return GetRelatedProperty<System.String>("Organization"); }
            		set { SetRelatedProperty("Organization", value); }
    	}
    
    	public BrightstarDBDemo.ICompany Employer
    	{
            get { return GetRelatedObject<BrightstarDBDemo.ICompany>("Employer"); }
            set { SetRelatedObject<BrightstarDBDemo.ICompany>("Employer", value); }
    	}
    	public System.Collections.Generic.ICollection<BrightstarDBDemo.IPublication> Publications
    	{
    		get { return GetRelatedObjects<BrightstarDBDemo.IPublication>("Publications"); }
    		set { if (value == null) throw new ArgumentNullException("value"); SetRelatedObjects("Publications", value); }
    								}
    	#endregion
    }
}
