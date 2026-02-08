/*
 * NUI CollectionView sample: vertical LinearLayouter.
 */

using System;
using System.Collections.ObjectModel;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.NUI.Binding;

namespace NUICoreTest
{
    public class ListItem
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ViewLabel => Title;
    }

    public class App : NUIApplication
    {
        private Window _window;
        private CollectionView _collectionView;
        private ObservableCollection<ListItem> _items;
        private int _itemCounter;

        protected override void OnCreate()
        {
            base.OnCreate();
            _window = GetDefaultWindow();
            _window.Title = "NUI Core Test";
            _window.BackgroundColor = new Color(0.95f, 0.95f, 0.95f, 1.0f);

            _items = new ObservableCollection<ListItem>();
            _items.Add(new ListItem { Title = $"Main Loop Test" });
            _items.Add(new ListItem { Title = $"Timer Test" });
            _items.Add(new ListItem { Title = $"Screen Test" });

            for (int i = 3; i < 20; i++)
            {
                _itemCounter++;
                _items.Add(new ListItem { Title = $"Item {_itemCounter}" });
            }

            var root = new View
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    CellPadding = new Size2D(0, 8),
                },
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Padding = new Extents(32, 32, 32, 32),
            };

            var title = new TextLabel
            {
                Text = "Tizen Core Manual Test",
                PointSize = 32,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
            };
            root.Add(title);

            // CollectionView with vertical LinearLayouter
            var headerItem = new DefaultTitleItem
            {
                Text = $"Items: {_items.Count}",
                WidthSpecification = LayoutParamPolicies.MatchParent,
            };

            _collectionView = new CollectionView
            {
                ItemsSource = _items,
                ItemsLayouter = new LinearLayouter(),
                ItemTemplate = new DataTemplate(() =>
                {
                    var item = new DefaultLinearItem
                    {
                        WidthSpecification = LayoutParamPolicies.MatchParent,
                    };
                    item.Label.SetBinding(TextLabel.TextProperty, "ViewLabel");
                    item.Label.HorizontalAlignment = HorizontalAlignment.Begin;
                    if (item.SubLabel != null)
                    {
                        item.SubLabel.SetBinding(TextLabel.TextProperty, "Subtitle");
                        item.SubLabel.HorizontalAlignment = HorizontalAlignment.Begin;
                    }
                    return item;
                }),
                Header = headerItem,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                SelectionMode = ItemSelectionMode.Single,
            };
            _collectionView.SelectionChanged += OnSelectionChanged;

            root.Add(_collectionView);
            _window.Add(root);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (object item in e.CurrentSelection)
            {
                if (item is ListItem listItem)
                {
                    Tizen.Log.Info("NUICoreTest", $"Selected: {listItem.Title}");
                    if (listItem.Title == "Main Loop Test")
                    {
                        var subpage = MainLoopTest.Create(_window);
                        _window.Add(subpage);
                    }
                    else if (listItem.Title == "Timer Test")
                    {
                        var subpage = TimerTest.Create(_window);
                        _window.Add(subpage);
                    }
                    else if (listItem.Title == "Screen Test")
                    {
                        var subpage = ScreenTest.Create(_window);
                        _window.Add(subpage);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            var app = new App();
            app.Run(args);
        }
    }
}
