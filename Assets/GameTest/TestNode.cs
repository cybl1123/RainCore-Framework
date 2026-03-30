using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RainCoreFramework;
using TMPro;
public class TestNode : Node
{
    [SerializeField] string Name;
    public  TMP_Text tM;
    public void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            int index = i;
            RainContainer.GetModule<CommandModule>().Enqueue(new TestCommand(this, () => { Debug.Log($"执行测试::{index}"); }, () => { Debug.Log($"Undo测试::{index}"); }));
        }
    }
}
class TestCommand : Command
{
    public TestCommand(Node node,Action action1,Action action2) : base(false,node, action1,action2)
    {
        Debug.Log("添加一次");
    }
}
