using SQLite;

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
    }
}