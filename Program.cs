// Document Analyzer - A C# Learning Project
using System;
using System.IO;

namespace DocumentAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===Document Analyzer ===");
            Console.WriteLine("Simple Word Counter\n");

            //Ask user for file path
            Console.Write("Enter the path to your test file: ");
            string? filePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("No file path entered. Exiting.");
                return;
            }

            //Check if file exsists
            if (File.Exists(filePath))
            {

                //Red the entire file
                string content = File.ReadAllText(filePath);

                //Count words
                string[] words = content.Split(' ', '\n', '\r','\t');
                int wordCount = words.Length;

                //Display results
                Console.WriteLine($"\nFile analyzed successfully!");
                Console.WriteLine($"Total words: {wordCount}");

            }
            else 
            {
                Console.WriteLine("File not found. Please check the path.");
                
            }
        }
    }
}