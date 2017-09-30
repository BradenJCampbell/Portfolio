using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;


public class DebugMessage
{
    public DebugMessage(object Message)
    {
        this._message = Message;
        this._frame = Time.frameCount;
        this._time = Time.fixedTime;
    }

    public object Message
    {
        get
        {
            return this._message;
        }
    }

    public int Frame
    {
        get
        {
            return this._frame;
        }
    }

    public float ElapsedTime
    {
        get
        {
            return this._time;
        }
    }

    private float _time;
    private int _frame;
    private object _message;
}

public class DebugMessages
{
    public DebugMessages(List<DebugMessage> container)
    {
        this._cont = container;
    }

    public void Clear()
    {
        this._cont.Clear();
    }

    public int Count
    {
        get
        {
            return this._cont.Count;
        }
    }

    public List<DebugMessage>.Enumerator GetEnumerator()
    {
        return this._cont.GetEnumerator();
    }

    public DebugMessage this[int index]
    {
        get
        {
            return this._cont[index];
        }
    }

    private List<DebugMessage> _cont;
}

public class DebugMessagesContainer
{
    public DebugMessagesContainer()
    {
        this._list = new List<DebugMessage>();
        this._reader = new DebugMessages(this._list);
    }

    public void Add(object Message)
    {
        this._list.Add(new DebugMessage(Message));
    }

    public DebugMessages Reader
    {
        get
        {
            return this._reader;
        }
    }

    private List<DebugMessage> _list;
    private DebugMessages _reader;
}

public class DebugStack
{
    public DebugStack()
    {
        this._funcs = new Stack<string>();
        this._indent = 0;
    }

    public void Enter(string Func)
    {
        string new_str = "";
        for (int i = 0; i < this.Indent; i++)
        {
            new_str += "  ";
        }
        new_str += Func;
        this._funcs.Push(new_str);
        this._indent++;
    }

    public void Exit()
    {
        this._indent--;
        if (this._indent < 0)
        {
            this._indent = 0;
        }
    }

    public int Indent
    {
        get
        {
            return this._indent;
        }
    }

    public string Message
    {
        get
        {
            string ret = String.Join("\n", this._funcs.ToArray());
            this._funcs.Clear();
            this._indent = 0;
            return ret;
        }
    }

    private int _indent;
    private Stack<string> _funcs;
}