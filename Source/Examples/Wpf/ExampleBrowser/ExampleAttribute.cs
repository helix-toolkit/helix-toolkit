using System;

namespace ExampleBrowser;

[AttributeUsage(AttributeTargets.Class)]
public class ExampleAttribute : Attribute
{
    public ExampleAttribute(string title, string description)
    {
        this.Title = title;
        this.Description = description;
    }

    public string Title { get; }

    public string Description { get; }
}
