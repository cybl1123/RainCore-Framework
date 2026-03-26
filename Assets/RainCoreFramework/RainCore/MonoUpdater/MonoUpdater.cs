using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TextMateSharp.Internal.Parser;
using UnityEngine;

public enum UpdateType
{ 
    Update,
    FixedUpdate,
    LateUpdate,
}
public class MonoUpdater : MonoBehaviour
{
    static MonoUpdater _instance;
    Dictionary<int, int> _orderCounter;
    List<int> existOrder;
    Dictionary<int, Action> _fixedUpdateActionDic;
    Dictionary<int, Action> _updateActionDic;
    Dictionary<int, Action> _lateUpdateActionDic;
    #region ÊôÐÔ·â×°Æ÷
    public Dictionary<int, int> OrderCounter
    {
        get
        {
            if (_orderCounter == null)
                _orderCounter = new Dictionary<int, int>();
            return _orderCounter;
        }
    }
    public List<int> ExistOrder 
    {
        get 
        {
        if (existOrder == null)
                existOrder = new List<int>();
        return existOrder;
        }
    }
    public Dictionary<int, Action> FixedUpdateActionDic
    {
        get
        {
            if (_fixedUpdateActionDic == null)
                _fixedUpdateActionDic = new Dictionary<int, Action>();
            return _fixedUpdateActionDic;
        }
    }
    public Dictionary<int, Action> UpdateActionDic
    {
        get
        {
            if (_updateActionDic == null)
                _updateActionDic = new Dictionary<int, Action>();
            return _updateActionDic;
        }
    }
    public Dictionary<int, Action> LateUpdateActionDic
    {
        get
        {
            if (_lateUpdateActionDic == null)
                _lateUpdateActionDic = new Dictionary<int, Action>();
            return _lateUpdateActionDic;
        }
    }
    public static MonoUpdater instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("[MonoUpdater]");
                _instance = obj.AddComponent<MonoUpdater>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    #endregion
    public static void AddUpdateAction(UpdateType updateType, int order, Action action)
    {
        if (action == null) return;
        var dic1 = instance.OrderCounter;
        Dictionary<int,Action> dic2= instance.UpdateActionDic;
        var list1 = instance.ExistOrder;
        switch (updateType)
        {
            case UpdateType.Update:
                dic2 = instance.UpdateActionDic;
                break;
            case UpdateType.FixedUpdate:
                dic2 = instance.FixedUpdateActionDic;
                break;
            case UpdateType.LateUpdate:
                dic2 = instance.LateUpdateActionDic;
                break;
        }
        if (dic1.ContainsKey(order))
        {
            dic1[order]++;
        }
        else
        {
            dic1.Add(order, 1);
            list1.Add(order);
            list1.Sort();
        }
        if (dic2.ContainsKey(order))
        {
            dic2[order] += action;
        }
        else
        {
            dic2.Add(order, action);
        }
    }
    public static void RemoveUpdateAction(UpdateType updateType, int order, Action action)
    {
        if (action == null) return;
        var dic1 = instance.OrderCounter;
        Dictionary<int, Action> dic2 = instance.UpdateActionDic;
        var list1 = instance.ExistOrder;
        switch (updateType)
        {
            case UpdateType.Update:
                dic2 = instance.UpdateActionDic;
                break;
            case UpdateType.FixedUpdate:
                dic2 = instance.FixedUpdateActionDic;
                break;
            case UpdateType.LateUpdate:
                dic2 = instance.LateUpdateActionDic;
                break;
        }
        if (dic1.ContainsKey(order))
        {
            dic1[order]--;
            dic2[order] -= action;
        }
        else
            return;
        if (dic1[order] == 0)
        {
            dic1.Remove(order);
            list1.Remove(order);
        }
        if (dic2[order] == null)
            dic2.Remove(order);
    }
    private void Awake()
    {
        if (_instance != null&&_instance!=this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        for (int i = 0;i< ExistOrder.Count;i++) 
        {
            UpdateActionDic[ExistOrder[i]]?.Invoke();
        }
    }
    private void LateUpdate()
    {
        for (int i = 0; i < ExistOrder.Count; i++)
        {
            LateUpdateActionDic[ExistOrder[i]]?.Invoke();
        }
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < ExistOrder.Count; i++)
        {
            FixedUpdateActionDic[ExistOrder[i]]?.Invoke();
        }
    }
}
public interface ICanUseMonoUpdater
{
    int UpdateOrder
    {
        get;
        set;
    }
    bool InUpdater
    {
        get;
        set;
    }
    public void FixedUpdate();
    public void Update();
    public void LateUpdate();
}
static class MonoUpdaterExtension
{
    public static void EnableMonoUpdate(this ICanUseMonoUpdater item)
    {
        if (item.InUpdater)
            return;
        int order = item.UpdateOrder;
        MonoUpdater.AddUpdateAction(UpdateType.Update,order,item.Update);
        MonoUpdater.AddUpdateAction(UpdateType.FixedUpdate, order, item.FixedUpdate);
        MonoUpdater.AddUpdateAction(UpdateType.LateUpdate, order,item.LateUpdate);
        item.InUpdater = true;
    }
    public static void DisableMonoUpdate(this ICanUseMonoUpdater item)
    {
        if (!item.InUpdater)
            return;
        int order = item.UpdateOrder;
        MonoUpdater.RemoveUpdateAction(UpdateType.Update, order, item.Update);
        MonoUpdater.RemoveUpdateAction(UpdateType.FixedUpdate, order, item.FixedUpdate);
        MonoUpdater.RemoveUpdateAction(UpdateType.LateUpdate, order, item.LateUpdate);
        item.InUpdater = false;
    }
}
