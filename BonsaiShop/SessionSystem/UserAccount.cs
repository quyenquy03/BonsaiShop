namespace BonsaiShop.SessionSystem
{
    [Serializable]
    public  class UserAccount
    {
        public  long UserID { get; set; }
        public  string UserName { get; set;}
        public  string FullName { get; set; }
        public  int? RoleID { get; set; }
    }
}
