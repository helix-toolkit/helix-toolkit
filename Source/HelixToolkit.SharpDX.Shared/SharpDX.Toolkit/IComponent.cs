/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
namespace SharpDX
{
    /// <summary>
    /// Base interface for a component base.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Gets the name of this component.
        /// </summary>
        /// <value>The name.</value>
        string Name
        {
            get; set;
        }
    }
}