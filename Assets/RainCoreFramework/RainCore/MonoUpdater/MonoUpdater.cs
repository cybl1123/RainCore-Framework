using System;
using System.Collections.Generic;
using UnityEngine;
namespace RainCoreFramework
{
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
        List<int> _existOrder;
        Dictionary<int, Action> _fixedUpdateActionDic;
        Dictionary<int, Action> _updateActionDic;
        Dictionary<int, Action> _lateUpdateActionDic;
        static bool _isDestoryed = false;
        #region 属性封装器
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
                if (_existOrder == null)
                    _existOrder = new List<int>();
                return _existOrder;
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
                if (_isDestoryed)
                {
                    RCDebug.NOTICE("[处于场景关闭阶段，MONOUPDATER已关闭，无法访问]");
                    return null;
                }
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
            if (action == null)
            {
                RCDebug.NOTICE("Action为空");
                return;
            }
            if (instance == null)
                return;
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
            if (action == null)
                return;
            if (instance == null)
                return;
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
            _isDestoryed = false;
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void Update()
        {
            for (int i = 0; i < ExistOrder.Count; i++)
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
        private void OnDestroy()
        {
            _isDestoryed = true;
            _orderCounter.Clear();
            _existOrder.Clear();
            _fixedUpdateActionDic.Clear();
            _updateActionDic.Clear();
            _lateUpdateActionDic.Clear();
            RCDebug.NOTICE("[MONOUPDATER 已关闭]");
        }
    }
    public interface ICanUseMonoUpdater<T> where T : ICanUseMonoUpdater<T>
    {
        UpdateSet UpdateSet { get; }

    }
    static class MonoUpdaterExtension
    {
        public static void EnableMonoUpdate(this ICanUseMonoUpdater<Node> item)
        {
            if (item.UpdateSet.InUpdater)
                return;
            int order = item.UpdateSet.UpdateOrder;
            MonoUpdater.AddUpdateAction(UpdateType.Update, order, item.UpdateSet.Update);
            MonoUpdater.AddUpdateAction(UpdateType.FixedUpdate, order, item.UpdateSet.FixedUpdate);
            MonoUpdater.AddUpdateAction(UpdateType.LateUpdate, order, item.UpdateSet.LateUpdate);
            item.UpdateSet.SetInUpdater(true);
        }
        public static void DisableMonoUpdate(this ICanUseMonoUpdater<Node> item)
        {
            if (!item.UpdateSet.InUpdater)
                return;
            int order = item.UpdateSet.UpdateOrder;
            MonoUpdater.RemoveUpdateAction(UpdateType.Update, order, item.UpdateSet.Update);
            MonoUpdater.RemoveUpdateAction(UpdateType.FixedUpdate, order, item.UpdateSet.FixedUpdate);
            MonoUpdater.RemoveUpdateAction(UpdateType.LateUpdate, order, item.UpdateSet.LateUpdate);
            item.UpdateSet.SetInUpdater(false);
        }
    }
    /// <summary>
    /// 此处有GC，是否能优化
    /// </summary>
    public class UpdateSet
    {
        int _updateOrder = 0;
        bool _inUpdater = false;
        Action _fixedUpdate;
        Action _update;
        Action _lateUpdate;
        public UpdateSet()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fixedUpdateAction">FixedUpdate物理帧更新</param>
        /// <param name="updateAction">Update帧更新</param>
        /// <param name="lateUpdateAction">LateUpdate后帧更新</param>
        public UpdateSet(Action fixedUpdateAction, Action updateAction, Action lateUpdateAction)
        {
            _fixedUpdate += fixedUpdateAction;
            _update += updateAction;
            _lateUpdate += lateUpdateAction;
        }
        public int UpdateOrder
        {
            get => _updateOrder;
        }
        public bool InUpdater
        {
            get => _inUpdater;
        }

        public Action FixedUpdate { get => _fixedUpdate; }
        public Action Update { get => _update; }
        public Action LateUpdate { get => _lateUpdate; }
        public void SetUpdateOrder(int value)
        {
            _updateOrder = value;
        }
        public void SetInUpdater(bool value)
        {
            _inUpdater = value;
        }
        public void AddUpdateAction(Action action)
        {
            _update += action;
        }
        public void AddFixedUpdateAction(Action action)
        {
            _fixedUpdate += action;
        }
        public void AddLateUpdateAction(Action action)
        {
            _lateUpdate += action;
        }
        public void RemoveUpdateAction(Action action)
        {
            _update -= action;
        }
        public void RemoveFixedUpdateAction(Action action)
        {
            _fixedUpdate -= action;
        }
        public void RemoveLateUpdateAction(Action action)
        {
            _lateUpdate -= action;
        }
    }
}