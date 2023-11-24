using DependencyPropertyGenerator;
using HelixToolkit.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace DataTemplate;

/// <summary>
///     Represents a model that can be used to present a collection of items.supports generating child items by a
///     <see cref="DataTemplate3D" />.
/// </summary>
/// <remarks>
///     Use the ItemsSource property to specify the collection to use to generate the content of your ItemsVisual3D. You can set the ItemsSource
///     property to any type that implements IEnumerable. ItemsSource is typically used to display a data collection or to bind an
///     ItemsControl to a collection object.
/// </remarks>
[DependencyProperty<DataTemplate3D>("ItemTemplate")]
[DependencyProperty<DataTemplateSelector3D>("ItemTemplateSelector", DefaultValueExpression = "new DefaultDataTemplateSelctor3D()")]
[DependencyProperty<IEnumerable>("ItemsSource")]
[DependencyProperty<bool>("RefreshChildrenOnChange", DefaultValue = true)]
public partial class ItemsVisual3D : ModelVisual3D
{
    /// <summary>
    /// Keeps track of the visuals created for each item.
    /// </summary>
    private readonly Dictionary<object, Visual3D> visuals = new();

    /// <summary>
    /// Handles changes in the ItemsSource property.
    /// </summary>
    /// <param name="oldValue">
    /// </param>
    /// <param name="newValue">
    /// </param>
    /// <exception cref="System.InvalidOperationException">
    /// Cannot create a Model3D from ItemTemplate.
    /// </exception>
    partial void OnItemsSourceChanged(IEnumerable? oldValue, IEnumerable? newValue)
    {
        if (oldValue is INotifyCollectionChanged oldObservableCollection)
        {
            oldObservableCollection.CollectionChanged -= this.CollectionChanged;
        }

        if (newValue is INotifyCollectionChanged observableCollection)
        {
            observableCollection.CollectionChanged += this.CollectionChanged;
        }

        if (this.ItemsSource != null)
        {
            AddItems(this.ItemsSource);
        }

        if (RefreshChildrenOnChange)
        {
            RefreshChildren();
        }
    }

    /// <summary>
    /// Re-attaches the instance to the viewport resulting in a refresh.
    /// </summary>
    public void RefreshChildren()
    {
        var viewPort = Visual3DHelper.GetViewport3D(this);

        if (viewPort is null)
        {
            return;
        }

        var index = viewPort.Children.IndexOf(this);
        viewPort.Children.Remove(this);
        viewPort.Children.Insert(index, this);
    }

    private void AddItems(IEnumerable? items)
    {
        if (items != null && items.Cast<object>().Any())
        {
            foreach (var item in items)
                AddItem(item);
        }
    }

    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AddItems(e.NewItems);
                break;

            case NotifyCollectionChangedAction.Remove:
                RemoveItems(e.OldItems);
                break;

            case NotifyCollectionChangedAction.Replace:
                RemoveItems(e.OldItems);
                AddItems(e.NewItems);
                break;

            case NotifyCollectionChangedAction.Reset:
                this.Children.Clear();
                this.visuals.Clear();

                if (ItemsSource is not null)
                {
                    this.AddItems(ItemsSource);
                }

                break;

            default:
                break;
        }

        if (RefreshChildrenOnChange)
            RefreshChildren();
    }

    private void AddItem(object item)
    {
        var visual = CreateVisualFromModel(item);
        if (visual != null)
        {
            // Cannot set DataContext, set bindings manually
            // http://stackoverflow.com/questions/7725313/how-can-i-use-databinding-for-3d-elements-like-visual3d-or-uielement3d
            this.Children.Add(visual);

            this.visuals[item] = visual;
        }
        else
        {
            throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
        }
    }
    private void RemoveItems(IEnumerable? items)
    {
        if (items == null)
            return;

        foreach (var rem in items)
        {
            if (visuals.ContainsKey(rem))
            {
                if (visuals[rem] != null)
                {
                    Children.Remove(visuals[rem]);
                }
            }
        }
    }

    private Visual3D? CreateVisualFromModel(object item)
    {
        if (this.ItemTemplate != null)
        {
            return this.ItemTemplate.CreateItem(item);
        }
        else if (ItemTemplateSelector != null)
        {
            var viewPort = Visual3DHelper.GetViewport3D(this);
            var dataTemplate = ItemTemplateSelector.SelectTemplate(item, viewPort);
            if (dataTemplate != null)
            {
                return dataTemplate.CreateItem(item);
            }
            else
            {
                return item as Visual3D;
            }
        }
        else
        {
            return item as Visual3D;
        }
    }
}
