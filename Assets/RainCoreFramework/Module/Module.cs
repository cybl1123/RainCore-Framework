using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RainCoreFramework
{
    public abstract class Module:ICanGetModule<Module>,INeedInit,INeedSelfRelease
    {
        int _dependencyParams = 0;
        public int DependencyParams
        {
            get { return _dependencyParams; }
            set { _dependencyParams = value; }
        }
        public bool _isInited = false;
        public bool IsInitialized { get { return _isInited; } protected set { _isInited = value; } }
        protected Module(){}
        /// <summary>
        /// 初始化方法，用于重写
        /// </summary>
        public abstract void Init();
        /// <summary>
        /// 释放方法
        /// </summary>
        public abstract void Release();

        public Module GetModule<T>(ref T moduleReference) where T : Module
        {
            if (this.GetType().IsAssignableFrom(typeof(T))||this.GetType().IsSubclassOf(typeof(T))||this.GetType().Equals(typeof(T)))
            {
                RCDebug.WARN("有继承关系的模块间依赖已被关闭，如有编译需求请在全局设置中开启");
                return moduleReference;
            }
            if (moduleReference != null)
            {
                RCDebug.NOTICE("无需重复获取依赖模块");
                return moduleReference;
            }
            else
            {
                var module = this.RequestModuleFromRainCore<T>() as T;
                if (module != null)
                {
                    RCDebug.LOG($"模块::{typeof(T)} 被获取完成");
                    moduleReference = module;
                }
                return moduleReference;
            }
        }
        public void ReleaseModule<T>(ref T moduleReference) where T: Module
        {
            if (moduleReference == null)
            {
                RCDebug.NOTICE("无需重复释放依赖模块");
                return;
            }
            moduleReference = null;
            this.ReleaseModuleFromRainCore<T>();
        }
    }

}
