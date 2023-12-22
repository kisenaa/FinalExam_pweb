using System.Diagnostics;
using System.Text;
using Server.Utils.Interfaces;
using Argon2id = Geralt.Argon2id;
using Encodings = Geralt.Encodings;
using SecureRandom = Geralt.SecureRandom;

namespace Server.Utils;

public class Argon : IArgon
{
    private int Iterations { get; set; }
    private int MemoryCost { get; set; }
    private const Encodings.Base64Variant Variant = Encodings.Base64Variant.OriginalNoPadding;

    public Argon(int iterations, int memoryCost)
    {
        Iterations = iterations;
        MemoryCost = memoryCost;
    }

    public Memory<byte> EncryptPassword(string password)
    {
        // Generate 16 byte of salt
        Span<byte> s = stackalloc byte[16];
        SecureRandom.Fill(s);
        // Generate password hash
        Span<byte> hash = stackalloc byte[32];
        Argon2id.DeriveKey(hash, Encoding.UTF8.GetBytes(password), s, Iterations, MemoryCost);
        // Construct the encoded argon output in base64 format
        EncodeHash(s, hash, out var output);
        Debug.WriteLine(output);
        return Encoding.UTF8.GetBytes(output);
    }
    
    private static void EncodeHash(ReadOnlySpan<byte> s, ReadOnlySpan<byte> hash, out string output)
    {
        output = $"$argon2id$v=19$m=8,t=2,p=1${Encodings.ToBase64(s, Variant)}${Encodings.ToBase64(hash, Variant)}";
    }

    public bool VerifyPassword(string password, ReadOnlySpan<byte> passwordHash)
    {
        return Argon2id.VerifyHash(passwordHash, Encoding.UTF8.GetBytes(password));
    }
}