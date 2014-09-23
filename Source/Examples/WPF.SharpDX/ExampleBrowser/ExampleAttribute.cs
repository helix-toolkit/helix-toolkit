// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleAttribute.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ExampleBrowser
{
    using System;

    public class ExampleAttribute : Attribute
    {
        public ExampleAttribute(string title, string description)
        {
            this.Title = title;
            this.Description = description;
        }

        public string Title { get; private set; }

        public string Description { get; private set; }
    }
}