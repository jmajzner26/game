using System.Collections.Generic;
using UnityEngine;

namespace LostHorizon.Resources
{
    /// <summary>
    /// Manages all resources in the world and their spawning
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [Header("Resource Nodes")]
        [SerializeField] private List<ResourceNode> activeResourceNodes = new List<ResourceNode>();

        [Header("Spawn Settings")]
        [SerializeField] private bool autoRegisterNodes = true;
        [SerializeField] private float resourceScanRadius = 100f;

        private Dictionary<ResourceType, List<ResourceNode>> nodesByType = new Dictionary<ResourceType, List<ResourceNode>>();

        public void Initialize()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (autoRegisterNodes)
            {
                RegisterAllResourceNodes();
            }
        }

        private void RegisterAllResourceNodes()
        {
            ResourceNode[] allNodes = FindObjectsOfType<ResourceNode>();
            activeResourceNodes.Clear();
            nodesByType.Clear();

            foreach (ResourceNode node in allNodes)
            {
                RegisterResourceNode(node);
            }

            Debug.Log($"Registered {activeResourceNodes.Count} resource nodes");
        }

        public void RegisterResourceNode(ResourceNode node)
        {
            if (node == null) return;

            if (!activeResourceNodes.Contains(node))
            {
                activeResourceNodes.Add(node);
            }

            ResourceType type = node.ResourceType;
            if (!nodesByType.ContainsKey(type))
            {
                nodesByType[type] = new List<ResourceNode>();
            }

            if (!nodesByType[type].Contains(node))
            {
                nodesByType[type].Add(node);
            }
        }

        public void UnregisterResourceNode(ResourceNode node)
        {
            if (node == null) return;

            activeResourceNodes.Remove(node);

            ResourceType type = node.ResourceType;
            if (nodesByType.ContainsKey(type))
            {
                nodesByType[type].Remove(node);
            }
        }

        public List<ResourceNode> GetNodesOfType(ResourceType type)
        {
            if (nodesByType.ContainsKey(type))
            {
                return new List<ResourceNode>(nodesByType[type]);
            }
            return new List<ResourceNode>();
        }

        public ResourceNode FindNearestNode(ResourceType type, Vector3 position)
        {
            List<ResourceNode> nodes = GetNodesOfType(type);
            ResourceNode nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (ResourceNode node in nodes)
            {
                if (node == null || node.IsDepleted) continue;

                float distance = Vector3.Distance(position, node.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = node;
                }
            }

            return nearest;
        }

        public List<ResourceNode> GetNodesInRange(Vector3 center, float radius)
        {
            List<ResourceNode> nodesInRange = new List<ResourceNode>();

            foreach (ResourceNode node in activeResourceNodes)
            {
                if (node == null) continue;

                float distance = Vector3.Distance(center, node.transform.position);
                if (distance <= radius)
                {
                    nodesInRange.Add(node);
                }
            }

            return nodesInRange;
        }

        public int GetTotalResourceCount(ResourceType type)
        {
            List<ResourceNode> nodes = GetNodesOfType(type);
            int total = 0;

            foreach (ResourceNode node in nodes)
            {
                if (node != null && !node.IsDepleted)
                {
                    total += node.CurrentResources;
                }
            }

            return total;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}

