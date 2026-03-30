using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace RainCoreFramework
{
    public class RainContainer : ICanGetModule<RainContainer>
    {
        Dictionary<Type, Module> _moduleContainer;

        // params Type[] moduleTypes 一个优化的地方，但是我不会大胆地去写，因为不是必须，而且涉及我不了解的层面
        // 传入变长参数统一进行初始化
        public RainContainer()
        {
            _moduleContainer = new Dictionary<Type, Module>();
            //foreach (var moduleType in moduleTypes)
            //{
            //    if (moduleType.IsAssignableFrom(typeof(Module)))
            //    {
            //        GetModule<T>()
            //    }
            //}
        }
        public T GetModule<T>() where T : Module
        {
            if (!_moduleContainer.ContainsKey(typeof(T)))
            {
                RCDebug.CONTAINER_FINDMISSING($"{typeof(T)}模块注册");
                var module = ReguestModule<T>();
                _moduleContainer.Add(typeof(T), module);
                return module;
            }
            else if (_moduleContainer.TryGetValue(typeof(T), out Module module))
            {
                if (module == null)
                {
                    RCDebug.Null_Exception("模块为空，重新申请");
                    return ReguestModule<T>();
                }
                return module as T;
            }
            else
            {
                return null;
            }
        }
        T ReguestModule<T>() where T : Module
        {
            var module = this.RequestModuleFromRainCore<T>();
            if (module == null)
            {
                RCDebug.Null_Exception($"获取 Module::{typeof(T)} 为空");
                return null;
            }
            return module as T;
        }
        
        public void ReleaseModule<T>(string nodeName) where T : Module
        {
            if (!_moduleContainer.ContainsKey(typeof(T)))
            {
                RCDebug.NOTICE($"容器不存在{typeof(T)}模块依赖，无需释放");
                return;
            }
            else
            {
                _moduleContainer.Remove(typeof(T));
                this.ReleaseModuleFromRainCore<T>();
                RCDebug.LOG($"Node::{nodeName} 释放 Module::{typeof(T)} 成功");
            }
        }
        public void ReleaseAllModule(params object[] nodeName)
        {
            MethodInfo method = GetType().GetMethod("ReleaseModule");
            var typeList = new List<Type>(_moduleContainer.Keys);
            foreach (Type item in typeList)
            {
                MethodInfo genericMethod= method.MakeGenericMethod(item);
                genericMethod.Invoke(this,nodeName);
            }
        }
        public T GetModule<T>(ref T module) where T : Module
        {
            if (!_moduleContainer.ContainsKey(typeof(T)))
            {
                RCDebug.CONTAINER_FINDMISSING($"{typeof(T)}模块注册");
                module = ReguestModule<T>();
                _moduleContainer.Add(typeof(T), module);
            }
            else if (_moduleContainer.TryGetValue(typeof(T), out var item))
            {
                if (item == null)
                {
                    RCDebug.Null_Exception("模块为空，重新申请");
                    item = ReguestModule<T>();
                }
                module= item as T;
            }
            if (module==null)
            {
                RCDebug.Null_Exception("请检查模块分配逻辑");
            }
            return module;
        }
    }
}
