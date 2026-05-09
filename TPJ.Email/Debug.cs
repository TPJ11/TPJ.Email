namespace TPJ.Email;
internal static class Debug
{
    public static void WriteLine(bool debug, string message)
    {
        if (!debug)
            return;

        if (string.IsNullOrWhiteSpace(message))
            Console.WriteLine();
        else
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} - TPJ.Email: {message}");
    }
}