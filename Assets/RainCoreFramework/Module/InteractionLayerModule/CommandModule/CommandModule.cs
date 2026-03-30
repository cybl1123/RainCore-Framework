using RainCoreFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandModule : Module
{
    Dictionary<int, Stack<Command>> _uidStackCommandDic;
    Dictionary<int, Queue<Command>> _uidQueueCommandDic;
    public void Excute(Command command)
    {
        if (command == null)
        {
            RCDebug.Null_Exception("CommandModule", "Command为空，请传入有效Command");
            return;
        }
        int uid = command.GetSubjectUID();
        if (_uidStackCommandDic.TryGetValue(uid, out var value))
        {
            if (value == null)
            {
                value = new Stack<Command>();
            }
            value.Push(command);
            RCDebug.LOG("CommandModule", $"uid:{uid}:注册Command{command.GetType()}");
        }
        else
        {
            var stack = new Stack<Command>();
            stack.Push(command);
            _uidStackCommandDic.Add(uid, stack);
        }
        if (command == null)
        {
            RCDebug.Null_Exception("CommandModule", "不可执行空方法");
            return;
        }
        command.Excute();
    }
    public bool UndoLast(bool isNoneSubjectCommand, ICanUseCommandModule item)
    {
        int uid;
        string debugSuccessInfo = "";
        string debugNullIndo = "";
        if (isNoneSubjectCommand)
        {
            uid = -2;
            debugSuccessInfo = "成功撤回Command|无主体Command";
            debugNullIndo = "无可撤回Command|无主体Command";
        }
        else
        {
            if (item == null)
            {
                uid = -1;
                debugSuccessInfo = "成功撤回Command|主体已消失，散落Command";
                debugNullIndo = "无可撤回Command|主体已消失，散落Command";
            }
            else
            {
                uid = (item as Node).UID;
                debugSuccessInfo = $"成功撤回Command|主体节点{(item as Node).NodeName}";
                debugNullIndo = "无可撤回Command|主体节点{(item as Node).NodeName}";
            }
        }
        if (_uidStackCommandDic.TryGetValue(uid, out var value))
        {
            if (value.Count == 0)
            {
                return false;
            }
            else
            {
                var stack = value.Pop();
                if (stack == null)
                {
                    RCDebug.Null_Exception("CommandModule", debugNullIndo);
                    _uidStackCommandDic.Remove(uid);
                }
                stack.Undo();
                RCDebug.LOG("CommandModule", debugSuccessInfo);
                return true;
            }
        }
        else
        {
            RCDebug.Null_Exception("CommandModule", debugNullIndo + " 或 指令主体未有记录Excute");
            return false;
        }
    }
    public void Enqueue(Command command)
    {
        if (command == null)
        {
            RCDebug.Null_Exception("CommandModule", "Command为空，请传入有效Command");
            return;
        }
        int uid = command.GetSubjectUID();
        if (_uidQueueCommandDic.TryGetValue(uid, out var value))
        {
            if (value == null)
            {
                value = new Queue<Command>();
            }
            value.Enqueue(command);
            RCDebug.LOG("CommandModule", $"uid:{uid}:入队列Command{command.GetType()}");
        }
        else
        {
            var stack = new Queue<Command>();
            stack.Enqueue(command);
            _uidQueueCommandDic.Add(uid, stack);
        }
        if (command == null)
        {
            RCDebug.Null_Exception("CommandModule", "不可入列空方法");
            return;
        }
    }
    public void ExecuteAllQueueAtOnce(bool isNoneSubjectCommand, ICanUseCommandModule item)
    {
        int uid;
        string debugInfo = "";
        if (isNoneSubjectCommand)
        {
            uid = -2;
            debugInfo = "无主体Command";
        }
        else
        {
            if (item == null)
            {
                uid = -1;
                debugInfo = "主体缺失Command";
            }
            else
            {
                uid = (item as Node).UID;
                debugInfo = $"绑定Node uid:{uid}";
            }
        }
        if (_uidQueueCommandDic.TryGetValue(uid, out var queue))
        {
            if (queue.Count == 0)
            {
                RCDebug.LOG("CommandModule", $"可撤回命令队列为空|{debugInfo}");
            }
            else
            {
                while (queue.Count != 0)
                {
                    var command = queue.Dequeue();
                    Excute(command);
                    RCDebug.LOG("CommandModule", $"执行命令{command.GetType()}|{debugInfo}");
                }
                if (queue.Count == 0)
                {
                    _uidQueueCommandDic.Remove(uid);
                }
                RCDebug.LOG("CommandModule", $"已执行全部命令队列|{debugInfo}|{debugInfo}");
            }
        }
        else if (!_uidQueueCommandDic.ContainsKey(uid))
        {
            RCDebug.LOG("CommandModule", $"节点uid:{uid} 无可撤回命令|{debugInfo}");
        }
    }
    public bool ExecuteQueueOnce(bool isNoneSubjectCommand, ICanUseCommandModule item)
    {
        int uid;
        string debugInfo = "";
        if (isNoneSubjectCommand)
        {
            uid = -2;
            debugInfo = "无主体Command";
        }
        else
        {
            if (item == null)
            {
                uid = -1;
                debugInfo = "主体缺失Command";
            }
            else
            {
                uid = (item as Node).UID;
                debugInfo = $"绑定Node uid:{uid}";
            }
        }
        if (_uidQueueCommandDic.TryGetValue(uid, out var queue))
        {
            if (queue.Count == 0)
            {
                RCDebug.LOG("CommandModule", $"可撤回命令队列为空|{debugInfo}");
                return false;
            }
            else
            {
                var command = queue.Dequeue();
                Excute(command);
                if (queue.Count == 0)
                {
                    _uidQueueCommandDic.Remove(uid);
                }
                RCDebug.LOG("CommandModule", $"执行命令{command.GetType()}|{debugInfo}");
                RCDebug.LOG("CommandModule", $"已执行全部命令队列|{debugInfo}|{debugInfo}");
                return true;
            }
        }
        else if (!_uidQueueCommandDic.ContainsKey(uid))
        {
            RCDebug.LOG("CommandModule", $"节点uid:{uid} 无可撤回命令|{debugInfo}");
            return false;
        }
        return false;
    }
    public void ClearCommandQueue(bool isNoneSubjectCommand, ICanUseCommandModule item)
    {
        int uid;
        string debugInfo = "";
        if (isNoneSubjectCommand)
        {
            uid = -2;
            debugInfo = "无主体Command";
        }
        else
        {
            if (item == null)
            {
                uid = -1;
                debugInfo = "主体缺失Command";
            }
            else
            {
                uid = (item as Node).UID;
                debugInfo = $"绑定Node uid:{uid}";
            }
        }
        if (_uidQueueCommandDic.ContainsKey(uid))
        {
            _uidQueueCommandDic[uid].Clear();
            _uidQueueCommandDic.Remove(uid);
            RCDebug.LOG("CommandModule", $"Command队列已清空|{debugInfo}");
        }
        else
        {
            RCDebug.LOG("CommandModule",$"无可清除Command队列|{debugInfo}");
        }
    }
    public void ClearUndoStack(bool isNoneSubjectCommand, ICanUseCommandModule item)
    {
        int uid;
        string debugInfo = "";
        if (isNoneSubjectCommand)
        {
            uid = -2;
            debugInfo = "无主体Command";
        }
        else
        {
            if (item == null)
            {
                uid = -1;
                debugInfo = "主体缺失Command";
            }
            else
            {
                uid = (item as Node).UID;
                debugInfo = $"绑定Node uid:{uid}";
            }
        }
        if (_uidStackCommandDic.ContainsKey(uid))
        {
            _uidStackCommandDic[uid].Clear();
            _uidStackCommandDic.Remove(uid);
            RCDebug.LOG("CommandModule", $"可撤回Command栈已清空|{debugInfo}");
        }
        else
        {
            RCDebug.LOG("CommandModule", $"无可撤回Command栈|{debugInfo}");
        }
    }
    public void ClearAll()
    {
        _uidStackCommandDic.Clear();
        _uidQueueCommandDic.Clear();
        RCDebug.LOG("CommandModule", "撤回栈与命令队列已清除");
    }
    public override void Init()
    {
        _uidStackCommandDic = new Dictionary<int, Stack<Command>>();
        _uidQueueCommandDic = new Dictionary<int, Queue<Command>>();
    }
    public override void Release()
    {
        _uidStackCommandDic.Clear();
        _uidQueueCommandDic.Clear();
        _uidStackCommandDic = null;
        _uidStackCommandDic = null;
    }
}
public interface ICanUseCommandModule
{
}
public abstract class Command
{
    ICanUseCommandModule _commandSubject;
    bool _isNodeSubjectCommand;
    Action _excuteAction;
    Action _undoAction;
    public Command(bool isNoneSubjectCommand, ICanUseCommandModule commandSubject = null, Action ExcuteAction = null, Action UndoAction = null)
    {
        if (commandSubject == null && !isNoneSubjectCommand)
        {
            RCDebug.Null_Exception("绑定Node为空");
            return;
        }
        _commandSubject = commandSubject;
        _isNodeSubjectCommand = isNoneSubjectCommand;
        _excuteAction = ExcuteAction;
        _undoAction = UndoAction;
    }
    public int GetSubjectUID()
    {
        if (_commandSubject == null && !_isNodeSubjectCommand)
        {
            RCDebug.Null_Exception("Command绑定节点不存在");
            return -1;
        }
        else if (_isNodeSubjectCommand)
        {
            return -2;
        }
        else
        {
            return (_commandSubject as Node).UID;
        }
    }
    public virtual void Excute()
    {
        _excuteAction?.Invoke();
    }
    public virtual void Undo()
    {
        _undoAction?.Invoke();
    }
}

