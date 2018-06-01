using Android.Util;
using Firebase.Auth;
using SQLite;
using System.Collections.Generic;
using System.Linq;
namespace XamarinApp

{
    public class Database
    {
        string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private FirebaseAuth auth= FirebaseAuth.GetInstance(MainActivity.app);
        public bool CreateDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "user.db3")))
                {
                    connection.CreateTable<User>();
                    connection.CreateTable<UserImage>();
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        //Add or Insert Operation for user table  

        public bool InsertIntoUserTable(User user)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "user.db3")))
                {
                    connection.Insert(user);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        public List<User> SelectUserTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "user.db3")))
                {
                    return connection.Table<User>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }
        //select single user
        public List<User> SelectSingleUserTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "user.db3")))
                {
                    return connection.Query<User>("SELECT * FROM User WHERE _Username=? COLLATE NOCASE", auth.CurrentUser.Email);
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        //Edit Operation  

        public int UpdateUserTable(User user)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "user.db3")))
                {
                    connection.Query<User>("UPDATE User set FirstName=?, LastName=?, City=?,PhoneNumber=? Where _Username=?" , user.FirstName, user.LastName, user.City, user.PhoneNumber, auth.CurrentUser.Email);
                    return 1;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return 0;
            }
        }
        //Delete Data Operation  

        public bool RemoveUserTable(User user)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "user.db3")))
                {
                    connection.Delete(user);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        //Select Operation  

        public bool SelectUserTable(string Username)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "user.db3")))
                {
                    connection.Query<User>("SELECT * FROM User WHERE Username=? COLLATE NOCASE",auth.CurrentUser.Email);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        //UserImage table operations
        public bool InsertIntoUserImageTable(UserImage userImage)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "user.db3")))
                {
                    connection.Insert(userImage);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
        public List<UserImage> SelectUserIamgeTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "user.db3")))
                {
                    return connection.Table<UserImage>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }
    }
}