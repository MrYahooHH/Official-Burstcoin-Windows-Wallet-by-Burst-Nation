
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using Windows.Storage;
using System.IO;

namespace BNWallet_Windows
{
    public class RuntimeVar
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string CurrentWalletName { get; set; }
        public string CurrentPassphrase { get; set; }
        

        public RuntimeVar()
        {
            id = 1;
            CurrentWalletName = "";
            CurrentPassphrase = "";
        }
    }

    public class RuntimeVarDB
    {
        private SQLiteConnection db;

        public RuntimeVarDB()
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

            }
        }

        public RuntimeVar Get()
        {
            var data = db.Table<RuntimeVar>();
            var RT = data.Where(s => s.id == 1).FirstOrDefault();
            return RT;
        }

        public void Save(RuntimeVar RT)
        {
            db.InsertOrReplace(RT);
        }

    }
}