using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using SQLite;
using System;
using System.IO;
using static Android.Graphics.Bitmap;
using static Android.Views.View;


namespace XamarinApp
{
    [Activity(Label = "SignUp", Theme = "@style/AppTheme")]
    public class SignUp : Activity, IOnClickListener, IOnCompleteListener
    {
        Button btnSignup;
        TextView btnLogin, btnForgetPass;
        EditText input_email, input_password, input_password_reenter;
        RelativeLayout activity_sign_up;
        FirebaseAuth auth;
        private const string FirebaseURL = "https://xamarinapp-67afd.firebaseio.com/";
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.signup_btn_login)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.signup_btn_forget_password)
            {
                StartActivity(new Intent(this, typeof(ForgetPassword)));
                Finish();
            }
            else
            if (v.Id == Resource.Id.signup_btn_register)
            {
                SignUpUser(input_email.Text, input_password.Text);
            }
        }
        private void SignUpUser(string email, string password)
        {

            if (ValidateSignUp(email, password))
            {
                //sqlite
                string dpPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3");
                var db = new SQLiteConnection(dpPath);
                User user = new User();
                user.Username = email;
                user.Password = password;

                db.Insert(user);
                Toast.MakeText(this, "Registration Successfull", ToastLength.Short).Show();

                auth.CreateUserWithEmailAndPassword(email, password);
            }

        }

        private bool ValidateSignUp(string email, string password)
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3"); //Call Database  
            var db = new SQLiteConnection(dbPath);

            db.CreateTable<User>();
            var data = db.Table<User>(); //Call Table  
            var userData = data.Where(x => x.Username == input_email.Text).FirstOrDefault(); //Linq Query  
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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here  
            SetContentView(Resource.Layout.SignUp);
            //Init Firebase  
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            //Views  
            btnSignup = FindViewById<Button>(Resource.Id.signup_btn_register);
            btnLogin = FindViewById<TextView>(Resource.Id.signup_btn_login);
            btnForgetPass = FindViewById<TextView>(Resource.Id.signup_btn_forget_password);
            input_email = FindViewById<EditText>(Resource.Id.signup_email);
            input_password = FindViewById<EditText>(Resource.Id.signup_password);
            input_password_reenter = FindViewById<EditText>(Resource.Id.signup_password_reeneter); 
            activity_sign_up = FindViewById<RelativeLayout>(Resource.Id.activity_sign_up);
            btnLogin.SetOnClickListener(this);
            btnSignup.SetOnClickListener(this);
            btnForgetPass.SetOnClickListener(this);


        }
        public void OnComplete(Task task)
        {
            if (task.IsSuccessful == true)
            {
                Toast.MakeText(this, "You Registered Successfully ", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Registration Failed, make sure the password contains minimum of 6 characters. ", ToastLength.Short).Show();
            }
        }
    }
}