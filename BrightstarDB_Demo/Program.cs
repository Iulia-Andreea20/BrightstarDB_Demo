using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BrightstarDB.EntityFramework;
using BrightstarDB_Demo;

namespace BrightstarDBDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BrightstarDB Academic Knowledge Graph Demo");
            Console.WriteLine("==========================================");

            // Define the store location - using a file store for simplicity
            string dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BrightstarDemo");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            string connectionString = $"type=embedded;storesdirectory={dataFolder};storename=AcademicKnowledgeGraph";

            try
            {
                // Create our context
                Console.WriteLine("Creating/Opening BrightstarDB store...");
                var context = new MyEntityContext(connectionString);

                // FEATURE 2: Add SavingChanges event handler
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

                // Demonstrate some LINQ queries
                PerformQueries(context);

                // FEATURE 1: Demonstrate Hierarchical Key Pattern
                DemonstrateHierarchicalKeys(context);

                // FEATURE 3: Demonstrate Entity Casting with Become<T>
                DemonstrateEntityCasting(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        // FEATURE 2: Event handler for SavingChanges
        private static void UpdateTrackables(object sender, EventArgs e)
        {
            try
            {
                // This method is invoked by the context before saving changes
                var context = sender as MyEntityContext;

                // Iterate through the tracked objects that implement IPublication
                foreach (var obj in context.TrackedObjects.Where(x => x is IPublication))
                {
                    var pub = obj as IPublication;

                    // If this is a new entity (Created is default value)
                    if (pub.Created == DateTime.MinValue)
                    {
                        pub.Created = DateTime.Now;
                        Console.WriteLine($"Setting Created timestamp on '{pub.Title}' to {pub.Created}");
                    }

                    // Always update the LastModified property
                    pub.LastModified = DateTime.Now;
                    Console.WriteLine($"Setting LastModified timestamp on '{pub.Title}' to {pub.LastModified}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateTrackables: {ex.Message}");
            }
        }

        // FEATURE 1: Demonstrate Hierarchical Key Pattern
        static void DemonstrateHierarchicalKeys(MyEntityContext context)
        {
            Console.WriteLine("\n--- Demonstrating Hierarchical Key Pattern ---\n");

            // Find the Semantic Web topic (which should be a child of Knowledge Graphs)
            var semWeb = context.Topics.Where(t => t.Name == "Semantic Web").FirstOrDefault();

            if (semWeb != null && semWeb.ParentTopic != null)
            {
                Console.WriteLine($"Topic: {semWeb.Name}");
                Console.WriteLine($"ID: {semWeb.Id}");
                Console.WriteLine($"Parent Topic: {semWeb.ParentTopic.Name}");
                Console.WriteLine($"Parent ID: {semWeb.ParentTopic.Id}");

                // Create a new child topic that will inherit the parent's ID in its own ID
                var sparql = context.Topics.Create();
                sparql.Name = "SPARQL";
                sparql.Description = "SPARQL Protocol and RDF Query Language";
                sparql.ParentTopic = semWeb;

                context.SaveChanges();

                Console.WriteLine($"Created new child topic: {sparql.Name}");
                Console.WriteLine($"Child ID: {sparql.Id}");
                Console.WriteLine("The child ID contains the parent's ID as a prefix, demonstrating hierarchical keys");
            }
        }

        // FEATURE 3: Demonstrate Entity Casting with Become<T>
        static void DemonstrateEntityCasting(MyEntityContext context)
        {
            Console.WriteLine("\n--- Demonstrating Entity Casting ---\n");

            // Get a person entity
            var bob = context.Persons.Where(p => p.Name == "Bob Jones").FirstOrDefault();

            if (bob != null)
            {
                Console.WriteLine($"Original person: {bob.Name}");
                Console.WriteLine($"Entity type: {bob.GetType().Name}");
                Console.WriteLine($"ID: {bob.Id}");

                try
                {
                    // Access the underlying entity object
                    // This is a safer approach that works with the generated classes
                    var entity = bob as BrightstarEntityObject;
                    if (entity != null)
                    {
                        // Cast the person to a researcher
                        var researcher = entity.Become<IResearcher>();

                        // Add researcher-specific properties
                        researcher.HIndex = 25;
                        researcher.ResearchField = "Knowledge Graph Engineering";

                        context.SaveChanges();

                        Console.WriteLine($"Transformed to researcher: {researcher.Name}");
                        Console.WriteLine($"H-Index: {researcher.HIndex}");
                        Console.WriteLine($"Research Field: {researcher.ResearchField}");
                        Console.WriteLine($"Entity now has additional properties in the RDF store");
                    }
                    else
                    {
                        Console.WriteLine("Could not access entity object for casting");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Entity casting failed: {ex.Message}");

                    // Alternative approach: Create a new entity with the same ID
                    Console.WriteLine("Using alternative approach to demonstrate extending an entity:");

                    // Get the ID of the existing person
                    string bobId = bob.Id;

                    // Create a researcher with specific properties
                    var researcher = context.Persons.Create();
                    researcher.Name = bob.Name + " (as Researcher)";
                    researcher.Email = bob.Email;
                    researcher.Organization = bob.Organization;

                    // Save the additional entity
                    context.SaveChanges();

                    Console.WriteLine($"Created a new entity representing {researcher.Name}");
                    Console.WriteLine("This demonstrates how entities can have multiple representations");
                }
            }
        }

        static void PopulateStore(MyEntityContext context)
        {
            // Your existing PopulateStore method remains the same...
            // Create topics (research areas)
            ITopic knowledgeGraphs = context.Topics.Create();
            knowledgeGraphs.Name = "Knowledge Graphs";
            knowledgeGraphs.Description = "Representation and management of structured knowledge using graph-based approaches";

            ITopic semanticWeb = context.Topics.Create();
            semanticWeb.Name = "Semantic Web";
            semanticWeb.Description = "Web of data that enables machines to understand the semantics of information";
            semanticWeb.ParentTopic = knowledgeGraphs;

            ITopic rdfDatabases = context.Topics.Create();
            rdfDatabases.Name = "RDF Databases";
            rdfDatabases.Description = "Database systems optimized for storing and querying RDF data";
            rdfDatabases.ParentTopic = semanticWeb;

            ITopic dotNet = context.Topics.Create();
            dotNet.Name = ".NET Development";
            dotNet.Description = "Development of applications using Microsoft's .NET framework";

            // Create companies
            ICompany brightstarCompany = context.Companies.Create();
            brightstarCompany.Name = "BrightstarDB";

            ICompany research = context.Companies.Create();
            research.Name = "Research Institute of Technology";

            // Create authors
            IPerson alice = context.Persons.Create();
            alice.Name = "Alice Smith";
            alice.Email = "alice.smith@example.org";
            alice.Organization = "University of Example";
            alice.Employer = research;

            IPerson bob = context.Persons.Create();
            bob.Name = "Bob Jones";
            bob.Email = "bob.jones@example.org";
            bob.Organization = "Research Institute of Technology";
            bob.Employer = research;

            IPerson charlie = context.Persons.Create();
            charlie.Name = "Charlie Brown";
            charlie.Email = "charlie.brown@example.org";
            charlie.Organization = "BrightstarDB";
            charlie.Employer = brightstarCompany;

            // Create publications
            IPublication pub1 = context.Publications.Create();
            pub1.Title = "Implementing Knowledge Graphs in .NET Applications";
            pub1.PublicationDate = new DateTime(2023, 6, 15);
            pub1.Abstract = "This paper explores approaches to implementing knowledge graphs in .NET applications, with a focus on BrightstarDB.";
            pub1.Authors = new List<IPerson> { alice, bob };
            pub1.Topics = new List<ITopic> { knowledgeGraphs, dotNet };

            IPublication pub2 = context.Publications.Create();
            pub2.Title = "Introduction to Semantic Web Technologies";
            pub2.PublicationDate = new DateTime(2022, 11, 10);
            pub2.Abstract = "An overview of semantic web technologies and standards including RDF, OWL, and SPARQL.";
            pub2.Authors = new List<IPerson> { alice };
            pub2.Topics = new List<ITopic> { semanticWeb };

            IPublication pub3 = context.Publications.Create();
            pub3.Title = "BrightstarDB: A Case Study in RDF Database Implementation";
            pub3.PublicationDate = new DateTime(2023, 2, 28);
            pub3.Abstract = "This case study examines the architecture and performance of BrightstarDB for enterprise applications.";
            pub3.Authors = new List<IPerson> { bob, charlie };
            pub3.Topics = new List<ITopic> { rdfDatabases, dotNet };
            pub3.References = new List<IPublication> { pub1 };

            // Save all changes to the store
            context.SaveChanges();
            Console.WriteLine("Sample data created successfully!");
        }

        static void PerformQueries(MyEntityContext context)
        {
            // Your existing PerformQueries method remains the same...
            Console.WriteLine("\n--- LINQ Query Examples ---\n");

            // Example 1: Find all publications about Knowledge Graphs
            Console.WriteLine("Example 1: Publications about Knowledge Graphs");
            Console.WriteLine("------------------------------------------");
            var kgTopic = context.Topics.Where(t => t.Name == "Knowledge Graphs").FirstOrDefault();
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
            var alice = context.Persons.Where(p => p.Name == "Alice Smith").FirstOrDefault();
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
            var targetPub = context.Publications.Where(p => p.Title == "Implementing Knowledge Graphs in .NET Applications").FirstOrDefault();
            if (targetPub != null)
            {
                if (targetPub.CitedBy.Count() == 0)
                {
                    foreach (var citingPub in targetPub.CitedBy)
                    {
                        Console.WriteLine($"Cited by: {citingPub.Title}");
                        Console.WriteLine($"Authors: {string.Join(", ", citingPub.Authors.Select(a => a.Name))}");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("No citations found for this publication.");
                }
            }

            // Example 5: Find researchers who have worked together
            Console.WriteLine("Example 5: Co-author Relationships");
            Console.WriteLine("--------------------------------");
            var coauthorships = new Dictionary<string, List<string>>();

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
