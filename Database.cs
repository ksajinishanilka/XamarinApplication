using Android.Util;
using SQLite;
using System.Collections.Generic;
using System.Linq;
namespace XamarinApp

{
    public class Database
    {
        string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
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

        //Edit Operation  

        public bool UpdateUserTable(User user)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "user.db3")))
                {
                    connection.Query<User>("UPDATE Person set FirstName=?, LastName=?, City=?,PhoneNumber=? Where user.Username=auth.Currentuser.Email", user.FirstName, user.LastName, user.City, user.PhoneNumber);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
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

        public bool SelectUserTable(int Id)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, "user.db3")))
                {
                    connection.Query<User>("SELECT * FROM Person Where Id=?", Id);
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