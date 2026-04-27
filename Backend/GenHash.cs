using System;
using BCrypt.Net;

public class Program
{
    public static void Main()
    {
        string password = "yourpassword";
        string salt = BCrypt.Net.BCrypt.GenerateSalt(11);
        string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
        Console.WriteLine(hash);
    }
}
