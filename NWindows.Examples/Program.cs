namespace NWindows.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var app = NApplication.Create())
            {
                app.Run(new MainWindow());
            }
        }
    }
}