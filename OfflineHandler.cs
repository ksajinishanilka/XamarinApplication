using System;
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
            foreach (var image in offlineImages)
            {
                String guid = Guid.NewGuid().ToString();
                var imagesref = storageRef.Child("images/" + guid); //guid assigns a new unique identifier to the image before storing in firebase
                var filepath = Android.Net.Uri.Parse(image.ImageRef);
                var putfileResult = imagesref.PutFile(filepath);
                //    var firebase = new FirebaseClient(FirebaseURL);
                //    image.ImageRef = imagesref.ToString();
                //    await firebase.Child("userimages").PostAsync<UserImage>(image);
                Console.WriteLine("Image ref is " + image.ImageRef);
                Console.WriteLine("Image guid is " + guid);
                //    Console.WriteLine("Putfile Result  is " + putfileResult.Result);
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