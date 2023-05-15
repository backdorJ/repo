using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Djscstra
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            int[] inputSizes =
            {
                100, 300, 500, 700, 900, 1100, 1300, 1500, 1700, 1900,
                2100, 2300, 2400, 2600, 2800, 3000, 3200, 3300, 3500, 3700, 3900, 4100, 4300, 4500, 4700, 4900,
                5100, 5300, 5400, 5600, 5800, 6100, 6300, 6400, 6600, 6800, 7100, 7300, 7400, 7600, 7800,
                8100, 8300, 8400, 8600, 8800, 9100, 9300, 9400, 9600, 9800, 10000
            };
            int numTests = 1;

            //Происходит генерация данных в файлы
            foreach (int size in inputSizes)
            {
                GenerateTestData(size, numTests);
            }

            foreach (int size in inputSizes)
            {
                string testDataDir = $"test_data_{size}";

                int totalIterations = 0;
                long totalTime = 0;

                for (int i = 0; i < numTests; i++)
                {

                    string inputFile = Path.Combine(testDataDir, $"test_input_{i}.txt");

                    //Считываем граф из файла
                    var graph = LoadGraphFromFile(inputFile);
                    int startNode = 0;

                    var sw = Stopwatch.StartNew();
                    int[] shortestDistances = DijcstraAlgorithm.Dijkstra(startNode, graph);
                    sw.Stop();

                    long elapsedTicks = sw.ElapsedTicks;
                    long elapsedMs = sw.ElapsedMilliseconds;

                    totalIterations += graph.Count;
                    totalTime += elapsedTicks;

                    Console.WriteLine(
                        $"Input size: {size}, Test case: {i + 1}, Elapsed time: {elapsedMs} ms, Iterations: {graph.Count}");
                }

                double averageIterations = (double)totalIterations / numTests;
                double averageTime = (double)totalTime / numTests;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{averageTime / Stopwatch.Frequency}");
                Console.WriteLine(
                    $"Average time for input size {size}: {averageTime} ticks, {averageTime / Stopwatch.Frequency} seconds");
                Console.WriteLine($"Average iterations for input size {size}: {averageIterations}");
                Console.ResetColor();
            }
        }

        public static Dictionary<int, Dictionary<int, int>> LoadGraphFromFile(string inputFile)
        {
            var graph = new Dictionary<int, Dictionary<int, int>>();

            using (var sr = new StreamReader(inputFile))
            {
                int nodeIndex = 0;

                while (!sr.EndOfStream)
                {
                    string[] lineParts = sr.ReadLine().Split(',');

                    var neighbors = new Dictionary<int, int>();

                    for (int i = 0; i < lineParts.Length; i++)
                    {
                        int weight = int.Parse(lineParts[i]);
                        if (weight != 0)
                        {
                            neighbors.Add(i, weight);
                        }
                    }

                    graph.Add(nodeIndex, neighbors);
                    nodeIndex++;
                }
            }

            return graph;
        }

        static void GenerateTestData(int size, int numTests)
        {
            string testDataDir = $"test_data_{size}";

            if (!Directory.Exists(testDataDir))
            {
                Directory.CreateDirectory(testDataDir);
            }

            Random rand = new Random();

            for (int i = 0; i < numTests; i++)
            {
                string inputFile = Path.Combine(testDataDir, $"test_input_{i}.txt");

                using (var sw = new StreamWriter(inputFile))
                {
                    for (int j = 0; j < size; j++)
                    {
                        for (int k = 0; k < size; k++)
                        {
                            int weight = rand.Next(1, 101);

                            if (k == size - 1)
                                sw.Write($"{weight}\n");
                            else
                                sw.Write($"{weight},");
                        }
                    }
                }
            }
        }
    }
}