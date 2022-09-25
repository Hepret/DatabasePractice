using System;

namespace DatabaseCopierUI
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Application.Run();
            }
            catch (Exception e)
            {
                Exception exception = e;
                while (exception != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(exception.Message);
                    Console.ResetColor();
                    Console.WriteLine(exception.StackTrace);
                    Console.WriteLine("##################################################");
                    exception = exception.InnerException;
                }
            }
        }
    }
}