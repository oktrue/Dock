﻿using System.Runtime.Serialization;
using Dock.Model.Adapters;
using Dock.Model.Core;

namespace Dock.Model.Mvvm.Core;

/// <summary>
/// Dockable base class.
/// </summary>
[DataContract(IsReference = true)]
public abstract class DockableBase : ReactiveBase, IDockable
{
    private readonly TrackingAdapter _trackingAdapter;
    private string _id = string.Empty;
    private string _title = string.Empty;
    private object? _context;
    private IDockable? _owner;
    private IDockable? _originalOwner;
    private IFactory? _factory;
    private bool _isEmpty;
    private bool _isCollapsable = true;
    private double _proportion = double.NaN;
    private bool _canClose = true;
    private bool _canPin = true;
    private bool _canFloat = true;

    /// <summary>
    /// Initializes new instance of the <see cref="DockableBase"/> class.
    /// </summary>
    protected DockableBase()
    {
        _trackingAdapter = new TrackingAdapter();
    }

    /// <inheritdoc/>
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    /// <inheritdoc/>
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <inheritdoc/>
    [IgnoreDataMember]
    public object? Context
    {
        get => _context;
        set => SetProperty(ref _context, value);
    }

    /// <inheritdoc/>
    [IgnoreDataMember]
    public IDockable? Owner
    {
        get => _owner;
        set => SetProperty(ref _owner, value);
    }

    /// <inheritdoc/>
    [IgnoreDataMember]
    public IDockable? OriginalOwner
    {
        get => _originalOwner;
        set => SetProperty(ref _originalOwner, value);
    }

    /// <inheritdoc/>
    [IgnoreDataMember]
    public IFactory? Factory
    {
        get => _factory;
        set => SetProperty(ref _factory, value);
    }

    /// <inheritdoc/>
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsEmpty
    {
        get => _isEmpty;
        set => SetProperty(ref _isEmpty, value);
    }

    /// <inheritdoc/>
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsCollapsable
    {
        get => _isCollapsable;
        set => SetProperty(ref _isCollapsable, value);
    }

    /// <inheritdoc/>
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public double Proportion
    {
        get => _proportion;
        set => SetProperty(ref _proportion, value);
    }

    /// <inheritdoc/>
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool CanClose
    {
        get => _canClose;
        set => SetProperty(ref _canClose, value);
    }

    /// <inheritdoc/>
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool CanPin
    {
        get => _canPin;
        set => SetProperty(ref _canPin, value);
    }

    /// <inheritdoc/>
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool CanFloat
    {
        get => _canFloat;
        set => SetProperty(ref _canFloat, value);
    }

    /// <inheritdoc/>
    public string? GetControlRecyclingId() => _id;

    /// <inheritdoc/>
    public virtual bool OnClose()
    {
        return true;
    }

    /// <inheritdoc/>
    public virtual void OnSelected()
    {
    }

    /// <inheritdoc/>
    public void GetVisibleBounds(out double x, out double y, out double width, out double height)
    {
        _trackingAdapter.GetVisibleBounds(out x, out y, out width, out height);
    }

    /// <inheritdoc/>
    public void SetVisibleBounds(double x, double y, double width, double height)
    {
        _trackingAdapter.SetVisibleBounds(x, y, width, height);
        OnVisibleBoundsChanged(x, y, width, height);
    }

    /// <inheritdoc/>
    public virtual void OnVisibleBoundsChanged(double x, double y, double width, double height)
    {
    }

    /// <inheritdoc/>
    public void GetPinnedBounds(out double x, out double y, out double width, out double height)
    {
        _trackingAdapter.GetPinnedBounds(out x, out y, out width, out height);
    }

    /// <inheritdoc/>
    public void SetPinnedBounds(double x, double y, double width, double height)
    {
        _trackingAdapter.SetPinnedBounds(x, y, width, height);
        OnPinnedBoundsChanged(x, y, width, height);
    }

    /// <inheritdoc/>
    public virtual void OnPinnedBoundsChanged(double x, double y, double width, double height)
    {
    }

    /// <inheritdoc/>
    public void GetTabBounds(out double x, out double y, out double width, out double height)
    {
        _trackingAdapter.GetTabBounds(out x, out y, out width, out height);
    }

    /// <inheritdoc/>
    public void SetTabBounds(double x, double y, double width, double height)
    {
        _trackingAdapter.SetTabBounds(x, y, width, height);
        OnTabBoundsChanged(x, y, width, height);
    }

    /// <inheritdoc/>
    public virtual void OnTabBoundsChanged(double x, double y, double width, double height)
    {
    }

    /// <inheritdoc/>
    public void GetPointerPosition(out double x, out double y)
    {
        _trackingAdapter.GetPointerPosition(out x, out y);
    }

    /// <inheritdoc/>
    public void SetPointerPosition(double x, double y)
    {
        _trackingAdapter.SetPointerPosition(x, y);
        OnPointerPositionChanged(x, y);
    }

    /// <inheritdoc/>
    public virtual void OnPointerPositionChanged(double x, double y)
    {
    }

    /// <inheritdoc/>
    public void GetPointerScreenPosition(out double x, out double y)
    {
        _trackingAdapter.GetPointerScreenPosition(out x, out y);
    }

    /// <inheritdoc/>
    public void SetPointerScreenPosition(double x, double y)
    {
        _trackingAdapter.SetPointerScreenPosition(x, y);
        OnPointerScreenPositionChanged(x, y);
    }

    /// <inheritdoc/>
    public virtual void OnPointerScreenPositionChanged(double x, double y)
    {
    }
}
