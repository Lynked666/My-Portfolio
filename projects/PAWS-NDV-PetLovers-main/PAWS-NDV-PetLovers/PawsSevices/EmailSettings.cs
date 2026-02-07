namespace PAWS_NDV_PetLovers.PawsSevices
{
    //this is use for your appsettings
    public class EmailSettings
    {
        public string SMTPServer { get; set; } = string.Empty;
        public int SMTPPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
    }
}
