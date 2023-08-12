namespace Nebular.Core.Network.Messages
{
    public class LoginMessage : Message
    {
        public LoginMessage() { }
        public LoginMessage(string email, string password)
        {
            Password = password;
            Email = email;
        }
        public override int MessageId => Id;

        public static int Id => 0;

        public string Email { get; set; }
        public string Password { get; set; }
        public string TokenBot { get; set; } = string.Empty;

    }
}
