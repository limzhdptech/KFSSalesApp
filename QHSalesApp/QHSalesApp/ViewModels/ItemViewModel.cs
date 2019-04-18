using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace QHSalesApp
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        
        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }



        private SQLiteConnection database;

        private static object collisionLock = new object();

        public ObservableCollection<Item> Items { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string name = "") =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public async Task PopulateDataAsync(bool refresh)
        {
            
            if (IsBusy)
                return;

            //if (refresh == true && App.IsConnected)
            //{
            //    try
            //    {
            //        IsBusy = true;
            //        DataManager manager = new DataManager();
            //        this.Items = new ObservableCollection<Item>(await QueryRssAsync());
            //        // Drop and recreate
            //        database.DropTable<Item>();
            //        database.CreateTable<Item>();
            //        SaveAllItems();
            //        return;
            //    }
            //    catch
            //    {
            //        return;
            //    }
            //    finally
            //    {
            //        IsBusy = false;
            //    }
            //}
           
            // If already any items in the table, no need of loading from Internet
            if (database.Table<Item>().Any())
            {
                this.Items =
                  new ObservableCollection<Item>(OfflineQuery());
                return;
            }
            //else
            //{
            //    // If not connected, raise an error
            //    if (!App.IsConnected) throw new InvalidOperationException();
            //    this.Items = new ObservableCollection<Item>(await QueryRssAsync());
            //    SaveAllItems();
            //    return;
            //}
        }

        public ItemViewModel()
        {
            

            // Invoke the platform-specific implementation of the interface via dep injection
            database = DependencyService.Get<ISQLite>().GetConnection(Constants.DatabaseName);

            // Create a table if not exists
            database.CreateTable<Item>();
        }

        // Query the table in the database
        private IEnumerable<Item> OfflineQuery()
        {
            lock (collisionLock)
            {
                return database.Table<Item>().AsEnumerable();
            }
        }

        public void SaveAllItems()
        {
            lock (collisionLock)
            {
                // In the database, save or update each item in the collection
                foreach (var feedItem in this.Items)
                {
                    if (feedItem.ID != 0)
                    {
                        database.Update(feedItem);
                    }
                    else
                    {
                        database.Insert(feedItem);
                    }
                }
            }
        }
    }
}
