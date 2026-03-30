using RainCoreFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ModuleTest : Module
{
    UpperModuleTest moduleTest;

    public void ModuleTestDebug()
    {
        Debug.Log("ModuleTest被调用方法打印");
        moduleTest?.ModuleTestDebug();
    }
    public override void Init()
    {
        //if (IsInitialized)
        //{
        //    RCDebug.NOTICE("无需再次初始化");
        //    return;
        //}
        RCDebug.SUCCEED("ModuleTest初始化完成");
        GetModule(ref moduleTest);
        //GetModule(ref moduleTest2);
    }
    public override void Release()
    {
        RCDebug.SUCCEED("ModuleTest卸载完成");
        ReleaseModule(ref moduleTest);
        //ReleaseModule(ref moduleTest2);
    }
}
