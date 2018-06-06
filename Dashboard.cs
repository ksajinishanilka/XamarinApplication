using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using static Android.Views.View;
using Android.Graphics;
using Android.Provider;
using Android.Runtime;
using Firebase.Storage;
using System;
using System.IO;
using Firebase.Xamarin.Database;

namespace XamarinApp
{
    [Activity(Label = "Dashboard", Theme = "@style/AppTheme")]
    public class Dashboard : AppCompatActivity, IOnClickListener,IOnSuccessListener, IOnFailureListener
    {
        Button btnLogout, btnProfile,btnSync;
        RelativeLayout activity_dashboard;
        FirebaseAuth auth;
        private const string FirebaseURL = "https://xamarinapp-67afd.firebaseio.com/";
        private const string ReachableHost = "www.google.com";
        private const string FirebaseUserImageChild = "userimages";
        private const string UploadDirectory = "Uploads";
        private Button btnUpload, btnChoose;
        private ImageView imgView;
        private Android.Net.Uri filePath;
        private const int PICK_IMAGE_REQUSET = 71;
        FirebaseStorage storage;
        StorageReference storageRef;
        Database db;
        OfflineHandler dataHandler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);
            //Init Firebase  
            auth = FirebaseAuth.GetInstance(MainActivity.app);
            storage = FirebaseStorage.Instance;
            storageRef = storage.GetReferenceFromUrl("gs://xamarinapp-67afd.appspot.com");

            //create database
            db = new Database();
            db.CreateDatabase();
            //View  
            btnLogout = FindViewById<Button>(Resource.Id.dashboard_btn_logout);
            btnProfile = FindViewById<Button>(Resource.Id.dashboard_user_profile);
            btnSync = FindViewById<Button>(Resource.Id.dashboard_btn_sync);
            //upload view init
            btnChoose = FindViewById<Button>(Resource.Id.btnChoose);
            btnUpload = FindViewById<Button>(Resource.Id.btnUpload);
            imgView = FindViewById<ImageView>(Resource.Id.imgView);

            //upload events
            btnChoose.Click += delegate
            {
                ChooseImage();
            };
            btnUpload.Click += delegate
            {
                UploadImage();
            };

