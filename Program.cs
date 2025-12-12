using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

    internal class Program
    {
        private static readonly HashSet<string> DefaultStopwords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a","an","the","and","to","of","in","for","on","with","is","it","this","that",
            "i","you","we","they","he","she","my","your","as","at","by","from","be","or","not"
        };

        static int Main(string[] args)
        {
            Console.WriteLine("=== Document Analyzer ===");
            Console.WriteLine("Simple Word Counter and Keyword Checker\n");

            Console.Write("Enter the path to your text file: ");
            string? filePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("No file path entered. Exiting.");
                return 1;
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found. Please check the path.");
                return 2;
            }

            ITextSource textSource = new FileTextSource();
            string content;
            try
            {
                content = textSource.ReadAll(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read file: {ex.Message}");
                return 3;
            }

            // Raw split to get an initial token list
            var rawTokens = content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            int rawWordCount = rawTokens.Length;

            var tokenNormalizer = new Regex("[^\\w#]", RegexOptions.Compiled);

            Console.Write("Enter keywords (comma-separated), or leave blank: ");
            string? keywordInput = Console.ReadLine();

            Dictionary<string, int>? keywordCounts = null;
            if (!string.IsNullOrWhiteSpace(keywordInput))
            {
                var keywords = keywordInput.Split(',')
                    .Select(k =>
                    {
                        var normalized = k.Trim().ToLowerInvariant();
                        normalized = tokenNormalizer.Replace(normalized, string.Empty);
                        return normalized;
                    })
                    .Where(k => k.Length > 0);
                keywordCounts = keywords.Distinct().ToDictionary(k => k, k => 0);
            }

            var wordFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            int meaningfulTokens = 0;

            foreach (var token in rawTokens)
            {
                var lower = token.ToLowerInvariant();
                lower = tokenNormalizer.Replace(lower, string.Empty);

                if (string.IsNullOrEmpty(lower) || lower.Length <= 1) continue;
                if (DefaultStopwords.Contains(lower)) continue;

                meaningfulTokens++;

                if (keywordCounts != null && keywordCounts.ContainsKey(lower))
                {
                    keywordCounts[lower]++;
                }

                if (wordFrequency.ContainsKey(lower))
                    wordFrequency[lower]++;
                else
                    wordFrequency[lower] = 1;
            }

            Console.WriteLine();
            Console.WriteLine("File analyzed successfully!");
            Console.WriteLine($"Raw token count: {rawWordCount}");
            Console.WriteLine($"Meaningful token count (after filtering): {meaningfulTokens}");

            if (keywordCounts != null)
            {
                Console.WriteLine("\nKeyword hits:");
                foreach (var kvp in keywordCounts)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value} times");
                }

                var missing = keywordCounts.Where(k => k.Value == 0).Select(k => k.Key).ToList();
                if (missing.Count > 0)
                {
                    Console.WriteLine("\nKeywords not found:");
                    foreach (var m in missing)
                        Console.WriteLine($" - {m}");
                }
            }

            Console.WriteLine("\nTop 5 Most Common Words:");
            foreach (var pair in wordFrequency.OrderByDescending(x => x.Value).Take(5))
            {
                Console.WriteLine($"{pair.Key}: {pair.Value} times");
            }

            return 0;
        }
    }
}