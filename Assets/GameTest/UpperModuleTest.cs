using RainCoreFramework;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UpperModuleTest : ModuleTest
{
    ModuleTest moduleTest2;
    public override void Init()
    {
        base.Init();
        RCDebug.SUCCEED("UpperModuleTest初始化完成");
        GetModule(ref moduleTest2);
    }

    public override void Release()
    {
        RCDebug.SUCCEED("UpperModuleTest卸载完成");
        ReleaseModule(ref moduleTest2);
    }
}
