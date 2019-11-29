/*
[문제 설명]
n개의 노드가 있는 그래프가 있습니다.
각 노드는 1부터 n까지 번호가 적혀있습니다. 
1번 노드에서 가장 멀리 떨어진 노드의 갯수를 구하려고 합니다. 
가장 멀리 떨어진 노드란 최단경로로 이동했을 때 간선의 개수가 가장 많은 노드들을 의미합니다.

노드의 개수 n, 간선에 대한 정보가 담긴 2차원 배열 vertex가 매개변수로 주어질 때, 
  1번 노드로부터 가장 멀리 떨어진 노드가 몇 개인지를 return 하도록 solution 함수를 작성해주세요.

[제한사항]
노드의 개수 n은 2 이상 20,000 이하입니다.
간선은 양방향이며 총 1개 이상 50,000개 이하의 간선이 있습니다.
vertex 배열 각 행 [a, b]는 a번 노드와 b번 노드 사이에 간선이 있다는 의미입니다.

[입출력 예]
n	vertex	return
6	[[3, 6], [4, 3], [3, 2], [1, 3], [1, 2], [2, 4], [5, 2]]	3

[입출력 예 설명]
예제의 그래프를 표현하면 아래 그림과 같고, 1번 노드에서 가장 멀리 떨어진 노드는 4,5,6번 노드입니다.
1 - 2 - 5
| / |
3 - 4
|
6
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace FurthestNode
{
    class Program
    {
        static void Main(string[] args)
        {
            Solution sol = new Solution();
            const int n = 6;
            var record = new int[,] {
                { 3, 6 },
                { 4, 3 },
                { 3, 2 },
                { 1, 3 },
                { 1, 2 },
                { 2, 4 },
                { 5, 2 }
                //{ 5, 1 }
            };
            //var record = new int[,] {
            //    { 1, 2 },
            //    { 2, 3 },
            //    { 3, 4 },
            //    { 4, 5 }
            //};
            var answer = sol.solution(n, record);
            Console.WriteLine($"1번 노드와 가장 먼 노드의 갯수: {answer}");
        }
    }

    /// <summary>
    /// 풀이 설명
    /// 
    /// 예시 그래프에 기반해 각 노드에 연결된 노드를 기록해보면,
    /// 1: 2,3
    /// 2: 1,3,4,5
    /// 3: 1,2,4,6
    /// 4: 2,3
    /// 5: 2
    /// 6: 3
    /// 
    /// 노드 1과의 거리를 rank라고 하면, rank=1인 노드는 아래와 같다:
    /// rank[1] = [2,3]
    /// 그럼 노드 2와 3 각각에게 거리가 1인 다른 노드를 찾게 되면,
    ///    노드 1과의 거리가 2인 노드를 찾게 되는 것이며,
    ///    그때마다 어떤 노드를 찾았는지 기록해두며 진행한다면
    ///    중복인 노드를 탐색할 일이 없으므로 노드 간의 최단거리를 얻게 된다.
    /// 따라서 노드 2와 3과 이웃하면서 처음 보는 노드는 다음과 같다:
    /// rank[2] = [4,5,6]
    /// 노드 4,5,6을 탐색해보면 각각 인접한 노드를 이미 찾았기 때문에,
    ///   노드 1과의 거리가 3 이상인 노드는 없는 것을 알 수 있다.
    /// </summary>
    public class Solution
    {
        public int solution(int n, int[,] results)
        {
            int answer = 0;

            Dictionary<int, Node> graph = new Dictionary<int, Node>();
            for (int i = 1; i <= n; i++)
            {
                graph.Add(i, new Node(i));
            }

            // 각 노드와 직접 연결되어있는 노드들을 파악
            for (int i = 0; i < results.Length / 2; i++)
            {
                /*
                 [[4, 3], [4, 2], [3, 2], [1, 2], [2, 5]] 에서
                 i = 0 일 때:
                 [i, 0] == 4
                 [i, 1] == 3
                 => 4 노드에 3과 연결되어있다는 걸 기록
                 => 3 노드에 4와 연결되어있다는 걸 기록
                 부호를 이용하면 방향 정보도 남길 수 있음
                */

                // 중복 연결정보 필터링
                graph[results[i, 0]].Neighbour.Add(results[i, 1]);
                graph[results[i, 1]].Neighbour.Add(results[i, 0]);
            }

            /*
            var connectedNode = new int[n + 1]; // 타 노드와의 거리 정보 담는 배열
            {
                var nodeKV = graph.First(); // 1번 노드 가져오기

                // 직접 연결이 안된 노드를 파악하고 거기에 도달할 수 있는지 알아보기
                //Array.Fill(connectedNode, int.MaxValue); // 전부 inf로 초기화
                Array.Fill(connectedNode, 0); // 전부 inf로 초기화

                HashSet<int> trail = new HashSet<int>();
                trail.Add(nodeKV.Value.Index); // 경로에 출발 노드 기록
                findConnection(connectedNode, ref graph, nodeKV.Value, trail);
            }

            int maxDistance = connectedNode.Where(dist => dist < int.MaxValue).Max();
            answer =
                (from node in graph
                 where maxDistance == connectedNode[node.Value.Index]
                 select node).Count();
            */

            bool[] added = new bool[n + 1];
            Array.Fill(added, false);
            added[0] = true;
            added[1] = true;

            Dictionary<int, List<int>> rankDict = new Dictionary<int, List<int>>();
            rankDict.Add(1, graph[1].Neighbour);
            foreach (var index in graph[1].Neighbour)
            {
                added[index] = true;
            }

            for (int i = 1; i < n; i++)
            {
                List<int> list = new List<int>();
                if (!rankDict.ContainsKey(i))
                {
                    continue;
                }
                foreach (var ranker in rankDict[i])
                {
                    foreach (var neighbour in graph[ranker].Neighbour)
                    {
                        if (added[neighbour] == false)
                        {
                            list.Add(neighbour);
                            added[neighbour] = true;
                         }
                    }
                }
                if (list.Count > 0)
                {
                    rankDict.Add(i + 1, list);
                }
            }

            answer = rankDict.Last().Value.Count;

            return answer;
        }

        /*
        private static void findConnection(int[] connectedNode, ref Dictionary<int, Node> graph, Node node, HashSet<int> trail)
        {
            //1 - 2 - 5
            //| / |
            //3 - 4
            //|
            //6
            
            //Console.WriteLine($"출발 노드: {trail.First()}\t이전 노드: {trail[trail.Count - 2]}\t현재 노드:{node.Index}");
            foreach (var nodeIndex in node.Neighbour)
            {
                if (trail.Contains(nodeIndex))
                {// 이미 거친 노드
                    continue;
                }
                if (connectedNode[nodeIndex] > trail.Count)
                {// 해당 노드까지의 최단경로인가?
                    connectedNode[nodeIndex] = trail.Count;
                    trail.Add(nodeIndex);
                    if (graph[nodeIndex].Neighbour.Count > 1)
                    {
                        findConnection(connectedNode, ref graph, graph[nodeIndex], trail);
                    }
                    trail.Remove(nodeIndex); // 기억한 경로 제거
                }
            }
        }
        */
    }

    public class Node
    {
        public Node(int idx)
        {
            Index = idx;
        }

        public int Index { get; }
        public List<int> Neighbour = new List<int>();
    }
}
