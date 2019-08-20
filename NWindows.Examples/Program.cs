namespace NWindows.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Application();
            app.Init();
            app.Run(new MainWindow());
        }
    }
}