using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using System;
using System.Linq;
using static Android.Views.View;


namespace XamarinApp
{
    [Activity(Label = "SignUp", Theme = "@style/AppTheme")]
    public class SignUp : Activity, IOnClickListener
    {
        Button btnSignup;
        TextView btnLogin;
        EditText input_email, input_password, input_password_reenter;
        RelativeLayout activity_sign_up;
        FirebaseAuth auth;
        Database db;
        OfflineHandler dataHandler;
        private const string FirebaseURL = "https://xamarinapp-67afd.firebaseio.com/";
        private const string ReachableHost = "www.google.com";
        private const string FirebaseUserChild = "users";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignUp);
            //Init Firebase  
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            //create local database
            db = new Database();
            db.CreateDatabase();
            dataHandler = new OfflineHandler();
            //Views  
            btnSignup = FindViewById<Button>(Resource.Id.signup_btn_register);
            btnLogin = FindViewById<TextView>(Resource.Id.signup_btn_login);
            input_email = FindViewById<EditText>(Resource.Id.signup_email);
            input_password = FindViewById<EditText>(Resource.Id.signup_password);
            input_password_reenter = FindViewById<EditText>(Resource.Id.signup_password_reeneter);
            activity_sign_up = FindViewById<RelativeLayout>(Resource.Id.activity_sign_up);
            btnLogin.SetOnClickListener(this);
            btnSignup.SetOnClickListener(this);
        }
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.signup_btn_login)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.signup_btn_register)
            {
                SignUpUser(input_email.Text, input_password.Text);
            }
        }
        //user registration
        private async void SignUpUser(string email, string password)
        {

            if (ValidateSignUp(email, password))
            {
                password = AppData.EncryptPassword(password);
                User user = new User()
                {
                    Username = email,
                    Password = password,
                    CreatedAt = DateTime.Now
                };
              
                
                db.InsertIntoUserTable(user);
                User addedUser = db.SelectSingleUser(email)[0];

                var reachability = new Reachability.Net.XamarinAndroid.Reachability();//check network
                if (reachability.IsHostReachable(ReachableHost))
                {
                    var firebase = new FirebaseClient(FirebaseURL);
                    var firebaseKey = (await firebase.Child(FirebaseUserChild).PostAsync<User>(addedUser)).Key;
                    user.FirebaseReference = firebaseKey.ToString();
                    db.UpdateUserTable(user);
                    auth.CreateUserWithEmailAndPassword(email, password);
                }
                Toast.MakeText(this, "You Registered Successfully ", ToastLength.Short).Show();
                StartActivity(new Android.Content.Intent(this, typeof(MainActivity)));
            }

        }
        //validation for signup
        private bool ValidateSignUp(string email, string password)
        {
            var data = db.SelectUserTable();// retrieve all users in the user table
            var userData = data.Where(x => x.Username == email).FirstOrDefault();  
            if (userData != null)
            {
                Toast.MakeText(this, "Signup Failed. Email Already Exists", ToastLength.Short).Show();
                return false;
            }
            if (!Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
            {
                Toast.MakeText(this, "Signup Failed. Enter a Valid Email", ToastLength.Short).Show();
                return false;
            }
            if (password.Length < 6)
            {
                Toast.MakeText(this, "Signup Failed. Enter a password with atleast 6 characters", ToastLength.Short).Show();
                return false;
            }
            if (input_password.Text != input_password_reenter.Text)
            {
                Toast.MakeText(this, "passwords you entered don't match", ToastLength.Short).Show();
                return false;
            }
            return true;
        }

    }
}