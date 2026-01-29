namespace MunicipalServices.Utils
{
    public class Graph<T> where T : notnull
    {
        private readonly Dictionary<T, Dictionary<T, int>> _adjacencyList = new Dictionary<T, Dictionary<T, int>>();

        public void AddVertex(T vertex)
        {
            if (!_adjacencyList.ContainsKey(vertex))
            {
                _adjacencyList[vertex] = new Dictionary<T, int>();
            }
        }

        public void AddEdge(T source, T destination, int weight)
        {
            if (!_adjacencyList.ContainsKey(source) || !_adjacencyList.ContainsKey(destination))
            {
                throw new ArgumentException("Source or destination vertex not found in the graph.");
            }

            _adjacencyList[source][destination] = weight;
            _adjacencyList[destination][source] = weight; // For an undirected graph
        }

        /// <summary>
        /// Finds the shortest path between two vertices using Dijkstra's algorithm.
        /// </summary>
        /// <param name="startVertex">The starting vertex.</param>
        /// <param name="endVertex">The ending vertex.</param>
        /// <returns>A list of vertices representing the shortest path, or null if no path is found.</returns>
        public List<T>? FindShortestPath(T startVertex, T endVertex)
        {
            if (!_adjacencyList.ContainsKey(startVertex) || !_adjacencyList.ContainsKey(endVertex))
            {
                return null; // Or throw an exception
            }

            var distances = new Dictionary<T, int>();
            var previous = new Dictionary<T, T?>();
            var priorityQueue = new PriorityQueue<T, int>();
            var path = new List<T>();

            foreach (var vertex in _adjacencyList.Keys)
            {
                if (EqualityComparer<T>.Default.Equals(vertex, startVertex))
                {
                    distances[vertex] = 0;
                    priorityQueue.Enqueue(vertex, 0);
                }
                else
                {
                    distances[vertex] = int.MaxValue;
                    priorityQueue.Enqueue(vertex, int.MaxValue);
                }
                previous[vertex] = default(T);
            }

            while (priorityQueue.Count > 0)
            {
                T currentVertex = priorityQueue.Dequeue();

                if (EqualityComparer<T>.Default.Equals(currentVertex, endVertex))
                {
                    T? pathNode = currentVertex;
                    while (pathNode != null && previous.TryGetValue(pathNode, out T? prevNode))
                    {
                        path.Add(pathNode);
                        pathNode = prevNode;
                    }
                    path.Reverse();
                    return path;
                }

                if (distances[currentVertex] == int.MaxValue)
                {
                    break; // No path found
                }

                foreach (var neighbor in _adjacencyList[currentVertex])
                {
                    int newDist = distances[currentVertex] + neighbor.Value;

                    if (newDist < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = newDist;
                        previous[neighbor.Key] = currentVertex;
                        priorityQueue.Enqueue(neighbor.Key, newDist);
                    }
                }
            }

            return null; // No path found
        }
    }
}
