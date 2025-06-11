using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace CorparateMessenger.Tools
{
    // Не работает
    public class SimpleScrollBehavior : Behavior<ListBox>
    {
        private ScrollViewer _scrollViewer;
        private object _lastVisibleItem;
        private double _lastScrollOffset;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;

            if (AssociatedObject.Items.SourceCollection is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnCollectionChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = FindScrollViewer(AssociatedObject);
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged += OnScrollChanged;
                _scrollViewer.ScrollToBottom();
            }
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_scrollViewer.VerticalOffset < _scrollViewer.ScrollableHeight)
            {
                _lastVisibleItem = GetFirstVisibleItem();
                _lastScrollOffset = e.VerticalOffset;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_scrollViewer == null) return;

            Dispatcher.BeginInvoke(() =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (e.NewStartingIndex == 0 && _lastVisibleItem != null)
                    {
                        var container = AssociatedObject
                            .ItemContainerGenerator
                            .ContainerFromItem(_lastVisibleItem) as FrameworkElement;

                        if (container != null)
                        {
                            var position = container.TransformToVisual(_scrollViewer)
                                .Transform(new Point(0, 0));
                            _scrollViewer.ScrollToVerticalOffset(_lastScrollOffset + position.Y);
                        }
                    }
                    else if (e.NewStartingIndex + e.NewItems.Count == AssociatedObject.Items.Count)
                    {
                        _scrollViewer.ScrollToBottom();
                    }
                }
            }, DispatcherPriority.Render);
        }

        private object GetFirstVisibleItem()
        {
            if (_scrollViewer == null) return null;

            for (int i = 0; i < AssociatedObject.Items.Count; i++)
            {
                var item = AssociatedObject.Items[i];
                var container = AssociatedObject.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (container == null) continue;

                var position = container.TransformToVisual(_scrollViewer).Transform(new Point(0, 0));
                if (position.Y + container.ActualHeight >= _scrollViewer.VerticalOffset)
                    return item;
            }
            return null;
        }

        private ScrollViewer FindScrollViewer(DependencyObject obj)
        {
            if (obj is ScrollViewer viewer) return viewer;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                if (FindScrollViewer(VisualTreeHelper.GetChild(obj, i)) is ScrollViewer result)
                    return result;
            return null;
        }
    }
}