            activity_dashboard = FindViewById<RelativeLayout>(Resource.Id.activity_dashboard);
            btnLogout.SetOnClickListener(this);
            btnProfile.SetOnClickListener(this);
            btnSync.SetOnClickListener(this);
        }
        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.dashboard_user_profile)
                StartActivity(new Android.Content.Intent(this, typeof(ProfileActivity)));
            else if (v.Id == Resource.Id.dashboard_btn_sync) {
                var reachability = new Reachability.Net.XamarinAndroid.Reachability();//check network
                if (reachability.IsHostReachable(ReachableHost))
                {
                    dataHandler = new OfflineHandler();
                    dataHandler.Sync();
                    Toast.MakeText(this, "Synced Successfully", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "Device is Offline", ToastLength.Short).Show();
                }
            }
            else if (v.Id == Resource.Id.dashboard_btn_logout)
                LogoutUser();
        }
        private void LogoutUser()
        {
            auth.SignOut();
            AppData.LoggedInUser = null;
            if (auth.CurrentUser == null)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
        }

        private async void UploadImage()
        {
            var docs = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            
            UserImage img = new UserImage();
            
            try
            {
                var destination = System.IO.Path.Combine(docs, UploadDirectory); //adding uploads folder in the path to store images
                Directory.CreateDirectory(destination); //create uploads folder in specified path
                var absolutePath = GetRealPathFromURI(Application.Context, filePath);//convert uri to a path
                var fileName = System.IO.Path.GetFileName(absolutePath); //get the filename from the path
                var uploadImage = System.IO.Path.Combine(destination, fileName); //adding the filename to the destination path
                File.Copy(absolutePath, uploadImage); //copying the file to the uploads folder

                img.Username = AppData.LoggedInUser;
                img.ImageRef = uploadImage;
                img.CreatedAt = DateTime.Now;
                db.InsertIntoUserImageTable(img); //insert data to userimage table
            }
            catch (Exception ex)
            {
                Console.WriteLine("Thrown exception is" + ex);
            }
            var reachability = new Reachability.Net.XamarinAndroid.Reachability();//check network
            if (reachability.IsHostReachable(ReachableHost))
            {
                String guid = Guid.NewGuid().ToString();// generate a unique id
               
                var imagesref = storageRef.Child("images/" + guid); //guid assigns a new unique identifier to the image before storing in firebase
                if (filePath != null)
                {
                    imagesref.PutFile(filePath)//add image to firebase storage
                    .AddOnSuccessListener(this)
                    .AddOnFailureListener(this);
                    var firebase = new FirebaseClient(FirebaseURL); //add image reference to firebase database 
                    img.ImageRef = imagesref.ToString();
                    var FirebaseReference = (await firebase.Child(FirebaseUserImageChild).PostAsync<UserImage>(img)).Key;
                    img.FirebaseReference = FirebaseReference;
                    db.UpdateUserImageTable(img);
                }
                else
                    Toast.MakeText(this, "First choose an image", ToastLength.Short).Show();

            }
            else
            {
                Toast.MakeText(this, "Uploaded Successfully", ToastLength.Short).Show();
            }

        }
        private void ChooseImage()
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(intent, "Select Picture"), PICK_IMAGE_REQUSET);

        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PICK_IMAGE_REQUSET &&
                resultCode == Result.Ok &&
                data != null &&
                data.Data != null)
            {
                filePath = data.Data;
                try
                {
                    Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, filePath);
                    imgView.SetImageBitmap(bitmap);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Out of Memory", ToastLength.Short).Show();
                }
            }
        }
        public void OnSuccess(Java.Lang.Object result)
        {
            Toast.MakeText(this, "Uploaded Successfully", ToastLength.Short).Show();
        }
        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, "" + e.Message, ToastLength.Short).Show();
        }

       
        public static string GetRealPathFromURI(Context context, Android.Net.Uri uri)
        {
            bool isKitKat = Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat;

            // DocumentProvider
            if (isKitKat && Android.Provider.DocumentsContract.IsDocumentUri(context, uri))
            {
                // ExternalStorageProvider
                if (isExternalStorageDocument(uri))
                {
                    string docId = Android.Provider.DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    string type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }

                    // TODO handle non-primary volumes
                }
                // DownloadsProvider
                else if (isDownloadsDocument(uri))
                {

                    string id = Android.Provider.DocumentsContract.GetDocumentId(uri);
                    Android.Net.Uri contentUri = ContentUris.WithAppendedId(Android.Net.Uri.Parse("content://downloads/public_downloads"), Convert.ToInt64(id));

                    return getDataColumn(context, contentUri, null, null);
                }
                // MediaProvider
                else if (isMediaDocument(uri))
                {
                    string docId = Android.Provider.DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    string type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Audio.Media.ExternalContentUri;
                    }

                    string selection = "_id=?";
                    string[] selectionArgs = new string[] {
                    split[1]
            };

                    return getDataColumn(context, contentUri, selection, selectionArgs);
                }
            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return getDataColumn(context, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        /**
         * Get the value of the data column for this Uri. This is useful for
         * MediaStore Uris, and other file-based ContentProviders.
         *
         * @param context The context.
         * @param uri The Uri to query.
         * @param selection (Optional) Filter used in the query.
         * @param selectionArgs (Optional) Selection arguments used in the query.
         * @return The value of the _data column, which is typically a file path.
         */
        public static String getDataColumn(Context context, Android.Net.Uri uri, String selection,
                String[] selectionArgs)
        {

            Android.Database.ICursor cursor = null;
            string column = "_data";
            string[] projection = {
                column
            };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs,
                        null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int column_index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(column_index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }


        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is ExternalStorageProvider.
         */
        public static bool isExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }

        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is DownloadsProvider.
         */
        public static bool isDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }

        /**
         * @param uri The Uri to check.
         * @return Whether the Uri authority is MediaProvider.
         */
        public static bool isMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }
    }
}