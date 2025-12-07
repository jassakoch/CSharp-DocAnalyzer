// Document Analyzer - A C# Learning Project
using System;
using System.IO;
using System.Linq;

namespace DocumentAnalyzer
{
    // Simple contract for anything that can provide text given a path
    public interface ITextSource
    {
        string ReadAll(string path);
    }

    // Concrete implementation that reads from local files
    public class FileTextSource : ITextSource
    {
        public string ReadAll(string path) => File.ReadAllText(path);
    }

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

                //Read the entire file via injected text source
                ITextSource textSource = new FileTextSource();
                string content = textSource.ReadAll(filePath);

                //Count words
                string[] words = content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                int wordCount = words.Length;


                //Prompt for keywords;
                Console.Write("Enter keywords(comma-separated): ");

                //Grab input
                string? keywordInput = Console.ReadLine();

                //split ;

                if (string.IsNullOrWhiteSpace(keywordInput))
                {
                    Console.WriteLine("No keywords entered. Skipping keyword analysis");
                }
                else
                {

                    string[] keywords = keywordInput.Split(',');
                                

                //create dictionary
                Dictionary<string, int> keywordCounts = new Dictionary<string, int>();

                //Populate dictionary with keywords
                foreach (string keyword in keywords)
                {
                    string cleanKeyword = keyword.Trim().ToLower();
                    keywordCounts[cleanKeyword] = 0;
                }
                }

                //Create a dictionary to count word frequency
                Dictionary<string, int> wordFrequency = new Dictionary<string, int>();

                //Loop through each word
                foreach (string word in words)
                {
                    //convert to lowercase 
                    string lowerWord = word.ToLower();

                    if (wordFrequency.ContainsKey(lowerWord))
                    {
                        wordFrequency[lowerWord]++;
                    }
                    else
                    {
                        //otherwise, add it withcount of 1
                        wordFrequency[lowerWord] = 1;
                    }
                }
                //Display top5 words
                Console.WriteLine("\nTop 5 Most Common Words:");
                var topWords = wordFrequency.OrderByDescending(x => x.Value).Take(5);
                foreach (var item in topWords)
                {
                    Console.WriteLine($"{item.Key}: {item.Value} times");
                }

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