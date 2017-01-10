using UnityEngine;
using System.Collections.Generic;

public class DebugStatement
{
	public DebugStatement()
	{
        this._str = "";
	}
    public void Line(string str = "")
    {
        this._str += str + "\n";
    }
    public void Commit()
    {
        Debug.Log(this._str);
    }

    private string _str;
}
