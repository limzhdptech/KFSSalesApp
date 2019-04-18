using Acr.UserDialogs;
using QHSalesApp.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace QHSalesApp
{
    public class SelectMultipleBasePage<T> : ContentPage
    {
        public class WrappedSelection<T> : INotifyPropertyChanged
        {
            public T Item { get; set; }
            bool isSelected = false;
            public bool IsSelected
            {
                get { return isSelected; }
                set
                {
                    if(isSelected !=value)
                    {
                        isSelected = value;
                        PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged = delegate { };
        }

        public class WrappedItemSelectionTemplate: ViewCell
        {
            public WrappedItemSelectionTemplate() : base()
            {
                //Label source = new Label { TextColor = Color.Black, FontSize=16 };
                //source.SetBinding(Label.TextProperty, new Binding("Item.SourceType"));
                Label docNo = new Label { TextColor = Color.Black, FontSize = 16 };
                docNo.SetBinding(Label.TextProperty, new Binding("Item.DocumentNo"));
                Label docDate = new Label { TextColor = Color.Black, FontSize = 16 };
                docDate.SetBinding(Label.TextProperty, new Binding("Item.DocumentDate"));
                Label amount = new Label { TextColor = Color.Black, FontSize = 16};
                //amount.SetBinding(Label.TextProperty, new Binding("Item.Amount", stringFormat: "{0:0.00}"));
                amount.SetBinding(Label.TextProperty, new Binding("Item.Amount"));
                Switch mainSwitch = new Switch();
                mainSwitch.SetBinding(Switch.IsToggledProperty, new Binding("IsSelected"));
                RelativeLayout layout = new RelativeLayout();
                layout.Children.Add(docNo,
                    Constraint.Constant(10),
                    Constraint.Constant(15),
                    Constraint.RelativeToParent(p => p.Width - 60),
                    Constraint.RelativeToParent(p => p.Height - 10)
                );
                layout.Children.Add(docDate,
                    Constraint.Constant(150),
                    Constraint.Constant(15),
                    Constraint.RelativeToParent(p => p.Width - 60),
                    Constraint.RelativeToParent(p => p.Height - 10)
                );
                layout.Children.Add(amount,
                  Constraint.Constant(250),
                   Constraint.Constant(15),
                   Constraint.RelativeToParent(p => p.Width - 60),
                   Constraint.RelativeToParent(p => p.Height - 10)
               );
                layout.Children.Add(mainSwitch,
                    Constraint.RelativeToParent(p => p.Width - 55),
                    Constraint.Constant(5),
                    Constraint.Constant(50),
                    Constraint.RelativeToParent(p => p.Height - 10)
                );
                View = layout;
            }
        }

        public List<WrappedSelection<T>> WrappedItems = new List<WrappedSelection<T>>();

        public SelectMultipleBasePage(List<T> items)
        {
            string[] refnos = App.gRefDocNo.Split(',');
            bool isSelected = false;
            //if (refnos.Contains(c.DocNo)) isSelected = true;
            int count = -1;
         
            foreach (var item in items)
            {
                PaymentReference r= item as PaymentReference;
                count = Array.IndexOf(refnos, r.DocumentNo);
                if (count>-1)
                    isSelected = true;
                else
                    isSelected = false;
               // if (refnos.Contains(item.)) isSelected = true;
                WrappedItems.Add(new WrappedSelection<T>() { Item = item, IsSelected = isSelected });
            }
          
          //  WrappedItems = items.Select(item => new WrappedSelection<T>() { Item = item, IsSelected = true }).ToList();
            ListView mainList = new ListView()
            {
                ItemsSource = WrappedItems,
                ItemTemplate = new DataTemplate(typeof(WrappedItemSelectionTemplate)),
            };

            mainList.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null) return;
                var o = (WrappedSelection<T>)e.SelectedItem;
                o.IsSelected = !o.IsSelected;
                ((ListView)sender).SelectedItem = null; //de-select
            };
            //Grid grid = new Grid
            //{
            //    VerticalOptions=LayoutOptions.FillAndExpand,
            //    RowDefinitions =
            //    {
            //        new RowDefinition { Height = GridLength.Auto },
            //        new RowDefinition { Height = GridLength.Auto },
            //        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
            //        new RowDefinition { Height = new GridLength(100, GridUnitType.Absolute) }
            //    },
            //    ColumnDefinitions =
            //    {
            //       new ColumnDefinition { Width = GridLength.Auto },
            //        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            //        new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) }
            //    }
            //};

            //grid.Children.Add(new Label
            //{
            //    Text="Doc No",
            //    Font=Font.BoldSystemFontOfSize(30),
            //    HorizontalOptions=LayoutOptions.Center
            //},0,3,0,1);

            //grid.Children.Add(new Label
            //{
            //    Text = "Autosized cell",
            //    TextColor = Color.White,
            //    BackgroundColor = Color.Blue
            //}, 0, 1);

            //grid.Children.Add(new BoxView
            //{
            //    Color = Color.Silver,
            //    HeightRequest = 0
            //}, 1, 1);

            //grid.Children.Add(new BoxView
            //{
            //    Color = Color.Teal
            //}, 0, 2);

            //grid.Children.Add(new Label
            //{
            //    Text = "Leftover space",
            //    TextColor = Color.Purple,
            //    BackgroundColor = Color.Aqua,
            //    XAlign = TextAlignment.Center,
            //    YAlign = TextAlignment.Center,
            //}, 1, 2);

            //grid.Children.Add(new Label
            //{
            //    Text = "Span two rows (or more if you want)",
            //    TextColor = Color.Yellow,
            //    BackgroundColor = Color.Navy,
            //    XAlign = TextAlignment.Center,
            //    YAlign = TextAlignment.Center
            //}, 2, 3, 1, 3);

            //grid.Children.Add(mainList, 0, 2, 3, 4);

            //grid.Children.Add(new Label
            //{
            //    Text = "Fixed 100x100",
            //    TextColor = Color.Aqua,
            //    BackgroundColor = Color.Red,
            //    XAlign = TextAlignment.Center,
            //    YAlign = TextAlignment.Center
            //}, 2, 3);

            //// Accomodate iPhone status bar.
            //this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

            //// Build the page.
            //this.Content = grid;

            Content = mainList;
            if (Device.RuntimePlatform == Device.WinPhone)
            {   // fix issue where rows are badly sized (as tall as the screen) on WinPhone8.1
                mainList.RowHeight = 40;
                // also need icons for Windows app bar (other platforms can just use text)
                ToolbarItems.Add(new ToolbarItem("All", "check.png", SelectAll, ToolbarItemOrder.Primary));
                ToolbarItems.Add(new ToolbarItem("None", "cancel.png", SelectNone, ToolbarItemOrder.Primary));
            }
            else
            {
                ToolbarItems.Add(new ToolbarItem("All", null, SelectAll, ToolbarItemOrder.Primary));
                ToolbarItems.Add(new ToolbarItem("None", null, SelectNone, ToolbarItemOrder.Primary));
            }
        }

        void SelectAll()
        {
            foreach (var wi in WrappedItems)
            {
                wi.IsSelected = true;
            }
        }
        void SelectNone()
        {
            foreach (var wi in WrappedItems)
            {
                wi.IsSelected = false;
            }
        }
        public List<T> GetSelection()
        {
            return WrappedItems.Where(item => item.IsSelected).Select(wrappedItem => wrappedItem.Item).ToList();
        }
    }
}
