using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using static Android.Views.View;
using System.Text.RegularExpressions;
using Firebase.Xamarin.Database.Query;

namespace XamarinApp
{
    [Activity(Label = "ProfileActivity", Theme = "@style/AppTheme")]
    public class ProfileActivity : Activity, IOnClickListener
    {
        TextView txtWelcome;
        Button btnSavedata, btnDashboard;
        EditText input_first_name, input_last_name, input_city, input_phonenumber;
        RelativeLayout activity_profile;
        FirebaseAuth auth;
        private const string FirebaseURL = "https://xamarinapp-67afd.firebaseio.com/";
        private const string ReachableHost = "www.google.com";
        private const string FirebaseUserChild = "users";
        Database db;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Profile);
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            txtWelcome = FindViewById<TextView>(Resource.Id.profile_welcome);
            input_first_name = FindViewById<EditText>(Resource.Id.input_first_name);
            input_last_name = FindViewById<EditText>(Resource.Id.input_last_name);
            input_city = FindViewById<EditText>(Resource.Id.input_city);
            input_phonenumber = FindViewById<EditText>(Resource.Id.input_phonenumber);
            activity_profile = FindViewById<RelativeLayout>(Resource.Id.activity_profile);

            btnDashboard = FindViewById<Button>(Resource.Id.profile_btn_dashboard);
            btnSavedata = FindViewById<Button>(Resource.Id.profile_save_data);
            btnSavedata.SetOnClickListener(this);
            btnDashboard.SetOnClickListener(this);

            //create database
            db = new Database();
            db.CreateDatabase();
            var curUser = db.SelectSingleUser(AppData.LoggedInUser)[0];//retreive the logged in user object from the database

            
            input_first_name.Text = curUser.FirstName;
            input_last_name.Text = curUser.LastName;
            input_city.Text = curUser.City;
            input_phonenumber.Text = curUser.PhoneNumber;
            
            string variable = AppData.LoggedInUser;
            if (variable != null)
                txtWelcome.Text = variable;
        }
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.profile_save_data)
            {
                int n;
                if((string.IsNullOrEmpty(input_first_name.Text)) && (string.IsNullOrEmpty(input_last_name.Text)) && (string.IsNullOrEmpty(input_city.Text)) && (string.IsNullOrEmpty(input_phonenumber.Text))){
                    Toast.MakeText(this, "No data to save", ToastLength.Short).Show();
                }
                else if (!Regex.IsMatch(input_first_name.Text, @"^[a-zA-Z]+$"))
                {
                    //first name contains characters other than letters
                    Toast.MakeText(this, "First Name should only contain letters", ToastLength.Short).Show();
                }
                else if (!Regex.IsMatch(input_last_name.Text, @"^[a-zA-Z]+$"))
                {
                    //last name contains characters other than letters
                    Toast.MakeText(this, "Last Name should only contain letters", ToastLength.Short).Show();
                }
                else if (!Regex.IsMatch(input_city.Text, @"^[a-zA-Z]+$"))
                {
                    //city contains characters other than letters
                    Toast.MakeText(this, "Invalid city enetered", ToastLength.Short).Show();
                }
                else if ((input_phonenumber.Text.Length > 11) ||
                    (input_phonenumber.Text.Length < 10) ||
                    !int.TryParse(input_phonenumber.Text, out n))
                {
                    Toast.MakeText(this, "Invalid Phone Number Entered", ToastLength.Short).Show();
                }
                else
                {
                    SaveUserDetail(input_first_name.Text, input_last_name.Text, input_city.Text, input_phonenumber.Text);
                }
            }
            else if (v.Id == Resource.Id.profile_btn_dashboard)
                StartActivity(new Android.Content.Intent(this, typeof(Dashboard)));
        }
        private void SaveUserDetail(string firstname, string lastname, string city, string phonenumber)
        {
            User currentUser = db.SelectSingleUser(AppData.LoggedInUser)[0];
            currentUser.FirstName = firstname;
            currentUser.LastName = lastname;
            currentUser.City = city;
            currentUser.PhoneNumber = phonenumber;
            currentUser.UpdatedAt = DateTime.Now;
            int updatedRows = db.UpdateUserTable(currentUser);

            if (updatedRows == 1)
            {
                User updatedUser = db.SelectSingleUser(AppData.LoggedInUser)[0];
                var reachability = new Reachability.Net.XamarinAndroid.Reachability();//check network
                if (reachability.IsHostReachable(ReachableHost))
                {
                    updatedUser.FirebaseUpdated = 1;
                    var firebase = new FirebaseClient(FirebaseURL);
                    var fbResult = firebase.Child(FirebaseUserChild).Child(updatedUser.FirebaseReference).PatchAsync(updatedUser);
                } else
                {
                    updatedUser.FirebaseUpdated = 0;
                    
                }
                db.UpdateUserTable(updatedUser);// record that the firebase database was not updated
                Toast.MakeText(this, "Profile Updated.", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Profile Update Failed. ", ToastLength.Short).Show();
            }
        }
    }
}