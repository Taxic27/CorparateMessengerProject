using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Media;
using System.Collections.Specialized;
using System.Windows.Threading;
using CorparateMessenger.ViewModels;

namespace CorparateMessenger.Tools
{
    public class SimpleScrollBehavior : Behavior<ListBox>
    {
        private ScrollViewer _scrollViewer;
        private double _previousScrollOffset;
        private object _lastItemBeforeLoad;
        private bool _isScrollingProgrammatically;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;

            if (AssociatedObject.Items.SourceCollection is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnCollectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnLoaded;

            if (_scrollViewer != null)
                _scrollViewer.ScrollChanged -= OnScrollChanged;

            if (AssociatedObject.Items.SourceCollection is INotifyCollectionChanged collection)
                collection.CollectionChanged -= OnCollectionChanged;
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
            if (_isScrollingProgrammatically)
            {
                _isScrollingProgrammatically = false;
                return;
            }

           
            _previousScrollOffset = e.VerticalOffset;

            //if (e.VerticalChange < 0 && e.VerticalOffset < 100 &&
            //    AssociatedObject.DataContext is MessagesViewModel vm)
            //{
            //    _lastItemBeforeLoad = AssociatedObject.Items.Count > 0 ?
            //        AssociatedObject.Items[0] : null;

            //    vm.LoadMoreMessagesCommand.Execute(null);
            //}
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_scrollViewer == null || _isScrollingProgrammatically) return;

            Dispatcher.BeginInvoke(() =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (e.NewStartingIndex == 0)
                    {
                        _isScrollingProgrammatically = true;

                        if (_lastItemBeforeLoad != null)
                        {
                            var container = AssociatedObject
                                .ItemContainerGenerator
                                .ContainerFromItem(_lastItemBeforeLoad) as FrameworkElement;

                            if (container != null)
                            {
                                var transform = container.TransformToVisual(_scrollViewer);
                                var position = transform.Transform(new Point(0, 0));
                                _scrollViewer.ScrollToVerticalOffset(position.Y);
                            }
                        }
                        else
                        {
                            _scrollViewer.ScrollToBottom();
                        }
                    }
                    else if (e.NewStartingIndex + e.NewItems.Count == AssociatedObject.Items.Count)
                    {
                        _isScrollingProgrammatically = true;
                        _scrollViewer.ScrollToBottom();
                    }
                }
            }, DispatcherPriority.Render);
        }

        private ScrollViewer FindScrollViewer(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is ScrollViewer viewer)
                    return viewer;

                var result = FindScrollViewer(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}