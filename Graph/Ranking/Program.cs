/*
문제 설명
n명의 권투선수가 권투 대회에 참여했고 각각 1번부터 n번까지 번호를 받았습니다.
권투 경기는 1대1 방식으로 진행이 되고, 만약 A 선수가 B 선수보다 실력이 좋다면 A 선수는 B 선수를 항상 이깁니다.
심판은 주어진 경기 결과를 가지고 선수들의 순위를 매기려 합니다.
하지만 몇몇 경기 결과를 분실하여 정확하게 순위를 매길 수 없습니다.

선수의 수 n, 경기 결과를 담은 2차원 배열 results가 매개변수로 주어질 때
정확하게 순위를 매길 수 있는 선수의 수를 return 하도록 solution 함수를 작성해주세요.

제한사항
선수의 수는 1명 이상 100명 이하입니다.
경기 결과는 1개 이상 4,500개 이하입니다.
results 배열 각 행 [A, B]는 A 선수가 B 선수를 이겼다는 의미입니다.
모든 경기 결과에는 모순이 없습니다.

입출력 예
n	results	return
5	[[4, 3], [4, 2], [3, 2], [1, 2], [2, 5]]	2
입출력 예 설명
1: +2
2: -1,-3,-4,+5
3: +2,-4
4: +2,+3
5: -2


2번 선수는 [1, 3, 4] 선수에게 패배했고 5번 선수에게 승리했기 때문에 4위입니다.
5번 선수는 4위인 2번 선수에게 패배했기 때문에 5위입니다.
 */

 
using System;
using System.Collections.Generic;
using System.Linq;
namespace Ranking
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            Solution sol = new Solution();
            //var record = new int[,] {
            //    { 4, 3 },
            //    { 4, 2 },
            //    { 3, 2 },
            //    { 1, 2 },
            //    { 2, 5 },
            //    //{ 5, 1 }
            //};
            var record = new int[,] {
                { 1, 2 },
                { 2, 3 },
                { 3, 4 },
                { 4, 5 }
            };
            var answer = sol.solution(5, record);
            Console.WriteLine($"확실한 선수의 수: {answer}");
        }
    }

    /// <summary>
    /// 승패 여부를 부호로 남겨두게 되어있습니다
    /// 그래서 출발 노드 시점에서 이웃 노드로 넘어갈 때의 부호를 유지하는 것으로 노드 탐색의 방향을 일관되게 만들었습니다
    /// 한붓그리기 같은 느낌으로 한 선수를 기준으로 승승승승승... 혹은 패패패패패... 를
    /// 다른 모든 노드(선수)에 대해 추적 가능하면 그 선수에 대한 순위를 결정할 수 있을 거라 생각하고 코드를 짰습니다
    /// 
    /// 핵심 아이디어
    /// - 노드가 n개 있을 때 어떤 노드와 방향 관계없이 한 번에 연결된 다른 노드의 수가 
    ///   * (n-1)개면 그 노드의 순위는 무조건 알 수 있다
    ///   * (n-1)보다 작으면 그 노드와 인접한 노드를 연쇄적으로 추적해서,
    ///       한 번에 연결되어 있지 않은 노드에 도달할 수 있는지 알아본다
    ///     - 승패 관계를 벡터의 방향처럼 생각한다면, 같은 벡터 방향의 경로만 탐색해나가면 된다.
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

            // 각 노드가 직접 연결되어있는 노드들을 파악
            for (int i = 0; i < results.Length / 2; i++)
            {
                /*
                 [[4, 3], [4, 2], [3, 2], [1, 2], [2, 5]] 에서
                 i = 0 일 때:
                 [i, 0] == 4
                 [i, 1] == 3
                 => 4 노드에 3과 연결되어있다는 걸 기록
                 => 3 노드에 4와 연결되어있다는 걸 기록
                 부호를 이용하면 승패여부 기록 가능
                */

                // 중복 경기결과 필터링
                if (!graph[results[i, 0]].Neighbour.Contains(results[i, 1]))
                {
                    graph[results[i, 0]].Neighbour.Add(results[i, 1]);
                }
                if (!graph[results[i, 1]].Neighbour.Contains(-results[i, 0]))
                {
                    graph[results[i, 1]].Neighbour.Add(-results[i, 0]);
                }
            }

            var connectedNode = new bool[n + 1];
            foreach (var nodeKV in graph)
            {
                if (nodeKV.Value.Neighbour.Count == (n - 1))
                {// 자기 이외의 모든 노드와 연결이 되어있어 바로 등수를 알 수 있음.
                    answer++;
                }
                else
                {// 직접 연결이 안된 노드를 파악하고 거기에 도달할 수 있는지 알아보기
                    Array.Fill(connectedNode, false); // 전부 false로 초기화
                    connectedNode[0] = true; // 더미 원소
                    connectedNode[nodeKV.Key] = true; // 자기 자신

                    List<int> trail = new List<int>();
                    trail.Add(nodeKV.Value.Index);
                    findConnection(connectedNode, ref graph, nodeKV.Value, ref trail);
                    if (connectedNode.All(linked => linked == true))
                    {
                        answer++;
                    }
                }
            }

            return answer;
        }

        private static void findConnection(bool[] connectedNode, ref Dictionary<int, Node> graph, Node node, ref List<int> trail, int prevDirection = 0)
        {
            if (trail.Count == 1)
            {// 출발 노드
                //Console.WriteLine($"출발 노드: {node.Index}");

                foreach (var nodeIndex in node.Neighbour)
                {
                    connectedNode[Math.Abs(nodeIndex)] = true;
                    trail.Add(Math.Abs(nodeIndex));
                    int direction = nodeIndex > 0 ? 1 : -1;
                    findConnection(connectedNode, ref graph, graph[Math.Abs(nodeIndex)], ref trail, direction);
                }
            }
            else
            {
                //Console.WriteLine($"출발 노드: {trail.First()}\t이전 노드: {trail[trail.Count - 2]}\t현재 노드:{node.Index}");
                foreach (var nodeIndex in node.Neighbour)
                {
                    int direction = nodeIndex > 0 ? 1 : -1;
                    if (prevDirection != direction)
                    {
                        continue;
                    }
                    else if (trail.Contains(Math.Abs(nodeIndex)))
                    {
                        continue;
                    }
                    else
                    {
                        connectedNode[Math.Abs(nodeIndex)] = true;
                        trail.Add(Math.Abs(nodeIndex));
                        findConnection(connectedNode, ref graph, graph[Math.Abs(nodeIndex)], ref trail, direction);
                    }
                }
            }
        }
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
