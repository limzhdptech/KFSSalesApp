using SQLite;

namespace QHSalesApp
{
    public class BaseItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
    }
}
