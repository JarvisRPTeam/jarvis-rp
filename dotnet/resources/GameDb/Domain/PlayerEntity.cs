namespace GameDb.Domain
{
    public class PlayerEntity
    {
        public ulong Id { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public long Cash { get; set; }
    }
}