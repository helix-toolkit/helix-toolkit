﻿using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;
using System.Collections;
using System.Collections.Specialized;

namespace HelixToolkit.WinUI.SharpDX;

/// <summary>
///     Represents a model that can be used to present a collection of items. supports generating child items by a
///     <see cref="DataTemplate" />.
/// </summary>
/// <remarks>
///     Use the ItemsSource property to specify the collection to use to generate the content of your ItemsControl. You can set the ItemsSource
///     property to any type that implements IEnumerable. ItemsSource is typically used to display a data collection or to bind an
///     ItemsControl to a collection object.
/// </remarks>
public class ItemsModel3D : CompositeModel3D
{
    /// <summary>
    ///     The item template property
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
        "ItemTemplate", typeof(DataTemplate), typeof(ItemsModel3D), new PropertyMetadata(null));

    /// <summary>
    /// ItemsSource for binding to collection. Please use ObservableElement3DCollection for observable, otherwise may cause memory leak.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        "ItemsSource", typeof(IEnumerable), typeof(ItemsModel3D), new PropertyMetadata(null, (s, e) =>
                {
                    if (s is ItemsModel3D itemsModel && itemsModel.IsAttached)
                    {
                        itemsModel.OnItemsSourceChanged(e.NewValue as IEnumerable);
                    }
                }));

    /// <summary>
    /// Add octree manager to use octree hit test.
    /// </summary>
    public static readonly DependencyProperty OctreeManagerProperty = DependencyProperty.Register("OctreeManager",
        typeof(IOctreeManagerWrapper),
        typeof(ItemsModel3D), new PropertyMetadata(null, (s, e) =>
        {
            var d = s as ItemsModel3D;
            if (e.OldValue is Element3D elem_old)
            {
                d?.Items.Remove(elem_old);
            }

            if (e.NewValue is Element3D elem)
            {
                d?.Items.Add(elem);
            }

            if (d?.SceneNode is GroupNode node)
            {
                node.OctreeManager = e.NewValue == null ? null : (e.NewValue as IOctreeManagerWrapper)?.Manager;
            }
        }));

    /// <summary>
    ///     Gets or sets the <see cref="DataTemplate" /> used to display each item.
    /// </summary>
    /// <value>
    ///     The item template.
    /// </value>
    public DataTemplate? ItemTemplate
    {
        get
        {
            return (DataTemplate?)this.GetValue(ItemTemplateProperty);
        }
        set
        {
            this.SetValue(ItemTemplateProperty, value);
        }
    }

    public DataTemplateSelector ItemTemplateSelector
    {
        get
        {
            return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty);
        }
        set
        {
            SetValue(ItemTemplateSelectorProperty, value);
        }
    }

    // Using a DependencyProperty as the backing store for ItemTemplateSelector.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ItemTemplateSelectorProperty =
        DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(ItemsModel3D), new PropertyMetadata(new DataTemplateSelector()));

    /// <summary>
    ///     Gets or sets a collection used to generate the content of the <see cref="ItemsModel3D" />.
    /// </summary>
    /// <value>
    ///     The items source.
    /// </value>
    public IEnumerable? ItemsSource
    {
        get
        {
            return (IEnumerable?)this.GetValue(ItemsSourceProperty);
        }
        set
        {
            this.SetValue(ItemsSourceProperty, value);
        }
    }

    public IOctreeManagerWrapper? OctreeManager
    {
        set
        {
            SetValue(OctreeManagerProperty, value);
        }
        get
        {
            return (IOctreeManagerWrapper?)GetValue(OctreeManagerProperty);
        }
    }

    private IOctreeBasic? Octree => (SceneNode as GroupNode)?.OctreeManager?.Octree;

    private readonly ItemsControl itemsControl = new();

    private IEnumerable? itemsSourceInternal;

    public ItemCollection Items => itemsControl.Items;

    private readonly Dictionary<object, Element3D> elementDict = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupElement3D"/> class.
    /// </summary>
    public ItemsModel3D()
    {
        Children.CollectionChanged += Items_CollectionChanged;
        SceneNode.Attached += SceneNode_Attached;
        SceneNode.Detached += SceneNode_Detached;
    }

    private void SceneNode_Attached(object? sender, EventArgs e)
    {
        if (ItemsSource != null)
        {
            OnItemsSourceChanged(ItemsSource);
        }
    }

    private void SceneNode_Detached(object? sender, EventArgs e)
    {
        if (itemsSourceInternal != null)
        {
            OnItemsSourceChanged(null);
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        AttachChild(itemsControl);
        Items.Clear();
        foreach (var item in Children)
        {
            if (item.Parent != this)
            {
                Items.Add(item);
            }
        }
        if (OctreeManager is Element3D elem)
        {
            Items.Add(elem);
        }
    }

    /// <summary>
    /// Called when [create scene node].
    /// </summary>
    /// <returns></returns>
    protected override SceneNode OnCreateSceneNode()
    {
        return new GroupNode() { AlwaysHittable = AlwaysHittable };
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            DetachChildren(e.OldItems);
        }
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            Items?.Clear();
            var node = SceneNode as GroupNode;
            node?.Clear();
            AttachChildren(sender as IList);
            if (OctreeManager is Element3D elem)
            {
                Items?.Add(elem);
            }
        }
        else if (e.NewItems != null)
        {
            AttachChildren(e.NewItems);
        }
    }

    /// <summary>
    /// Attaches the children.
    /// </summary>
    /// <param name="children">The children.</param>
    protected void AttachChildren(IList? children)
    {
        if (children is null)
        {
            return;
        }

        if (SceneNode is not GroupNode node)
        {
            return;
        }

        foreach (Element3D c in children)
        {
            if (node.AddChildNode(c.SceneNode))
            {
                Items?.Add(c);
            }
        }
    }
    /// <summary>
    /// Detaches the children.
    /// </summary>
    /// <param name="children">The children.</param>
    protected void DetachChildren(IList? children)
    {
        if (children is null)
        {
            return;
        }

        if (SceneNode is not GroupNode node)
        {
            return;
        }

        foreach (Element3D c in children)
        {
            if (node.RemoveChildNode(c.SceneNode))
            {
                Items?.Remove(c);
            }
        }
    }

    private void OnItemsSourceChanged(IEnumerable? itemsSource)
    {
        if (itemsSourceInternal == itemsSource)
        { return; }
        if (itemsSourceInternal is INotifyCollectionChanged o)
        {
            o.CollectionChanged -= ItemsModel3D_CollectionChanged;
        }
        foreach (Element3D item in Children)
        {
            item.DataContext = null;
        }
        if (itemsSourceInternal == null && itemsSource != null && Children.Count > 0)
        {
            throw new InvalidOperationException("Children must be empty before using ItemsSource");
        }
        Clear();
        itemsSourceInternal = itemsSource;
        if (itemsSource == null)
        {
            return;
        }

        if (itemsSource is INotifyCollectionChanged n)
        {
            n.CollectionChanged -= ItemsModel3D_CollectionChanged;
            n.CollectionChanged += ItemsModel3D_CollectionChanged;
        }

        AddItems(itemsSource);

        if (Children.Count > 0)
        {
            var groupNode = SceneNode as GroupNode;
            groupNode?.OctreeManager?.RequestRebuild();
        }
    }

    protected void ItemsModel3D_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (elementDict.TryGetValue(item, out Element3D? element))
                        {
                            Children.Remove(element);
                            elementDict.Remove(item);
                        }
                    }
                    InvalidateRender();
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                Clear();
                break;
        }

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                AddItems(ItemsSource);
                InvalidateRender();
                break;
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Replace:
                AddItems(e.NewItems);
                break;
        }
    }

    private void AddItems(IEnumerable? items)
    {
        if (items is null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (this.ItemTemplate != null)
            {
                AddFromTemplate(item, this.ItemTemplate);
            }
            else if (this.ItemTemplateSelector != null)
            {
                DataTemplate template = this.ItemTemplateSelector.SelectTemplate(item, this);

                if (template != null)
                {
                    AddFromTemplate(item, template);
                }
                else
                {
                    throw new InvalidOperationException("No template for item.");
                }
            }
            else
            {
                if (item is Element3D model)
                {
                    this.Children.Add(model);
                    elementDict.Add(item, model);
                }
                else
                {
                    throw new InvalidOperationException("Item is not a Model3D.");
                }
            }
        }
    }

    private void AddFromTemplate(object item, DataTemplate template)
    {
        if (template.LoadContent() is Element3D model)
        {
            model.DataContext = item;

            this.Children.Add(model);

            elementDict.Add(item, model);
        }
        else
        {
            throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
        }
    }

    public virtual void Clear()
    {
        elementDict.Clear();
        var node = SceneNode as GroupNode;
        node?.Clear();
        Children.Clear();
    }
}
