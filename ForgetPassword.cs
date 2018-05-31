using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using static Android.Views.View;
namespace XamarinApp
{
    [Activity(Label = "ForgetPasswordcs", Theme = "@style/AppTheme")]
    public class ForgetPassword : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        Button btnResetPas;
        TextView btnBack;
        RelativeLayout activity_forget;
        FirebaseAuth auth;
        EditText input_new_password;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ForgetPassword);
            //Init Firebase  
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            //Views 
            btnResetPas = FindViewById<Button>(Resource.Id.forget_btn_reset);
            btnBack = FindViewById<TextView>(Resource.Id.forget_btn_back);
            input_new_password = FindViewById<EditText>(Resource.Id.dashboard_newpassword);
            activity_forget = FindViewById<RelativeLayout>(Resource.Id.activity_forget);
            btnResetPas.SetOnClickListener(this);
            btnBack.SetOnClickListener(this);
        }
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.forget_btn_back)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
            else if (v.Id == Resource.Id.forget_btn_reset)
            {
                ChangePassword(input_new_password.Text);

            }
        }
        private void ChangePassword(string newPassword)
        {

            FirebaseUser user = auth.CurrentUser;
            user.UpdatePassword(newPassword)
            .AddOnCompleteListener(this);
        }
        public void OnComplete(Task task)
        {
            if (!task.IsSuccessful)
            {
                Toast.MakeText(this, "Reset Password Failed! Make sure that the password contains minimum of 6 characters.", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Password changed successfully.Login using the new password", ToastLength.Short).Show();
                StartActivity(new Intent(this, typeof(MainActivity)));
            }
        }
    }
}