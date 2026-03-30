using RainCoreFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleAllocator
{
    Dictionary<Type, Module> _typeModuleDic;
    Dictionary<Type, int> _dependencyDic;
    Dictionary<Type, Module> TypeModuleDic
    {
        get
        {
            if (_typeModuleDic == null)
                _typeModuleDic = new Dictionary<Type, Module>();
            return _typeModuleDic;
        }
    }
    Dictionary<Type, int> DependenceDic
    {
        get
        {
            if (_dependencyDic == null)
                _dependencyDic =new Dictionary<Type, int>();
            return _dependencyDic;
        }
    }
    public Module Allocate<T>() where T : Module
    {
        Type type = typeof(T);
        if (!TypeModuleDic.ContainsKey(type))
        {
            T module = Activator.CreateInstance(type) as T;
            TypeModuleDic.Add(type, module);
            DependenceDic.Add(type, 1);
            module.Init();
            return module;
        }
        else
        {
            DependenceDic[type]++;
            return TypeModuleDic[type];
        }
    }
    public void Release<T>() where T : Module
    {
        Type type = typeof(T);
        if (!TypeModuleDic.ContainsKey(type))
        {
            RCDebug.CONTAINER_FINDMISSING($"无法清除{type}模块：已被清除/未曾加入RainCore");
            return;
        }
        else
        {
            int num=--DependenceDic[type];
            RCDebug.LOG($"模块::{typeof(T)} 当前依赖指数::{num}");
            if (num == 0)
            {
                if (TypeModuleDic.TryGetValue(type, out Module module))
                {
                    module.Release();
                    TypeModuleDic.Remove(type);
                }
            }
            else if (num<0)
            {
                RCDebug.WARN("依赖指数小于0，出现严重逻辑错误");
            }
        }
    }
    public  void PrintDependency()
    {
        string str = "";
        int num = 0;
        foreach (var type in _typeModuleDic.Keys)
        {
            num++;
            str += $"\n模块::{_typeModuleDic[type]} 依赖指数::{_dependencyDic[type]}";
        }
        if (num==0)
        {
            str += "当前无模块存在依赖";
        }
        RCDebug.LOG(str);
    }
}
