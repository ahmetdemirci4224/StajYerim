using System;
using BCrypt.Net;

public class Program
{
    public static void Main()
    {
        string hash = "$2a$11$EsEYsIvq0uNx1X/TTUoSN.HRcf2HveW9u8qs42/ra4Yq7OI.6KqFu";
        string password = "yourpassword";
        bool isMatch = BCrypt.Net.BCrypt.Verify(password, hash);
        Console.WriteLine("Match: " + isMatch);
    }
}
