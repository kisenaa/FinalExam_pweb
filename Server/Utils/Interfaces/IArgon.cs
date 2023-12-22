namespace Server.Utils.Interfaces;

public interface IArgon
{
    Memory<byte> EncryptPassword(string password);
    bool VerifyPassword(string password, ReadOnlySpan<byte> passwordHash);
}