using System;
using NWindows;

namespace NWindows.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Application();
            app.Run(new Window());
        }
    }
}