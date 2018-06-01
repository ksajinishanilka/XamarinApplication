using SQLite;

namespace XamarinApp
{
    public class UserImage
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]

        public int Id { get; set; } // AutoIncrement and set primarykey  

        public string Username { get; set; }

        public string ImageRef { get; set; }
    }
}