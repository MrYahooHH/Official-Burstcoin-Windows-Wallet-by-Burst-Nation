
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using Windows.Storage;
using System.IO;

namespace BNWallet_Windows
{
    public class UserAccountRuntime
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }


        public UserAccountRuntime()
        {
            id = 1;
            Username = "";
            Password = "";
        }
    }

    public class UserAccountRuntimeDB
    {
        private SQLiteConnection db;

        public UserAccountRuntimeDB()
        {

            var dbName = //*Hidden*;
            string dbFileName =//*Hidden*;

            if (System.IO.File.Exists(dbFileName))
            {
                db = new SQLiteConnection(dbFileName);
               
            }
            else
            {
                //If the file does not exist then create the initial table.
                db = new SQLiteConnection(dbFileName);
                db.CreateTable<UserAccounts>();
                db.CreateTable<RuntimeVar>();
                db.CreateTable<UserAccountRuntime>();

            }
            


        }

        public UserAccountRuntime Get()
        {
            var data = db.Table<UserAccountRuntime>();
            var RT = data.Where(s => s.id == 1).FirstOrDefault();
            return RT;
        }

        public void Save(UserAccountRuntime RT)
        {
            db.InsertOrReplace(RT);
        }

    }
}
   
