namespace PJATK_APBD_PROJ_s33611.Services.Auth;

public interface IPasswordService
{
    public string HashPassword(string password);
    public bool VerifyHashedPassword(string hashedPassword, string password);
}