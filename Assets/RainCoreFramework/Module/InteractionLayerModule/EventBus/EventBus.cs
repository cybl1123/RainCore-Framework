using CodiceApp.EventTracking.Plastic;
using RainCoreFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EventBus : Module
{
    Dictionary<string, Action> strActionDic;
    Dictionary<Type, Action> typeActionDic;
    public EventBus Subscribe(string eventName, Action action)
    {
        if (action == null)
        {
            RCDebug.NOTICE("EventBus", "请勿传入空事件");
            return this;
        }
        if (!strActionDic.ContainsKey(eventName))
        {
            strActionDic.Add(eventName, action);
            RCDebug.NOTICE("EventBus", $"首次订阅{eventName}事件");
        }
        else
        {
            strActionDic[eventName] += action;
            RCDebug.NOTICE("EventBus", $"订阅{eventName}事件");
        }
        return this;
    }
    public EventBus Subscribe<T>(Action action) where T : struct
    {
        if (action == null)
        {
            RCDebug.NOTICE("EventBus", "请勿传入空事件");
        }
        Type structType= typeof(T);
        if (!typeActionDic.ContainsKey(structType))
        {
            typeActionDic.Add(structType, action);
        }
        else
        {
            typeActionDic[structType] += action;
        }
        return this;
    }
    public EventBus UnSubscribe(string eventName, Action action)
    {
        if (action == null)
        {
            RCDebug.NOTICE("EventBus", "请勿传入空事件");
        }
        if (!strActionDic.ContainsKey(eventName))
        {
            RCDebug.NOTICE("EventBus", "请勿传入正确的EventName");
        }
        else
        {
            strActionDic[eventName] -= action;
            if (strActionDic[eventName] == null)
            {
                strActionDic.Remove(eventName);
            }
        }
        return this;
    }
    public EventBus UnSubscribe<T>(Action action) where T : struct
    {
        if (action == null)
        {
            RCDebug.NOTICE("EventBus", "请勿传入空事件");
        }
        Type structType = typeof(T);
        if (!typeActionDic.ContainsKey(structType))
        {
            RCDebug.NOTICE("EventBus", "请勿传入正确的StructType");
        }
        else
        {
            typeActionDic[structType] -= action;
            if (typeActionDic[structType] == null)
            {
                typeActionDic.Remove(structType);
            }
        }
        return this;
    }
    public void Publish(string eventName)
    {
        if (!strActionDic.ContainsKey(eventName))
        {
            RCDebug.NOTICE($"Eventbus", $"未有节点订阅{eventName}类型事件，请传入正确的事件名称");
        }
        else 
        {
            strActionDic[eventName].Invoke();
            RCDebug.NOTICE($"Eventbus", $"{eventName}事件发布");
        }
    }
    public void Publish<T>() where T : struct
    {
        Type structType = typeof(T);
        if (!typeActionDic.ContainsKey(structType))
        {
            RCDebug.NOTICE($"Eventbus", $"未有节点订阅{structType}类型事件，请传入正确的结构体类型");
        }
        else
        {
            typeActionDic[structType].Invoke();
            RCDebug.NOTICE($"Eventbus", $"{structType}事件发布");
        }
    }
    public override void Init()
    {
        strActionDic = new Dictionary<string, Action>();
        typeActionDic = new Dictionary<Type, Action>();
    }

    public override void Release()
    {
        strActionDic.Clear();
        typeActionDic.Clear();
        strActionDic = null;
        typeActionDic=null;
    }
}
