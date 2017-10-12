
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using Windows.Storage;
using System.IO;


namespace BNWallet_Windows
{
    public class  UserAccounts
    {
        [PrimaryKey, NotNull]
        public string AccountName { get; set; }
        public string BurstAddress { get; set; }
        public string PassPhrase { get; set; }
        

        public UserAccounts()
        {
            AccountName = "";
            BurstAddress = "";
            PassPhrase = "";
           
        }
    }

    public class UserAccountsDB
    {
        private SQLiteConnection db;

        public UserAccountsDB()
        {
            var dbName = //*Hidden*;
            string dbFileName = //*Hidden*;



            if (System.IO.File.Exists(dbFileName))
            {
                db = new SQLiteConnection(dbFileName);
            }
            else
            {
                //If the file does not exist then create the initial table.
                db = new SQLiteConnection(dbFileName);
                
            }
            //db.CreateTable<UserAccounts>();
           // db.CreateTable<RuntimeVar>();
           // db.CreateTable<UserAccountRuntime>();

        }

        public UserAccounts Get(string AccountName)
        {
            var data = db.Table<UserAccounts>();
            var accountname = data.Where(s => s.AccountName == AccountName).FirstOrDefault();
            return accountname;
        }

        public void Save(UserAccounts accountname)
        {
            db.InsertOrReplace(accountname);
        }
        

        public void ClearDB()
        {
            db.DeleteAll<UserAccounts>();
        }

        public void Save(UserAccounts[] userAccountsArr)
        {
            for (int i = 0; i < userAccountsArr.Length; i++)
            {
                db.InsertOrReplace(userAccountsArr[i]);
            }

        }

        public UserAccounts[] GetAccountList()
        {
            var data = db.Table<UserAccounts>();
            var AccountList = data.OrderBy(s=>s.AccountName);

            UserAccounts[] ua = new UserAccounts[AccountList.Count()];
            for (int i = 0; i < AccountList.Count(); i++)
                ua[i] = AccountList.ElementAt(i);

            return ua;
        }

        public void RemoveWallet(UserAccounts accountname)
        {
            db.Delete(accountname);
        }

    }
}