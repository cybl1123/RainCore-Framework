using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RainCoreFramework
{
    public abstract class Node : MonoBehaviour, ICanUseMonoUpdater<Node>, IBelongToRainCore<Node>,INeedSelfRelease,INeedInit, ICanUseCommandModule
    {
        int _uid = int.MinValue;
        bool _isInNodeContainer = false;
        bool _isDestroyedByRainCore = false;
        UpdateSet _updateSet;
        RainContainer _container;
        bool _isInited = false;
        Action _onDestoryEvent;
        public int UID { get { return _uid; } set { _uid = value; } }
        public bool IsInNodeContainer { get { return _isInNodeContainer; } set { _isInNodeContainer = value; } }
        public bool IsActive { get { return gameObject.activeSelf; } }
        public bool IsInitialized { get { return _isInited; } }
        public bool IsDestroyedByRainCore { get { return _isDestroyedByRainCore; } set { _isDestroyedByRainCore = value; } }
        public string NodeName=>gameObject.name;
        public Action OnDestroyEvent { get { return _onDestoryEvent; } set { _onDestoryEvent = value; } }
        public UpdateSet UpdateSet 
        {
            get 
            {
                if (!_isInited)
                {
                    Init();
                }
                if (_updateSet==null)
                {
                    _updateSet=new UpdateSet();
                }
                return _updateSet; 
            } 
        }
        public RainContainer RainContainer
        { 
            get 
            {
                if (!_isInited)
                {
                    Init();
                }
                if (_container==null)
                {
                    _container=new RainContainer();
                }
                return _container; 
            }
        }

        public virtual void Init()
        {
            //UpdateSet.AddFixedUpdateAction(() => { Debug.Log(UID + ":" + "FixedUpdate更新" + ":顺序" + _updateSet.UpdateOrder); });
            //UpdateSet.AddLateUpdateAction(() => { Debug.Log(UID + ":" + "LateUpdate更新" + ":顺序" + _updateSet.UpdateOrder); });
            //UpdateSet.AddUpdateAction(() => { Debug.Log(UID + ":" + "Update更新" + ":顺序" + _updateSet.UpdateOrder); });
            if(_isInited)
                { return; }
            _isInited = true;
            this.EnableMonoUpdate();// 将自身加入MonoUpdate
            this.RegistSelfToRainCore();// 将自身加入RianCore的节点容器
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.EnableMonoUpdate();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                this.DisableMonoUpdate();
            }
        }
        public void DestorySelf()
        {
            Destroy(this.gameObject);
        }
        public void Release()
        {
            this.UnRegistSelfToRainCore();
            this.DisableMonoUpdate();
            RainContainer.ReleaseAllModule(NodeName);
        }
        public void ReleaseModule<T>() where T:Module
        {
            RainContainer.ReleaseModule<T>(NodeName);
        }
        public void OnDestroy()
        {
            _onDestoryEvent?.Invoke();
            if (!IsDestroyedByRainCore)
            {
                IsDestroyedByRainCore = true;
                Release();
            }
            RCDebug.LOG($"Node::{NodeName} 删除成功");
        }
    }

}
