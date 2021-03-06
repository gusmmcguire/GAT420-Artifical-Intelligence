using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAgent : Agent
{
    [SerializeField] protected Node initialNode;


    public GraphNode sourceNode { get; set; }
    public GraphNode destinationNode { get; set; }
    public Node targetNode { get; set; }

    List<GraphNode> path = new List<GraphNode>();

    private void Start()
    {
        do
        {
            sourceNode = Node.GetRandomNode<GraphNode>();
            destinationNode = Node.GetRandomNode<GraphNode>();
        } while (sourceNode == destinationNode);

        targetNode = sourceNode;

        GeneratePath();
    }

    void Update()
    {
        //Debug.DrawLine(transform.position, sourceNode.transform.position, Color.green);
        //Debug.DrawLine(transform.position, destinationNode.transform.position, Color.red);
        //Debug.DrawLine(transform.position + Vector3.up, targetNode.transform.position + Vector3.up, Color.yellow);
        if (targetNode)
        {
            movement.MoveTowards(targetNode.transform.position);
        }
    }

    public GraphNode GetNextNode(GraphNode graphNode)
    {
        GraphNode nextNode;
        if (path.Count == 0) return null;

        int index = path.FindIndex(node => node == graphNode);

        if (index == path.Count - 1)
        {
            sourceNode = destinationNode;
            do
            {
                destinationNode = Node.GetRandomNode<GraphNode>();
            } while (sourceNode == destinationNode);
            GeneratePath();
            index = 0;
        }

        nextNode = path[index + 1];

        return nextNode;
    }

    private void GeneratePath()
    {
        GraphNode.ResetNodes();
        Search.BuildPath(Search.AStar, sourceNode, destinationNode, ref path);
    }
}
