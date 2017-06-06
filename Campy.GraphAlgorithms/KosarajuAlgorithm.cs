﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Campy.Graphs;

namespace Campy.GraphAlgorithms
{
    public class KosarajuAlgorithm<NAME>
    {
        private IGraph<NAME> _graph;

        public KosarajuAlgorithm(IGraph<NAME> graph)
        {
            _graph = graph;
        }

        private void fillOrder(IGraph<NAME> gr, NAME v, Dictionary<NAME, bool> visited, Stack<NAME> stack)
        {
            // Mark the current node as visited and print it
            visited[v] = true;

            foreach (var n in gr.Successors(v))
            {
                if (!visited[n])
                    fillOrder(gr, n, visited, stack);
            }

            stack.Push(v);
        }



        public class SCCEnumerator
        {
            private IGraph<NAME> graph;
            private NAME start;
            private Dictionary<NAME, bool> visited;

            public SCCEnumerator(IGraph<NAME> g, NAME s, Dictionary<NAME, bool> v)
            {
                graph = g;
                start = s;
                visited = v;
            }

            // A recursive function to print DFS starting from v
            private List<NAME> DFSUtil(List<NAME> result, IGraph<NAME> g, NAME v, Dictionary<NAME, bool> visited)
            {
                // Mark the current node as visited and print it
                visited[v] = true;

                result.Add(v);

                // For all predecessors and successors to v...
                foreach (var n in g.Successors(v))
                {
                    if (!visited[n])
                        DFSUtil(result, g, n, visited);
                }
                return result;
            }

            public List<NAME> ToList()
            {
                return DFSUtil(new List<NAME>(), graph, start, visited);
            }
        }


        public IEnumerator<List<NAME>> GetEnumerator()
        {
            Stack<NAME> stack = new Stack<NAME>();

            Dictionary<NAME, bool> visited = new Dictionary<NAME, bool>();
            foreach (var i in _graph.Vertices)
                visited[i] = false;

            foreach (var i in _graph.Vertices)
                if (visited[i] == false)
                    fillOrder(_graph, i, visited, stack);

            var gr = Transpose<NAME>.getTranspose(_graph);

            foreach (var i in gr.Vertices)
                visited[i] = false;

            while (stack.Any())
            {
                // Pop a vertex from stack
                var v = stack.Pop();

                // Print Strongly connected component of the popped vertex
                if (visited[v] == false)
                {
                    List<NAME> result = new List<NAME>();
                    yield return new SCCEnumerator(gr, v, visited).ToList();
                }
            }
        }
    }
}