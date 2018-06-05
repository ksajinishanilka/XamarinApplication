using Android.Util;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
namespace XamarinApp

{
    public class Database
    {
        string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private const string DatabaseName = "user.db3";
        public bool CreateDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, DatabaseName)))
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

        //Insert Operation for user table
        public bool InsertIntoUserTable(User user)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, DatabaseName),false))
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
        //Select Operation for User Table
        public List<User> SelectUserTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, DatabaseName),false))
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
        //Select a single user by username
        public List<User> SelectSingleUser(string Username)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, DatabaseName), false))
                {
                    return connection.Query<User>("SELECT * FROM User WHERE _Username=? COLLATE NOCASE", Username);
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }
        //Update Operation by giving a user object
        public int UpdateUserTable(User user)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, DatabaseName), false))
                {
                    user.UpdatedAt = DateTime.Now;
                    connection.Update(user);
                    return 1;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return 0;
            }
        }
        //Update Operation by giving a userImage object
        public int UpdateUserImageTable(UserImage userImage)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, DatabaseName), false))
                {
                    connection.Update(userImage);
                    return 1;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return 0;
            }
        }
        //Select offline added users
        public List<User> GetOfflineAddedUsers()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, DatabaseName), false))
                {
                    return connection.Query<User>("SELECT * FROM User where FirebaseReference is null or FirebaseReference = ''");
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }
        //Select offline updated users
        public List<User> GetOfflinUpdatedUsers()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, DatabaseName), false))
                {
                    return connection.Query<User>("SELECT * FROM User where FirebaseUpdated =  0");
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }
        //UserImage table operations
        public bool InsertIntoUserImageTable(UserImage userImage)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, DatabaseName), false))
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
        //Select offline added userimages
        public List<UserImage> GetOfflineAddedUserImages()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(folder, DatabaseName), false))
                {
                    return connection.Query<UserImage>("SELECT * FROM UserImage where FirebaseReference is null or FirebaseReference = ''");
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