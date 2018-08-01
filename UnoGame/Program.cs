using System;
using Microsoft.Owin.Hosting;

namespace UnoGame
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddress = $"http://localhost:9000/";
            WebApp.Start<Startup>(baseAddress);

            Console.ReadLine();
        }
    }
}
