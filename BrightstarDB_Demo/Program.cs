using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BrightstarDB.Client;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void PopulateStore(MyEntityContext context)
        {
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