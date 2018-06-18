using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

class FileTask
{
    static void Main(string[] args)
    {
        string sourceDirectory = @"C:\filetasker\deposit";

        try
        {
            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.work");

            foreach (string currentFile in txtFiles)
            {
                string fileName = currentFile.Substring(sourceDirectory.Length + 1);
                Console.WriteLine("{0},{1}", fileName, currentFile);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
