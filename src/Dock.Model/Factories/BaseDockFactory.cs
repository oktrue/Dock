﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Dock.Model.Factories
{
    /// <summary>
    /// Dock factory base.
    /// </summary>
    public abstract class BaseDockFactory : IDockFactory
    {
        /// <inheritdoc/>
        public virtual IDictionary<string, Func<object>> ContextLocator { get; set; }

        /// <inheritdoc/>
        public virtual IDictionary<string, Func<IDockHost>> HostLocator { get; set; }

        /// <inheritdoc/>
        public virtual object GetContext(string id, object context)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Func<object> locator = null;
                if (ContextLocator?.TryGetValue(id, out locator) == true)
                {
                    return locator?.Invoke();
                }
            }
            return context;
        }

        /// <inheritdoc/>
        public virtual IDockHost GetHost(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Func<IDockHost> locator = null;
                if (HostLocator?.TryGetValue(id, out locator) == true)
                {
                    return locator?.Invoke();
                }
            }
            throw new Exception($"Dock host with {id} is not registered.");
        }

        /// <inheritdoc/>
        public virtual void Update(IDockWindow window, object context, IDock owner)
        {
            window.Host = GetHost(window.Id);
            window.Context = GetContext(window.Id, context);
            window.Owner = owner;
            window.Factory = this;

            if (window.Layout != null)
            {
                Update(window.Layout, context, null);
            }
        }

        /// <inheritdoc/>
        public virtual void Update(IList<IDockWindow> windows, object context, IDock owner)
        {
            foreach (var window in windows)
            {
                Update(window, context, owner);
            }
        }

        /// <inheritdoc/>
        public virtual void Update(IDock view, object context, IDock parent)
        {
            view.Context = GetContext(view.Id, context);
            view.Parent = parent;
            view.Factory = this;

            if (view.Windows != null)
            {
                Update(view.Windows, context, view);
            }

            if (view.Views != null)
            {
                Update(view.Views, context, view);
            }
        }

        /// <inheritdoc/>
        public virtual void Update(IList<IDock> views, object context, IDock parent)
        {
            foreach (var view in views)
            {
                Update(view, context, parent);
            }
        }

        /// <inheritdoc/>
        public virtual void Remove(IDock dock)
        {
            if (dock.Parent != null)
            {
                dock.Parent.Views?.Remove(dock);
            }
        }

        /// <inheritdoc/>
        public virtual void Move(IDock dock, IDock parent)
        {
            IDock orignalParent = dock.Parent;
            int index = orignalParent.Views.IndexOf(dock);

            orignalParent.Views.Remove(dock);
            parent.Views.Add(dock);

            Update(dock, dock.Context, parent);

            if (orignalParent.Views.Count > 0)
            {
                orignalParent.CurrentView = orignalParent.Views[index > 0 ? index - 1 : 0];
            }

            parent.CurrentView = dock;
        }

        /// <inheritdoc/>
        public virtual void Replace(IDock source, IDock destination)
        {
            if (source.Parent is IDock parent)
            {
                int index = parent.Views.IndexOf(source);
                parent.Views.RemoveAt(index);
                parent.Views.Insert(index, destination);
            }
        }

        /// <inheritdoc/>
        public virtual void Split(IDock dock, DockOperation operation)
        {
            switch (operation)
            {
                case DockOperation.Left:
                case DockOperation.Right:
                case DockOperation.Top:
                case DockOperation.Bottom:
                    {
                        var layout = dock.SplitLayout(dock.Context, operation);
                        if (layout != null)
                        {
                            Replace(dock, layout);
                            Update(layout, dock.Context, dock.Parent);
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException($"Not support dock operation: {operation}.");
            }
        }

        /// <inheritdoc/>
        public virtual void SplitToFill(IDock dock)
        {
            // TODO:
        }

        /// <inheritdoc/>
        public virtual void SplitToLeft(IDock dock)
        {
            Split(dock, DockOperation.Left);
        }

        /// <inheritdoc/>
        public virtual void SplitToRight(IDock dock)
        {
            Split(dock, DockOperation.Left);
        }

        /// <inheritdoc/>
        public virtual void SplitToTop(IDock dock)
        {
            Split(dock, DockOperation.Left);
        }

        /// <inheritdoc/>
        public virtual void SplitToBottom(IDock dock)
        {
            Split(dock, DockOperation.Left);
        }

        /// <inheritdoc/>
        public virtual void SplitToWindow(IDock dock)
        {
            // TODO:
        }

        private void InsertLayout(IDock dock, int index, object context)
        {
            var layout = new DockLayout
            {
                Id = nameof(DockLayout),
                Title = nameof(DockLayout),
                Width = double.NaN,
                Height = double.NaN
            };

            Update(layout, context, dock);

            dock.Views.Insert(index, layout);
        }

        private void InsertRoot(IDock dock, int index, object context)
        {
            var root = new DockRoot
            {
                Id = nameof(DockRoot),
                Title = nameof(DockRoot),
                Width = double.NaN,
                Height = double.NaN
            };

            Update(root, context, dock);

            dock.Views.Insert(index, root);
        }

        private void InsertSplitter(IDock dock, int index, object context)
        {
            var splitter = new DockSplitter
            {
                Id = nameof(DockSplitter),
                Title = nameof(DockSplitter),
                Width = double.NaN,
                Height = double.NaN
            };

            Update(splitter, context, dock);

            dock.Views.Insert(index, splitter);
        }

        private void InsertStrip(IDock dock, int index, object context)
        {
            var strip = new DockStrip
            {
                Id = nameof(DockStrip),
                Title = nameof(DockStrip),
                Width = double.NaN,
                Height = double.NaN
            };

            Update(strip, context, dock);

            dock.Views.Insert(index, strip);
        }

        private void InsertView(IDock dock, int index, object context)
        {
            var view = new DockView
            {
                Id = nameof(DockView),
                Title = nameof(DockView),
                Width = double.NaN,
                Height = double.NaN
            };

            Update(view, context, dock);

            dock.Views.Insert(index, view);
        }

        /// <inheritdoc/>
        public void AddLayout(IDock dock)
        {
            if (dock.Views == null)
            {
                dock.Views = new ObservableCollection<IDock>();
            }
            InsertLayout(dock, dock.Views.Count, dock.Context);
        }

        /// <inheritdoc/>
        public virtual void AddRoot(IDock dock)
        {
            if (dock.Views == null)
            {
                dock.Views = new ObservableCollection<IDock>();
            }
            InsertRoot(dock, dock.Views.Count, dock.Context);
        }

        /// <inheritdoc/>
        public virtual void AddSplitter(IDock dock)
        {
            if (dock.Views == null)
            {
                dock.Views = new ObservableCollection<IDock>();
            }
            InsertSplitter(dock, dock.Views.Count, dock.Context);
        }

        /// <inheritdoc/>
        public virtual void AddStrip(IDock dock)
        {
            if (dock.Views == null)
            {
                dock.Views = new ObservableCollection<IDock>();
            }
            InsertStrip(dock, dock.Views.Count, dock.Context);
        }

        /// <inheritdoc/>
        public virtual void AddView(IDock dock)
        {
            if (dock.Views == null)
            {
                dock.Views = new ObservableCollection<IDock>();
            }
            InsertView(dock, dock.Views.Count, dock.Context);
        }

        /// <inheritdoc/>
        public virtual void InsertLayoutBefore(IDock dock)
        {
            if (dock.Parent is IDock parent)
            {
                int index = parent.Views.IndexOf(dock);
                InsertLayout(parent, index, parent.Context);
            }
        }

        /// <inheritdoc/>
        public virtual void InsertRootBefore(IDock dock)
        {
            if (dock.Parent is IDock parent)
            {
                int index = parent.Views.IndexOf(dock);
                InsertRoot(parent, index, parent.Context);
            }
        }

        /// <inheritdoc/>
        public virtual void InsertSplitterBefore(IDock dock)
        {
            if (dock.Parent is IDock parent)
            {
                int index = parent.Views.IndexOf(dock);
                InsertSplitter(parent, index, parent.Context);
            }
        }

        /// <inheritdoc/>
        public virtual void InsertStripBefore(IDock dock)
        {
            if (dock.Parent is IDock parent)
            {
                int index = parent.Views.IndexOf(dock);
                InsertStrip(parent, index, parent.Context);
            }
        }

        /// <inheritdoc/>
        public virtual void InsertViewBefore(IDock dock)
        {
            if (dock.Parent is IDock parent)
            {
                int index = parent.Views.IndexOf(dock);
                InsertView(parent, index, parent.Context);
            }
        }

        /// <inheritdoc/>
        public virtual void InsertLayoutAfter(IDock dock)
        {
            if (dock.Parent is IDock parent)
            {
                int index = parent.Views.IndexOf(dock) + 1;
                InsertLayout(parent, index, parent.Context);
            }
        }

        /// <inheritdoc/>
        public virtual void InsertRootAfter(IDock dock)
        {
            if (dock.Parent is IDock parent)
            {
                int index = parent.Views.IndexOf(dock) + 1;
                InsertRoot(parent, index, parent.Context);
            }
        }

        /// <inheritdoc/>
        public virtual void InsertSplitterAfter(IDock dock)
        {
            if (dock.Parent is IDock parent)
            {
                int index = parent.Views.IndexOf(dock) + 1;
                InsertSplitter(parent, index, parent.Context);
            }
        }

        /// <inheritdoc/>
        public virtual void InsertStripAfter(IDock dock)
        {
            if (dock.Parent is IDock parent)
            {
                int index = parent.Views.IndexOf(dock) + 1;
                InsertStrip(parent, index, parent.Context);
            }
        }

        /// <inheritdoc/>
        public virtual void InsertViewAfter(IDock dock)
        {
            if (dock.Parent is IDock parent)
            {
                int index = parent.Views.IndexOf(dock) + 1;
                InsertView(parent, index, parent.Context);
            }
        }

        /// <summary>
        /// Copies properties from source to destination dock.
        /// </summary>
        /// <param name="source">The source dock.</param>
        /// <param name="destination">The destination dock.</param>
        /// <param name="bCopyViews">The flag indicating whether to copy source views.</param>
        /// <param name="bCopyWindows">The flag indicating whether to copy source windows.</param>
        private void Copy(IDock source, IDock destination, bool bCopyViews, bool bCopyWindows)
        {
            destination.Id = source.Id;
            destination.Dock = source.Dock;
            destination.Width = source.Width;
            destination.Height = source.Height;
            destination.Title = source.Title;

            if (bCopyViews)
            {
                destination.Views = source.Views;
                destination.CurrentView = source.CurrentView;
                destination.DefaultView = source.DefaultView;
            }

            if (bCopyWindows)
            {
                destination.Windows = source.Windows;
            }
        }

        /// <inheritdoc/>
        public virtual void ConvertToLayout(IDock dock)
        {
            var layout = new DockLayout();
            Copy(dock, layout, true, true);
            Update(layout, dock.Context, dock.Parent);
            Replace(dock, layout);
        }

        /// <inheritdoc/>
        public virtual void ConvertToRoot(IDock dock)
        {
            var layout = new DockRoot();
            Copy(dock, layout, true, true);
            Update(layout, dock.Context, dock.Parent);
            Replace(dock, layout);
        }

        /// <inheritdoc/>
        public virtual void ConvertToSplitter(IDock dock)
        {
            var layout = new DockSplitter();
            Copy(dock, layout, false, false);
            Update(layout, dock.Context, dock.Parent);
            Replace(dock, layout);
        }

        /// <inheritdoc/>
        public virtual void ConvertToStrip(IDock dock)
        {
            var layout = new DockStrip();
            Copy(dock, layout, true, false);
            Update(layout, dock.Context, dock.Parent);
            Replace(dock, layout);
        }

        /// <inheritdoc/>
        public virtual void ConvertToView(IDock dock)
        {
            var layout = new DockView();
            Copy(dock, layout, false, true);
            Update(layout, dock.Context, dock.Parent);
            Replace(dock, layout);
        }

        /// <inheritdoc/>
        public virtual IDockWindow CreateWindow(IDock target, IDock dock, object context)
        {
            var strip = new DockStrip
            {
                Id = nameof(DockStrip),
                Title = nameof(DockStrip),
                CurrentView = dock,
                Views = new ObservableCollection<IDock> { dock }
            };

            var root = new DockRoot
            {
                Id = nameof(DockRoot),
                Title = nameof(DockRoot),
                CurrentView = strip,
                DefaultView = strip,
                Views = new ObservableCollection<IDock> { strip  }
            };

            var window = new DockWindow()
            {
                Id = nameof(DockWindow),
                Title = nameof(DockWindow),
                Layout = root
            };

            target.AddWindow(window);

            Update(window, context, target);

            return window;
        }

        /// <inheritdoc/>
        public abstract IDock CreateLayout();

        /// <inheritdoc/>
        public abstract void InitLayout(IDock layout, object context);
    }
}
