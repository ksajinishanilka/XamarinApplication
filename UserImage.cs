using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace XamarinApp
{
    public class UserImage
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]

        public int Id { get; set; } // AutoIncrement and set primarykey  

        public string Username { get; set; }

        public string ImageRef { get; set; }
    }
}