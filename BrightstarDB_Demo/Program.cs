using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BrightstarDB.EntityFramework;
using BrightstarDB.Client;
using BrightstarDB_Demo;
using VDS.RDF.Writing;
using VDS.RDF;
namespace BrightstarDBDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BrightstarDB Academic Knowledge Graph Demo");
            Console.WriteLine("==========================================");

            // FEATURE 1: Database Management - using a file store for simplicity
            string dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BrightstarDemo");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            string storeName = "AcademicKnowledgeGraph";
            string connectionString = $"type=embedded;storesdirectory={dataFolder};storename={storeName}";

            try
            {
                // Check if store exists first, if not create it
                var client = BrightstarService.GetClient(connectionString);
                if (!client.DoesStoreExist(storeName))
                {
                    Console.WriteLine($"Creating new BrightstarDB store: {storeName}");
                    client.CreateStore(storeName);
                }
                else
                {
                    Console.WriteLine($"Using existing BrightstarDB store: {storeName}");
                }

                // Create context with optimistic concurrency control enabled
                Console.WriteLine("Opening BrightstarDB store...");
                var context = new MyEntityContext(connectionString, enableOptimisticLocking: true);

                // FEATURE 4: Add SavingChanges event handler for tracked objects
                context.SavingChanges += UpdateTrackables;

                // Check if we need to populate the store
                if (context.Publications.Count() == 0)
                {
                    Console.WriteLine("Populating knowledge graph with sample data...");
                    PopulateStore(context);
                }
                else
                {
                    Console.WriteLine("Using existing knowledge graph data");
                }

                // FEATURE 5: Demonstrate LINQ queries
                PerformQueries(context);

                // FEATURE 2: Demonstrate Hierarchical Key Pattern
                DemonstrateHierarchicalKeys(context);

                // FEATURE 3: Demonstrate Entity Casting with Become<T>
                DemonstrateEntityCasting(context);

                // FEATURE 6: Demonstrate SPARQL Query Execution
                ExecuteSparqlQuery(context, connectionString);

                // FEATURE 7: Export data to RDF
                string exportFilePath = Path.Combine(dataFolder, "export.ttl");
                ExportDataToRdf(context, exportFilePath);

                // Offer option to cleanup for next run
                Console.WriteLine("\nDo you want to clean up the database? (Y/N)");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("\nCleaning up database...");
                    context.Dispose();
                    client.DeleteStore(storeName);
                    Console.WriteLine("Database deleted for next run.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        // FEATURE 4: Event handler for SavingChanges - Tracked Objects
        private static void UpdateTrackables(object sender, EventArgs e)
        {
            try
            {
                var context = sender as MyEntityContext;
                foreach (var obj in context.TrackedObjects.Where(x => x is IPublication))
                {
                    var pub = obj as IPublication;
                    if (pub.Created == DateTime.MinValue)
                    {
                        pub.Created = DateTime.Now;
                        Console.WriteLine($"Setting Created timestamp on '{pub.Title}' to {pub.Created}");
                    }

                    pub.LastModified = DateTime.Now;
                    Console.WriteLine($"Setting LastModified timestamp on '{pub.Title}' to {pub.LastModified}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateTrackables: {ex.Message}");
            }
        }

        // FEATURE 2: Demonstrate Hierarchical Key Pattern
        static void DemonstrateHierarchicalKeys(MyEntityContext context)
        {
            Console.WriteLine("\n--- Demonstrating Hierarchical Key Pattern ---\n");

            var allTopics = context.Topics.ToList();
            var semWeb = allTopics.FirstOrDefault(t => t.Name == "Semantic Web");

            if (semWeb != null)
            {
                Console.WriteLine($"Topic: {semWeb.Name}");
                Console.WriteLine($"ID: {semWeb.Id}");
                if (semWeb.ParentTopic != null)
                {
                    Console.WriteLine($"Parent Topic: {semWeb.ParentTopic.Name}");
                    Console.WriteLine($"Parent ID: {semWeb.ParentTopic.Id}");
                }

                // Check if the child topic already exists
                var existingSparql = allTopics.FirstOrDefault(t => t.Name == "SPARQL" &&
                                                            t.ParentTopic != null &&
                                                            t.ParentTopic.Id == semWeb.Id);
                if (existingSparql == null)
                {
                    // Create a new child topic that will inherit the parent's ID in its own ID
                    var sparql = context.Topics.Create();
                    sparql.ParentTopic = semWeb;

                    sparql.Name = "SPARQL";
                    sparql.Description = "SPARQL Protocol and RDF Query Language";

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (BrightstarClientException ex)
                    {
                        Console.WriteLine($"Optimistic concurrency conflict detected when saving hierarchical keys: {ex.Message}");
                    }

                    Console.WriteLine($"Created new child topic: {sparql.Name}");
                    Console.WriteLine($"Child ID: {sparql.Id}");
                    Console.WriteLine("The child ID contains the parent's ID as a prefix, demonstrating hierarchical keys");
                }
                else
                {
                    Console.WriteLine("The child topic 'SPARQL' already exists.");
                }
            }
        }

        // FEATURE 3: Demonstrate Entity Casting with Become<T>
        static void DemonstrateEntityCasting(MyEntityContext context)
        {
            Console.WriteLine("\n--- Demonstrating Entity Casting ---\n");

            var bob = context.Persons.FirstOrDefault(p => p.Name == "Bob Jones");

            if (bob != null)
            {
                Console.WriteLine($"Original person: {bob.Name}");
                Console.WriteLine($"Entity type: {bob.GetType().Name}");
                Console.WriteLine($"ID: {bob.Id}");
                try
                {
                    var entity = bob as BrightstarEntityObject;
                    if (entity != null)
                    {
                        var researcher = entity.Become<IResearcher>();

                        researcher.HIndex = 25;
                        researcher.ResearchField = "Knowledge Graph Engineering";

                        try
                        {
                            context.SaveChanges();
                        }
                        catch (BrightstarClientException ex)
                        {
                            Console.WriteLine($"Optimistic concurrency conflict detected when saving entity casting changes: {ex.Message}");
                        }

                        Console.WriteLine($"Transformed to researcher: {researcher.Name}");
                        Console.WriteLine($"H-Index: {researcher.HIndex}");
                        Console.WriteLine($"Research Field: {researcher.ResearchField}");
                        Console.WriteLine("Entity now has additional properties in the RDF store");
                    }
                    else
                    {
                        Console.WriteLine("Could not access entity object for casting");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Entity casting failed: {ex.Message}");
                    Console.WriteLine("Using alternative approach to demonstrate extending an entity:");

                    var researcher = context.Researchers.Create();
                    researcher.Name = bob.Name;
                    researcher.Email = bob.Email;
                    researcher.Organization = bob.Organization;
                    researcher.HIndex = 25;
                    researcher.ResearchField = "Knowledge Graph Engineering";

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (BrightstarClientException ex2)
                    {
                        Console.WriteLine($"Optimistic concurrency conflict detected when saving new researcher: {ex.Message}");
                    }

                    Console.WriteLine($"Created a new entity representing {researcher.Name}");
                    Console.WriteLine("This demonstrates how entities can have multiple representations");
                }
            }
        }

        // FEATURE 6: Demonstrate SPARQL Query Execution
        static void ExecuteSparqlQuery(MyEntityContext context, string connectionString)
        {
            Console.WriteLine("\n--- Demonstrating SPARQL Query Execution ---\n");

            try
            {
                // Access the underlying data store directly using the connection string
                var dataObjectContext = BrightstarService.GetDataObjectContext(connectionString);
                var store = dataObjectContext.OpenStore("AcademicKnowledgeGraph");

                string sparqlQuery = @"
                    SELECT ?s ?p ?o
                    WHERE {
                        ?s ?p ?o .
                    }
                    LIMIT 10
                ";

                var result = store.ExecuteSparql(sparqlQuery);
                Console.WriteLine("SPARQL query executed successfully");
                Console.WriteLine($"Result contains {result.ToString().Length} characters of data");

                string preview = result.ToString().Length > 500
                    ? result.ToString().Substring(0, 500) + "..."
                    : result.ToString();
                Console.WriteLine("Preview of result:");
                Console.WriteLine(preview);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing SPARQL query: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        // FEATURE 1: Populate store with initial data
        static void PopulateStore(MyEntityContext context)
        {
            try
            {
                // Create topics (research areas) in separate SaveChanges blocks
                var knowledgeGraphs = context.Topics.Create();
                knowledgeGraphs.Name = "Knowledge_Graphs";
                knowledgeGraphs.Description = "Representation and management of structured knowledge using graph-based approaches";
                context.SaveChanges();

                var semanticWeb = context.Topics.Create();
                semanticWeb.Name = "Semantic_Web";
                semanticWeb.Description = "Web of data that enables machines to understand the semantics of information";
                semanticWeb.ParentTopic = knowledgeGraphs;
                context.SaveChanges();

                var rdfDatabases = context.Topics.Create();
                rdfDatabases.Name = "RDF_Databases";
                rdfDatabases.Description = "Database systems optimized for storing and querying RDF data";
                rdfDatabases.ParentTopic = semanticWeb;
                context.SaveChanges();

                var dotNet = context.Topics.Create();
                dotNet.Name = ".NET_Development";
                dotNet.Description = "Development of applications using Microsoft's .NET framework";
                context.SaveChanges();

                var brightstarCompany = context.Companies.Create();
                brightstarCompany.Name = "BrightstarDB";
                context.SaveChanges();

                var research = context.Companies.Create();
                research.Name = "Research Institute of Technology";
                context.SaveChanges();

                var alice = context.Persons.Create();
                alice.Name = "Alice Smith";
                alice.Email = "alice.smith@example.org";
                alice.Organization = "University of Example";
                alice.Employer = research;
                context.SaveChanges();

                var bob = context.Persons.Create();
                bob.Name = "Bob Jones";
                bob.Email = "bob.jones@example.org";
                bob.Organization = "Research Institute of Technology";
                bob.Employer = research;
                context.SaveChanges();

                var charlie = context.Persons.Create();
                charlie.Name = "Charlie Brown";
                charlie.Email = "charlie.brown@example.org";
                charlie.Organization = "BrightstarDB";
                charlie.Employer = brightstarCompany;
                context.SaveChanges();

                var pub1 = context.Publications.Create();
                pub1.Title = "Implementing Knowledge Graphs in .NET Applications";
                pub1.PublicationDate = new DateTime(2023, 6, 15);
                pub1.Abstract = "This paper explores approaches to implementing knowledge graphs in .NET applications, with a focus on BrightstarDB.";
                pub1.Authors = new List<IPerson> { alice, bob };
                pub1.Topics = new List<ITopic> { knowledgeGraphs, dotNet };
                context.SaveChanges();

                var pub2 = context.Publications.Create();
                pub2.Title = "Introduction to Semantic Web Technologies";
                pub2.PublicationDate = new DateTime(2022, 11, 10);
                pub2.Abstract = "An overview of semantic web technologies and standards including RDF, OWL, and SPARQL.";
                pub2.Authors = new List<IPerson> { alice };
                pub2.Topics = new List<ITopic> { semanticWeb };
                context.SaveChanges();

                var pub3 = context.Publications.Create();
                pub3.Title = "BrightstarDB: A Case Study in RDF Database Implementation";
                pub3.PublicationDate = new DateTime(2023, 2, 28);
                pub3.Abstract = "This case study examines the architecture and performance of BrightstarDB for enterprise applications.";
                pub3.Authors = new List<IPerson> { bob, charlie };
                pub3.Topics = new List<ITopic> { rdfDatabases, dotNet };
                pub3.References = new List<IPublication> { pub1 };
                context.SaveChanges();

                Console.WriteLine("Sample data created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error populating store: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        // FEATURE 7: Export Data to RDF
        static void ExportDataToRdf(MyEntityContext context, string filePath)
        {
            Console.WriteLine("\n--- Exporting RDF Data ---\n");

            try
            {
                var graph = new Graph();

                // Define and add namespaces to the graph for proper serialization
                graph.NamespaceMap.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
                graph.NamespaceMap.AddNamespace("ex", new Uri("http://example.org/"));
                graph.NamespaceMap.AddNamespace("dcterms", new Uri("http://purl.org/dc/terms/"));
                graph.NamespaceMap.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));

                string baseUri = "http://example.org/entities/";

                // Helper function to create safe URI from entity ID
                Func<string, Uri> createSafeUri = (id) => {
                    // Remove any characters that would make a URI invalid
                    string safeId = id.Replace(":", "_")
                                      .Replace(" ", "_")
                                      .Replace("#", "_")
                                      .Replace("<", "_")
                                      .Replace(">", "_")
                                      .Replace("\"", "_")
                                      .Replace("{", "_")
                                      .Replace("}", "_")
                                      .Replace("|", "_")
                                      .Replace("\\", "_")
                                      .Replace("^", "_")
                                      .Replace("`", "_");

                    return new Uri(baseUri + safeId);
                };

                foreach (var pub in context.Publications)
                {
                    // Use safe URI creation
                    var pubNode = graph.CreateUriNode(createSafeUri(pub.Id));

                    // Create nodes using the namespaces
                    var typeNode = graph.CreateUriNode("rdf:type");
                    var pubTypeNode = graph.CreateUriNode("ex:Publication");
                    var titleNode = graph.CreateUriNode("ex:title");
                    var dateNode = graph.CreateUriNode("ex:publicationDate");
                    var abstractNode = graph.CreateUriNode("ex:abstract");
                    var authorNode = graph.CreateUriNode("ex:author");
                    var personTypeNode = graph.CreateUriNode("ex:Person");
                    var nameNode = graph.CreateUriNode("ex:name");
                    var topicNode = graph.CreateUriNode("ex:topic");
                    var topicTypeNode = graph.CreateUriNode("ex:Topic");
                    var emailNode = graph.CreateUriNode("foaf:mbox");
                    var descNode = graph.CreateUriNode("dcterms:description");

                    // Add publication triples
                    graph.Assert(pubNode, typeNode, pubTypeNode);
                    graph.Assert(pubNode, titleNode, graph.CreateLiteralNode(pub.Title));
                    graph.Assert(pubNode, dateNode, graph.CreateLiteralNode(
                        pub.PublicationDate.ToString("yyyy-MM-dd"),
                        new Uri("http://www.w3.org/2001/XMLSchema#date")));

                    if (!string.IsNullOrEmpty(pub.Abstract))
                    {
                        graph.Assert(pubNode, abstractNode, graph.CreateLiteralNode(pub.Abstract));
                    }

                    // Add author information
                    foreach (var author in pub.Authors)
                    {
                        var authorEntityNode = graph.CreateUriNode(createSafeUri(author.Id));
                        graph.Assert(pubNode, authorNode, authorEntityNode);
                        graph.Assert(authorEntityNode, typeNode, personTypeNode);
                        graph.Assert(authorEntityNode, nameNode, graph.CreateLiteralNode(author.Name));

                        // Add email if available
                        if (!string.IsNullOrEmpty(author.Email))
                        {
                            graph.Assert(authorEntityNode, emailNode, graph.CreateLiteralNode(author.Email));
                        }
                    }

                    // Add topic information
                    foreach (var topic in pub.Topics)
                    {
                        var topicEntityNode = graph.CreateUriNode(createSafeUri(topic.Id));
                        graph.Assert(pubNode, topicNode, topicEntityNode);
                        graph.Assert(topicEntityNode, typeNode, topicTypeNode);
                        graph.Assert(topicEntityNode, nameNode, graph.CreateLiteralNode(topic.Name));

                        // Add description if available
                        if (!string.IsNullOrEmpty(topic.Description))
                        {
                            graph.Assert(topicEntityNode, descNode, graph.CreateLiteralNode(topic.Description));
                        }
                    }
                }

                // Save with proper formatting
                var writer = new CompressingTurtleWriter();
                writer.Save(graph, filePath);
                Console.WriteLine($"Data exported to {filePath}");
                Console.WriteLine($"Total triples exported: {graph.Triples.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting RDF data: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        // FEATURE 5: LINQ Queries for graph traversal
        static void PerformQueries(MyEntityContext context)
        {
            Console.WriteLine("\n--- LINQ Query Examples ---\n");

            // Example 1: Find all publications about Knowledge Graphs
            Console.WriteLine("Example 1: Publications about Knowledge Graphs");
            Console.WriteLine("------------------------------------------");
            var kgTopic = context.Topics.FirstOrDefault(t => t.Name == "Knowledge Graphs");
            if (kgTopic != null)
            {
                foreach (var pub in kgTopic.Publications)
                {
                    Console.WriteLine($"Title: {pub.Title}");
                    Console.WriteLine($"Authors: {string.Join(", ", pub.Authors.Select(a => a.Name))}");
                    Console.WriteLine($"Date: {pub.PublicationDate.ToShortDateString()}");
                    Console.WriteLine();
                }
            }

            // Example 2: Find publications by author
            Console.WriteLine("Example 2: Publications by Alice Smith");
            Console.WriteLine("------------------------------------");
            var alice = context.Persons.FirstOrDefault(p => p.Name == "Alice Smith");
            if (alice != null)
            {
                foreach (var pub in alice.Publications)
                {
                    Console.WriteLine($"Title: {pub.Title}");
                    Console.WriteLine($"Date: {pub.PublicationDate.ToShortDateString()}");
                    Console.WriteLine($"Topics: {string.Join(", ", pub.Topics.Select(t => t.Name))}");
                    Console.WriteLine();
                }
            }

            // Example 3: Find all .NET-related publications from 2023
            Console.WriteLine("Example 3: .NET Publications from 2023");
            Console.WriteLine("-------------------------------------");
            var dotNetPubs2023 = context.Publications
                .ToList()
                .Where(p => p.Topics.Any(t => t.Name == ".NET Development") && p.PublicationDate.Year == 2023)
                .ToList();

            foreach (var pub in dotNetPubs2023)
            {
                Console.WriteLine($"Title: {pub.Title}");
                Console.WriteLine($"Authors: {string.Join(", ", pub.Authors.Select(a => a.Name))}");
                Console.WriteLine($"Abstract: {pub.Abstract.Substring(0, Math.Min(100, pub.Abstract.Length))}...");
                Console.WriteLine();
            }

            // Example 4: Find publications that cite a specific paper
            Console.WriteLine("Example 4: Citations of 'Implementing Knowledge Graphs in .NET Applications'");
            Console.WriteLine("-------------------------------------------------------------------");
            var targetPub = context.Publications.FirstOrDefault(p => p.Title == "Implementing Knowledge Graphs in .NET Applications");
            if (targetPub != null)
            {
                if (targetPub.CitedBy.Count() == 0)
                {
                    Console.WriteLine("No citations found for this publication.");
                }
                else
                {
                    foreach (var citingPub in targetPub.CitedBy)
                    {
                        Console.WriteLine($"Cited by: {citingPub.Title}");
                        Console.WriteLine($"Authors: {string.Join(", ", citingPub.Authors.Select(a => a.Name))}");
                        Console.WriteLine();
                    }
                }
            }

            // Example 5: Find researchers who have worked together
            Console.WriteLine("Example 5: Co-author Relationships");
            Console.WriteLine("--------------------------------");
            foreach (var person in context.Persons)
            {
                var coauthors = new HashSet<string>();

                foreach (var pub in person.Publications)
                {
                    foreach (var author in pub.Authors)
                    {
                        if (author.Id != person.Id)
                        {
                            coauthors.Add(author.Name);
                        }
                    }
                }

                if (coauthors.Any())
                {
                    Console.WriteLine($"{person.Name} has co-authored with: {string.Join(", ", coauthors)}");
                }
            }
        }
    }
}
