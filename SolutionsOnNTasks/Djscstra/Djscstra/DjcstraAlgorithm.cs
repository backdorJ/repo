using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Djscstra
{
    public class DijcstraAlgorithm
    {
        private static readonly Mutex _mutex = new Mutex();

        public static int[] Dijkstra(int startNode, Dictionary<int, Dictionary<int, int>> graph)
        {
            int n = graph.Count;
            int[] distances = new int[n];
            bool[] visited = new bool[n];

            for (int i = 0; i < n; i++)
            {
                distances[i] = int.MaxValue;
            }

            distances[startNode] = 0;

            // Создаем список задач
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < n; i++)
            {
                int u = i;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    // Находим ближайший не посещенный узел
                    while (true)
                    {
                        int closestUnvisitedNode = GetClosestUnvisitedNode(distances, visited);
                        if (closestUnvisitedNode == -1 || closestUnvisitedNode != u)
                        {
                            break;
                        }

                        visited[u] = true;

                        // Обновляем расстояние до соседних узлов
                        foreach (var neighbor in graph[u])
                        {
                            int v = neighbor.Key;
                            int weight = neighbor.Value;

                            if (!visited[v] && distances[u] != int.MaxValue && distances[u] + weight < distances[v])
                            {
                                _mutex.WaitOne();
                                distances[v] = distances[u] + weight;
                                _mutex.ReleaseMutex();
                            }
                        }
                    }
                }));
            }

            // Дожидаемся выполнения всех задач
            Task.WaitAll(tasks.ToArray());

            return distances;
        }

        private static int GetClosestUnvisitedNode(int[] distances, bool[] visited)
        {
            int minDistance = int.MaxValue;
            int minDistanceNode = -1;

            for (int i = 0; i < distances.Length; i++)
            {
                if (!visited[i] && distances[i] < minDistance)
                {
                    minDistance = distances[i];
                    minDistanceNode = i;
                }
            }

            return minDistanceNode;
        }
    }
}