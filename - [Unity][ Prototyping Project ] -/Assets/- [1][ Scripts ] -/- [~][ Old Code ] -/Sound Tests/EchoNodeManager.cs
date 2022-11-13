using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeConnection
{
    public EchoNode start, end;
}

public class EchoNodeManager : MonoBehaviour
{
    [SerializeField] private Transform _echoNodeFolder;
    private readonly List<EchoNode> _echoNodes = new List<EchoNode>();
    
    [SerializeField] private List<NodeConnection> _nodeConnections = new List<NodeConnection>();
    
    
    
    [ContextMenu("Re-Assign")]
    private void ReAssign()
    {
        _echoNodes.Clear();
        
        int childCount = _echoNodeFolder.childCount;
        for (int i = 0; i < childCount; i++)
        {
            _echoNodes.Add(_echoNodeFolder.GetChild(i).GetComponent<EchoNode>());
        }
    }
    
    [ContextMenu("Recalculate")]
    private void Recalculate()
    {
        foreach (EchoNode node in _echoNodes)
        {
            node.connectedNodes.Clear();
        }
        
        foreach (NodeConnection connection in _nodeConnections)
        {
            connection.start.connectedNodes.Add(connection.end);
            connection.end.connectedNodes.Add(connection.start);
        }
    }
}
