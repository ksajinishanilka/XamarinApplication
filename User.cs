using SQLite;
using System;

namespace XamarinApp
{
    public class User
    {
        [PrimaryKey, Column("_Username")]
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string FirebaseReference { get; set; }
        public int FirebaseUpdated { get; set; } = 1;

    }
}