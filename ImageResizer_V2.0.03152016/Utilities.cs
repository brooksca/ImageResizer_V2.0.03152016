using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer_V2._0._03152016
{
    public static class Utilities
    {
        public static void WriteLine(this string message)
        {
            Console.WriteLine(message);
        }

        public static void Write(this string message)
        {
            Console.Write(message);
        }

        public static void PressAnyKeyToExit()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static bool CheckDependencies()
        {
            try
            {
                return new Ping().Send("schintranet").Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}
