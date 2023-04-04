using UnityEngine;
using System;
using Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model;
using Vocario.GameStateManager;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;

namespace Vocario.EventBasedArchitecture.EventFlowStateMachine
{
    [Serializable]
    [CreateAssetMenu(fileName = "EventFlowStateMachine_", menuName = "Vocario/EventFlowStateMachine", order = 0)]
    public class EventFlowStateMachine : StateMachine
    {
#if UNITY_EDITOR
        [SerializeField]
        public GraphViewData GraphViewData;
#endif
        [SerializeField]
        private bool _initialized = false;

        private void Awake()
        {
            if (_initialized)
            {
                return;
            }

            GraphViewData = new GraphViewData();
            GraphViewData.Init(this);
            _initialized = true;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}

namespace Vocario.EventBasedArchitecture.EventFlowStateMachine.Editor.Model
{
    [Serializable]
    public class GraphViewData
    {
        [SerializeField]
        protected NodeDataMap _nodeData = new NodeDataMap();
        [SerializeField]
        protected EdgeDataMap _edgeData = new EdgeDataMap();

        public void Init(EventFlowStateMachine stateMachine)
        {
            State state = stateMachine.CreateState<State>();
            _ = CreateNode(Guid.NewGuid(), state.Id, "Start", 0.0f, 0.0f, true);
        }

        public Node CreateNode(Guid id, Guid stateId, string name, float x, float y, bool isInitial)
        {
            var nodeData = new Node
            {
                GraphId = id,
                StateId = stateId,
                Name = name,
                X = x,
                Y = y,
                IsInitial = isInitial
            };
            _nodeData.Add(id.ToString(), nodeData);

            return nodeData;
        }

        public void UpdateNode(Guid id, string name, float x, float y, bool isInitial)
        {
            Node data = _nodeData[ id.ToString() ];
            data.Name = name;
            data.X = x;
            data.Y = y;
            data.IsInitial = isInitial;
        }

        public Node RemoveNode(Guid id)
        {
            Node data = _nodeData[ id.ToString() ];
            _ = _nodeData.Remove(id.ToString());
            return data;
        }

        public Node GetNode(Guid id) => !_nodeData.Contains(id.ToString()) ? null : _nodeData[ id.ToString() ];

        public Node[] GetNodes() => _nodeData.Values.ToArray();

        public void CreatePort(Guid id, Guid nodeId, int index)
        {
            Node node = _nodeData[ nodeId.ToString() ];
            var port = new Port()
            {
                ID = id,
                Index = index
            };
            node.Ports.Add(id.ToString(), port);
        }

        public void UpdatePort(Guid id, Guid nodeId, int index)
        {
            Node node = _nodeData[ nodeId.ToString() ];
            Port port = node.Ports[ id.ToString() ];
            port.Index = index;
        }

        public bool RemovePort(Guid id, Guid nodeId)
        {
            Node node = _nodeData[ nodeId.ToString() ];
            return node.Ports.Remove(id.ToString());
        }

        public Port GetPort(Guid portId, Guid nodeId)
        {
            Node node = _nodeData[ nodeId.ToString() ];
            return node.Ports[ portId.ToString() ];
        }

        public Edge CreateEdge(Guid outPortId, Guid inNodeId, Guid outNodeId)
        {
            var edge = new Edge()
            {
                InNodeId = inNodeId,
                OutNodeId = outNodeId,
                OutPortId = outPortId,
            };
            _edgeData.Add(outPortId.ToString(), edge);
            return edge;
        }

        public Edge RemoveEdge(Guid outPortId)
        {
            Edge edge = _edgeData[ outPortId.ToString() ];
            _ = _edgeData.Remove(outPortId.ToString());
            return edge;
        }

        public Edge[] GetEdges() => _edgeData.Values.ToArray();
    }

    [Serializable]
    public class NodeDataMap : SerializableDictionary<string, Node> { }
    [Serializable]
    public class PortDataMap : SerializableDictionary<string, Port> { }
    [Serializable]
    public class EdgeDataMap : SerializableDictionary<string, Edge> { }
    [Serializable]
    public class StateBehaviourDataMap : SerializableDictionary<string, VisualElement> { }

    [Serializable]
    public class Node
    {
        [SerializeField]
        private string _graphId;
        public Guid GraphId
        {
            get => Guid.Parse(_graphId);
            set => _graphId = value.ToString();
        }
        [SerializeField]
        private string _stateId;
        public Guid StateId
        {
            get => Guid.Parse(_stateId);
            set => _stateId = value.ToString();
        }
        public bool IsInitial;
        public string Name;
        public float X;
        public float Y;
        public PortDataMap Ports = new PortDataMap();
    }

    [Serializable]
    public class Port
    {
        [SerializeField]
        private string _id;
        public Guid ID
        {
            get => Guid.Parse(_id);
            set => _id = value.ToString();
        }
        public string Name = "";
        public int Index = -1;
    }

    [Serializable]
    public class Edge
    {
        [SerializeField]
        private string _outPortId;
        public Guid OutPortId
        {
            get => Guid.Parse(_outPortId);
            set => _outPortId = value.ToString();
        }
        [SerializeField]
        private string _inNodeId;
        public Guid InNodeId
        {
            get => Guid.Parse(_inNodeId);
            set => _inNodeId = value.ToString();
        }
        [SerializeField]
        private string _outNodeId;
        public Guid OutNodeId
        {
            get => Guid.Parse(_outNodeId);
            set => _outNodeId = value.ToString();
        }
    }
}
