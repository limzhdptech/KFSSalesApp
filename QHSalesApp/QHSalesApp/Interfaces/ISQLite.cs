using System;
using SQLite;

namespace QHSalesApp
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection(string database);
        long GetSize(string database);
       // SQLiteConnection GetConnection();
    }
}
