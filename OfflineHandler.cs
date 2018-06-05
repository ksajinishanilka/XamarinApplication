using System;
using System.IO;
using Firebase.Auth;
using Firebase.Storage;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace XamarinApp
{
    class OfflineHandler
    {
        Database db=new Database();
        FirebaseAuth auth = FirebaseAuth.GetInstance(MainActivity.app);
        StorageReference storageRef;
        FirebaseStorage storage;
        private const string FirebaseURL = "https://xamarinapp-67afd.firebaseio.com/";
        public async void Sync()
        {
            db.CreateDatabase();

            var OfflineAddedUsers = db.GetOfflineAddedUsers();
            var firebase = new FirebaseClient(FirebaseURL);
            Console.WriteLine("Came Here");
            foreach (var user in OfflineAddedUsers)
            {
                var firebaseKey = (await firebase.Child("users").PostAsync<User>(user)).Key;
                user.FirebaseReference = firebaseKey.ToString();
                db.UpdateUserTable(user);
                auth.CreateUserWithEmailAndPassword(user.Username, user.Password);
            }

            var offlineImages = db.GetOfflineAddedUserImages();
            storage = FirebaseStorage.Instance;
            storageRef = storage.GetReferenceFromUrl("gs://xamarinapp-67afd.appspot.com");
            var direc = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var direcs = System.IO.Path.Combine(direc, "Uploads");
            var files = Directory.GetFiles(direcs);
            foreach (var file in files)
            {
                Console.WriteLine("File is " + file);
            }
            Console.WriteLine("Folder is : " + direcs);
            foreach (var image in offlineImages)
            {
                String guid = Guid.NewGuid().ToString();
                var imagesref = storageRef.Child("images/" + guid); //guid assigns a new unique identifier to the image before storing in firebase
                var filepath = Android.Net.Uri.Parse("file://" + image.ImageRef);
                var putfileResult = imagesref.PutFile(filepath);
                image.ImageRef = imagesref.ToString();
                var firebaseReference = (await firebase.Child("userimages").PostAsync<UserImage>(image)).Key;
                image.FirebaseReference = firebaseReference;
                db.UpdateUserImageTable(image);
            }

            var OfflineUpdatedUsers = db.GetOfflinUpdatedUsers();
            foreach (var user in OfflineUpdatedUsers)
            {
                user.FirebaseUpdated = 1;
                await firebase.Child("users").Child(user.FirebaseReference).PatchAsync(user);
                db.UpdateUserTable(user);
            }
        }
        
    }
}