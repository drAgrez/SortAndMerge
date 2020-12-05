using System;
using System.IO;

namespace SortAndMerge
{
    class Program
    {
        static void Main(string[] args)
        {
            bool done = false;
            char c;
            SortAndMergeClass sorter = new SortAndMergeClass();
            while(!done)
            {
                Console.WriteLine("Press 1 to create 2 files\nPress 2 to sort and merge files\nPress 3 to exit");
                c = Console.ReadKey().KeyChar;
                switch (c)
                {
                    case '1':
                        Console.WriteLine("\nEnter a number to generate files");
                        Generator.Generate(int.Parse(Console.ReadLine()));
                        break;
                    case '2':
                        sorter.Split(Path.Combine(Environment.CurrentDirectory, "File1.txt"));
                        sorter.Split(Path.Combine(Environment.CurrentDirectory, "File2.txt"));
                        sorter.SortTheParts();
                        sorter.MergeTheParts();
                        done = true;
                        Console.WriteLine("The files were sorted and merged. You can find the new file here:\n" + Environment.CurrentDirectory.ToString() + "\\MergeAndSortFile.txt");
                        break;
                    case '3':
                        done = true;
                        break;
                    default:
                        continue;
                }
            }
        }
    }
}
