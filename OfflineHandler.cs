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
        private const string FirebaseUserChild = "users";
        private const string FirebaseUserImageChild = "userimages";
        private const string firebaseStorageReference = "gs://xamarinapp-67afd.appspot.com";

        //add offline added data to firebase
        public async void Sync()
        {
            db.CreateDatabase();

            var OfflineAddedUsers = db.GetOfflineAddedUsers();
            var firebase = new FirebaseClient(FirebaseURL);
            // add offline added users to firebase database and authenticatio
            foreach (var user in OfflineAddedUsers)
            {
                var firebaseKey = (await firebase.Child(FirebaseUserChild).PostAsync<User>(user)).Key;
                user.FirebaseReference = firebaseKey.ToString();
                db.UpdateUserTable(user);
                auth.CreateUserWithEmailAndPassword(user.Username, user.Password);
            }
            //add offline added images to firebase database and storage
            var offlineImages = db.GetOfflineAddedUserImages();
            storage = FirebaseStorage.Instance;
            storageRef = storage.GetReferenceFromUrl(firebaseStorageReference);
            
            foreach (var image in offlineImages)
            {
                String guid = Guid.NewGuid().ToString();
                var imagesref = storageRef.Child("images/" + guid);
                var filepath = Android.Net.Uri.Parse("file://" + image.ImageRef);
                var putfileResult = imagesref.PutFile(filepath);
                image.ImageRef = imagesref.ToString();
                var firebaseReference = (await firebase.Child(FirebaseUserImageChild).PostAsync<UserImage>(image)).Key;
                image.FirebaseReference = firebaseReference;
                db.UpdateUserImageTable(image);
            }
            //add offline updated user details in firebase database
            var OfflineUpdatedUsers = db.GetOfflinUpdatedUsers();
            foreach (var user in OfflineUpdatedUsers)
            {
                user.FirebaseUpdated = 1;
                await firebase.Child(FirebaseUserChild).Child(user.FirebaseReference).PatchAsync(user);
                db.UpdateUserTable(user);
            }
        }
        
    }
}