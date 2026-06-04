using SDKTemplate.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace SDKTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private ItemList? defaultViewModel;
        public string index = "index";

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.defaultViewModel = new ItemList();

            this.navigationHelper.LoadState += this.NavigationHelper_Load;
            this.navigationHelper.SaveState += this.NavigationHelper_Save;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// </summary>
        public ItemList? DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(SubPage), e.ClickedItem.ToString());
        }

        /// <summary>
        /// Adds an item to the list when the app bar button is clicked.
        /// </summary>
        private void AddItem()
        {
            this.defaultViewModel?.Add(new Item() { Name = "Item " + ((this.defaultViewModel?.Count ?? 0) + 1).ToString() });
        }

        /// <summary>
        /// Resets the item list to the default.
        /// </summary>
        private void ResetItems()
        {
            this.defaultViewModel = ItemList.CreateDefaultItemList();
            list.ItemsSource = this.defaultViewModel;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.
        /// </summary>
        private void NavigationHelper_Load(object sender, LoadStateEventArgs e)
        {
            this.defaultViewModel = null;
            if (SuspensionManager.SessionState.ContainsKey(index))
            {
                this.defaultViewModel = SuspensionManager.SessionState[index] as ItemList;
            }

            if (this.defaultViewModel == null)
            {
                this.defaultViewModel = ItemList.CreateDefaultItemList();
            }

            list.ItemsSource = this.defaultViewModel;
        }

        /// <summary>
        /// Preserves state associated with this page.
        /// </summary>
        private void NavigationHelper_Save(object sender, SaveStateEventArgs e)
        {
            SuspensionManager.SessionState[index] = defaultViewModel!;
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }

    /// <summary>
    /// Observable collection that will contain the list of items.
    /// </summary>
    public class ItemList : ObservableCollection<Item>
    {
        public ItemList()
        {
        }

        static public ItemList CreateDefaultItemList()
        {
            return new ItemList()
            {
                new Item { Name = "Item 1" },
                new Item { Name = "Item 2" },
                new Item { Name = "Item 3" },
                new Item { Name = "Item 4" },
            };
        }
    }

    /// <summary>
    /// Items that are in the observable collection (ItemList).
    /// </summary>
    public class Item
    {
        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            return this.Name;
        }
    }
}
