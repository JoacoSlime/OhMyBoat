namespace OhMyBoat.UI.Server.Helpers;

public class EmailSettings
{

    // refresh token time to live (in days), inactive tokens are
    // automatically deleted from the database after this time
    public int RefreshTokenTTL { get; set; }

    public required string EmailFrom { get; set; }
    public required string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public required string SmtpUser { get; set; }
    public required string SmtpPass { get; set; }
}