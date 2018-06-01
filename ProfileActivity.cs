using System;
using Android.Gms.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using static Android.Views.View;
using System.Text.RegularExpressions;
using System.IO;
using SQLite;

namespace XamarinApp
{
    [Activity(Label = "ProfileActivity", Theme = "@style/AppTheme")]
    public class ProfileActivity : Activity, IOnClickListener, IOnCompleteListener
    {
        TextView txtWelcome;
        Button btnSavedata, btnLogout;
        EditText input_first_name, input_last_name, input_city, input_phonenumber;
        RelativeLayout activity_profile;
        FirebaseAuth auth;
        private const string FirebaseURL = "https://xamarinapp-67afd.firebaseio.com/";
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

            btnLogout = FindViewById<Button>(Resource.Id.profile_btn_logout);
            btnSavedata = FindViewById<Button>(Resource.Id.profile_save_data);
            btnSavedata.SetOnClickListener(this);
            btnLogout.SetOnClickListener(this);

            //create database
            db = new Database();
            db.CreateDatabase();
            var curUsers = db.SelectSingleUserTable();

            foreach (var curUser in curUsers)
            {
                input_first_name.Text = curUser.FirstName;
                input_last_name.Text = curUser.LastName;
                input_city.Text = curUser.City;
                input_phonenumber.Text = curUser.PhoneNumber;
            }

            if (auth != null)
                txtWelcome.Text = auth.CurrentUser.Email;
        }
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.profile_save_data)
            {
                int n;
                if (!Regex.IsMatch(input_first_name.Text, @"^[a-zA-Z]+$"))
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
            else if (v.Id == Resource.Id.profile_btn_logout)
                LogoutUser();
        }
        private void LogoutUser()
        {
            auth.SignOut();
            if (auth.CurrentUser == null)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
        }
        private void SaveUserDetail(string firstname, string lastname, string city, string phonenumber)
        {
            User user = new User();
            user.FirstName = firstname;
            user.LastName = lastname;
            user.City = city;
            user.PhoneNumber = phonenumber;

            int updatedRows = db.UpdateUserTable(
                new User
                {
                    Username = auth.CurrentUser.Email,
                    FirstName = firstname,
                    LastName = lastname,
                    City = city,
                    PhoneNumber = phonenumber
                }
            );

            if (updatedRows == 1)
            {
                var firebase = new FirebaseClient(FirebaseURL);
                firebase.Child("users").PostAsync<User>(user);
                Toast.MakeText(this, "Profile Updated..", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Profile Update Failed.. ", ToastLength.Short).Show();
            }
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful == true)
            {
                Toast.MakeText(this, "Profile Updated.", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Profile Update Failed. ", ToastLength.Short).Show();
            }
        }
    }
}