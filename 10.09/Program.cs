using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        int threadCount = int.Parse(Console.ReadLine());

        int[] array = GenerateRandomArray(1000000);

        ArrayTasks arrayTasks = new ArrayTasks(array);

        var tasks = new List<Task>();

        tasks.Add(Task.Run(() => Console.WriteLine($"Min: {arrayTasks.MinValue()}")));
        tasks.Add(Task.Run(() => Console.WriteLine($"Max: {arrayTasks.MaxValue()}")));
        tasks.Add(Task.Run(() => Console.WriteLine($"Sum: {arrayTasks.Sum()}")));
        tasks.Add(Task.Run(() => Console.WriteLine($"Average: {arrayTasks.Average()}")));

        Task.WaitAll(tasks.ToArray());

        string text = File.ReadAllText("E:\\10.09\\123.txt");

        TextAnalyzer textAnalyzer = new TextAnalyzer(text);

        var textTasks = new List<Task>();

        textTasks.Add(Task.Run(() =>
        {
            var charFrequency = textAnalyzer.CharFrequency();
            Console.WriteLine("Char Frequency:");
            foreach (var kvp in charFrequency)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }));

        textTasks.Add(Task.Run(() =>
        {
            var wordFrequency = textAnalyzer.WordFrequency();
            Console.WriteLine("Word Frequency:");
            foreach (var kvp in wordFrequency)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }));

        Task.WaitAll(textTasks.ToArray());
    }

    static int[] GenerateRandomArray(int size)
    {
        Random rand = new Random();
        int[] array = new int[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = rand.Next(1000);
        }
        return array;
    }
}

class ArrayTasks
{
    private int[] _array;

    public ArrayTasks(int[] array)
    {
        _array = array;
    }

    public int MinValue()
    {
        return _array.Min();
    }

    public int MaxValue()
    {
        return _array.Max();
    }

    public long Sum()
    {
        return _array.Sum();
    }

    public double Average()
    {
        return _array.Average();
    }

    public int[] CopyPart(int start, int length)
    {
        return _array.Skip(start).Take(length).ToArray();
    }
}

class TextAnalyzer
{
    private string _text;

    public TextAnalyzer(string text)
    {
        _text = text;
    }

    public Dictionary<char, int> CharFrequency()
    {
        ConcurrentDictionary<char, int> charFreq = new ConcurrentDictionary<char, int>();

        Parallel.ForEach(_text, c =>
        {
            if (!char.IsWhiteSpace(c))
            {
                charFreq.AddOrUpdate(c, 1, (key, oldValue) => oldValue + 1);
            }
        });

        return charFreq.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public Dictionary<string, int> WordFrequency()
    {
        ConcurrentDictionary<string, int> wordFreq = new ConcurrentDictionary<string, int>();

        var words = _text.Split(new[] { ' ', '\n', '\r', '\t', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

        Parallel.ForEach(words, word =>
        {
            wordFreq.AddOrUpdate(word, 1, (key, oldValue) => oldValue + 1);
        });

        return wordFreq.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}
