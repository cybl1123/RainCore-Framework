using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RainCoreFramework
{
    public class RainCore
    {
        static NodeContainer _nodeContainer;
        static ModuleAllocator _moduleAllocator;
        static ModuleAllocator Allocator
        {
            get
            {
                if (_moduleAllocator == null)
                {
                    _moduleAllocator = new ModuleAllocator();
                }
                return _moduleAllocator;
            }
        }
        static NodeContainer NodeContainer
        {
            get
            {
                if (_nodeContainer == null)
                    _nodeContainer = new NodeContainer();
                return _nodeContainer;
            }
        }
        public static void RegistNode(Node node)
        {
            NodeContainer.AddNode(node);
        }
        public static void UnRegistNode(Node node)
        {
            NodeContainer.RemoveNode(node);
        }
        public static Node GetNode(int UID)
        {
            return NodeContainer.GetNode(UID);
        }
        public static bool HasNode(int UID)
        {
            return NodeContainer.NodeExists(UID);
        }
        //后续编写节点查询的扩展
        public static Module AllocateModule<T>() where T : Module
        {
            return Allocator.Allocate<T>();
        }
        public static void ReleaseModule<T>() where T : Module
        {
            Allocator.Release<T>();
        }
        public static void PrintDependency()
        {
            Allocator.PrintDependency();
        }
    }
    public static class RainCoreExtension
    {
        public static void RegistSelfToRainCore(this IBelongToRainCore<Node> node)
        {
            RainCore.RegistNode(node as Node);
        }
        public static void UnRegistSelfToRainCore(this IBelongToRainCore<Node> node)
        {
            RainCore.UnRegistNode(node as Node);
        }
        public static void RainCoreDestory(this IBelongToRainCore<Node> node)
        {
            var Node=node as Node;
            Node.IsDestroyedByRainCore = true;
            Node.Release();
            Node.DestorySelf();
        }
        public static Module RequestModuleFromRainCore<T>(this ICanGetModule<Module> module) where T : Module
        {
            return RainCore.AllocateModule<T>();
        }
        public static void ReleaseModuleFromRainCore<T>(this ICanGetModule<Module> module) where T : Module
        {
            RainCore.ReleaseModule<T>();
        }
        public static Module RequestModuleFromRainCore<T>(this ICanGetModule<RainContainer> rainContainer) where T : Module
        {
            return RainCore.AllocateModule<T>();
        }
        public static void ReleaseModuleFromRainCore<T>(this ICanGetModule<RainContainer> rainContainer) where T : Module
        {
            RainCore.ReleaseModule<T>();
        }
        // 这就是意义，和接口相关的方法不一定要写在接口里面
    }
    public interface IBelongToRainCore<T> where T : IBelongToRainCore<T>
    {
    }
    public interface ICanGetNode<T> where T : ICanGetNode<T>
    {

    }
    public interface ICanGetModule<T> where T : ICanGetModule<T>
    {

    }
    public interface INeedInit
    {
        public bool IsInitialized { get; }
        public virtual void Init() {}
    }
    public interface INeedSelfRelease
    {
        public virtual void Release() { }
    }
}
