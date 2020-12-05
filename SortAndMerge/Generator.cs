using System;
using System.IO;

namespace SortAndMerge
{
    public static class Generator
    {
        public static void Generate(int num)
        {
            StreamWriter sw1 = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "File1.txt"));
            StreamWriter sw2 = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "File2.txt"));
            for(long i = num; i > 0; i--)
            {
                if (i % 2 == 0)
                    sw1.WriteLine(i);
                else
                    sw2.WriteLine(i);
            }
            sw1.Close();
            sw2.Close();

            Console.WriteLine("The files were created. You can find them here:\n" + Environment.CurrentDirectory.ToString() + "\\...");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
