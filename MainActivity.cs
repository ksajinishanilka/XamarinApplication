using Android.App;
using Android.Widget;
using Android.OS;
using Firebase;
using Firebase.Auth;
using System;
using static Android.Views.View;
using Android.Views;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace XamarinApp
{
    [Activity(Label = "XamarinApp  ", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : Activity, IOnClickListener
    {
        Button btnLogin;
        EditText input_email, input_password;
        TextView btnSignUp;
        RelativeLayout activity_main;
        public static FirebaseApp app;
        FirebaseAuth auth;
        private const string FirebaseURL = "https://xamarinapp-67afd.firebaseio.com/";
        Database db;
        OfflineHandler dataHandler;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource  
            SetContentView(Resource.Layout.Main);
            //Init Auth  
            InitFirebaseAuth();

            //create database
            db = new Database();
            db.CreateDatabase();

            dataHandler = new OfflineHandler();

            //Views  
            btnLogin = FindViewById<Button>(Resource.Id.login_btn_login);
            input_email = FindViewById<EditText>(Resource.Id.login_email);
            input_password = FindViewById<EditText>(Resource.Id.login_password);
            btnSignUp = FindViewById<TextView>(Resource.Id.login_btn_sign_up);
            activity_main = FindViewById<RelativeLayout>(Resource.Id.activity_main);
            btnSignUp.SetOnClickListener(this);
            btnLogin.SetOnClickListener(this);
        }
        private void InitFirebaseAuth()
        {
            var options = new FirebaseOptions.Builder()
               .SetApplicationId("1:954847151497:android:523a5296bcd8083b")
               .SetApiKey("AIzaSyAwYiQQo4aH_aL7cISxhjNFdND6x4pAIuI")
               .Build();
            if (app == null)
                app = FirebaseApp.InitializeApp(this, options);
            auth = FirebaseAuth.GetInstance(app);
        }
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.login_btn_sign_up)
            {
                StartActivity(new Android.Content.Intent(this, typeof(SignUp)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.login_btn_login)
            {
                if (string.IsNullOrEmpty(input_email.Text) || string.IsNullOrEmpty(input_password.Text))
                {
                    Toast.MakeText(this, "Email and Password cannot be empty", ToastLength.Short).Show();
                }
                else
                {
                    LoginUser(input_email.Text, input_password.Text);
                }
            }
        }

        private void LoginUser(string email, string password)
        {
            Console.WriteLine("password is " + password);
            password = AppData.EncryptPassword(password);
            Console.WriteLine("encrypted password is " + password);
            AppData.LoggedInUser = email;
            var data = db.SelectUserTable(); //retrieve all users in the user table
            var userData = data.Where(x => x.Username == email && x.Password == password).FirstOrDefault(); //getting the matching user 
            var reachability = new Reachability.Net.XamarinAndroid.Reachability();//check network
            if (reachability.IsHostReachable("www.google.com"))
            {
                auth.SignInWithEmailAndPassword(email, password); //firebase auth signin
            }
            if (userData != null)
            {
                Toast.MakeText(this, "Login Success", ToastLength.Short).Show();//go to dashboard on succesful user login 
                StartActivity(new Android.Content.Intent(this, typeof(Dashboard)));
            }else
            {
                Toast.MakeText(this, "Login Failed", ToastLength.Short).Show();
            }
        }
        
    }
}