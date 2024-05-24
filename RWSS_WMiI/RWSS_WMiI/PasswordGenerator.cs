using System;

class PasswordGenerator
{
    // Stała długość hasła
    const int PasswordLength = 8;

    // Metoda generująca losowe hasło
    public static string GeneratePassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+";
        Random random = new Random();
        char[] password = new char[PasswordLength];
        for (int i = 0; i < PasswordLength; i++)
        {
            password[i] = chars[random.Next(chars.Length)];
        }
        return new string(password);
    }
}
