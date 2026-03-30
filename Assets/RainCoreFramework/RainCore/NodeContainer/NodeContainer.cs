using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RainCoreFramework
{
    public class NodeContainer
    {
        Dictionary<int, Node> _uidNodeDic;
        Dictionary<int, Node> UIDNodeDic
        {
            get
            {
                if (_uidNodeDic == null)
                    _uidNodeDic = new Dictionary<int, Node>();
                return _uidNodeDic;
            }
        }
        int _uidCounter;
        public NodeContainer()
        {
            _uidCounter = 0;
        }
        public void AddNode(Node node)
        {
            if (node == null)
            {
                RCDebug.Null_Exception(node.name);
                return;
            }
            else if (node.IsInNodeContainer && UIDNodeDic.ContainsKey(node.UID))
            {
                RCDebug.NOTICE("请勿添加重复节点");
                return;
            }
            node.UID = _uidCounter++;
            UIDNodeDic.Add(node.UID, node);
            node.IsInNodeContainer = false;
            RCDebug.SUCCEED("节点添加");
        }
        public void RemoveNode(Node node)
        {
            if (node == null || !UIDNodeDic.ContainsKey(node.UID))
            {
                RCDebug.NOTICE("节点不存在，无法删除");
                return;
            }
            UIDNodeDic.Remove(node.UID);
            node.IsInNodeContainer = false;
            RCDebug.SUCCEED("节点删除");
        }
        public void RemoveNode(int UID)
        {
            if (!UIDNodeDic.ContainsKey(UID))
            {
                RCDebug.NOTICE("UID不存在，无法删除");
                return;
            }
            if (UIDNodeDic.TryGetValue(UID, out Node node))
            {
                node.IsInNodeContainer = false;
            }
            UIDNodeDic.Remove(UID);
            RCDebug.SUCCEED("节点删除");
        }
        public Node GetNode(int UID)
        {
            if (!UIDNodeDic.ContainsKey(UID))
            {
                RCDebug.NOTICE("UID不存在，无法获取");
            }
            return null;
        }
        public bool NodeExists(int UID)
        {
            if (!UIDNodeDic.ContainsKey(UID))
            {
                RCDebug.NOTICE("UID不存在，无法获取");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 返回IEnumerable可迭代对象的方法
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Node> GetNodes()
        {
            return null;
        }
    }
}
