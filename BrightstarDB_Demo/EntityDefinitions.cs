using System;
using System.Collections.Generic;
using BrightstarDB.EntityFramework;

namespace BrightstarDBDemo
{
    [Entity]
    public interface IPerson
    {
        [Identifier("http://example.org/people/")]
        string Id { get; }

        string Name { get; set; }
        string Email { get; set; }
        string Organization { get; set; }

        ICompany Employer { get; set; }

        [InverseProperty("Authors")] 
        ICollection<IPublication> Publications { get; }
    }

    [Entity]
    public interface ICompany
    {
        [Identifier("http://example.org/companies/")]
        string Id { get; }

        string Name { get; set; }

        [InverseProperty("Employer")]
        ICollection<IPerson> Employees { get; }
    }

    [Entity]
    public interface IPublication
    {
        [Identifier("http://example.org/publications/")]
        string Id { get; }

        string Title { get; set; }
        DateTime PublicationDate { get; set; }
        string Abstract { get; set; }

        ICollection<IPerson> Authors { get; set; }
        ICollection<ITopic> Topics { get; set; }
        ICollection<IPublication> References { get; set; }

        [InverseProperty("References")]
        ICollection<IPublication> CitedBy { get; }
    }

    [Entity]
    public interface ITopic
    {
        [Identifier("http://example.org/topics/")]
        string Id { get; }

        string Name { get; set; }
        string Description { get; set; }

        ITopic ParentTopic { get; set; }

        [InverseProperty("ParentTopic")]
        ICollection<ITopic> SubTopics { get; }

        [InverseProperty("Topics")]
        ICollection<IPublication> Publications { get; }
    }
}