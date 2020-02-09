namespace NWindows.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var app = new NApplication())
            {
                app.Init();
                app.Run(new MainWindow());
            }
        }
    }
}