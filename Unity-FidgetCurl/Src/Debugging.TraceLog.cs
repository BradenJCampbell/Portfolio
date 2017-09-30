using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

public partial class TraceLog
{
    public int NoteSize = 400;

    public bool SpawnThread = false;

    public bool Enabled
    {
        set
        {
            this._enabled = value;
        }
        get
        {
            return this._enabled && this.writer != null;
        }
    }

    public TraceLog(string FilePath)
    {
        this._init();
        this._filename = FilePath;
    }

    public TraceLog(Stream OutputStream) : this(new StreamWriter(OutputStream))
    {
        
    }

    public TraceLog(StreamWriter OutputStreamWriter)
    {
        this._init();
        this._output_stream = OutputStreamWriter;
    }

    private void _init()
    {
        this._lock = new Object();
        this._traces = new List<TraceLogFrame>();
        this._traces.Add(new TraceLogFrame(0, 0, "", null));
        this._count = 1;
    }

    public bool Update(string Note = "")
    {
        if (this.Enabled)
        {
            bool ret = false;
            lock (this._lock)
            {
                try
                {
                    if (this.SpawnThread && this._listener == null)
                    {
                        this._listener = new Thread(ListenerThread);
                        this._listener.Start();
                    }
                    if (Note.Length > this.NoteSize)
                    {
                        Note = Note.Substring(0, this.NoteSize);
                    }
                    if (Note.Length < this.NoteSize)
                    {
                        Note = Note.PadRight(this.NoteSize, ' ');
                    }
                    //  get current trace
                    this._traces.Add(new TraceLogFrame(this._count, UnityEngine.Time.fixedTime, Note, new StackTrace(1)));
                    this._count++;
                    if (!this.SpawnThread)
                    {
                        this._tick();
                    }
                    ret = true;
                }
                catch (Exception ex)
                {
                    GameEngine.DebugLog(ex);
                    ret = false;
                }
            }
            return ret;
        }
        return true;
    }

    public void Close()
    {
        this._enabled = false;
        if (this._output_stream != null)
        {
            this._output_stream.Flush();
            this._output_stream.Close();
        }
    }

    protected void ListenerThread()
    {
        while(this._enabled)
        {
            this._tick();
        }
    }

    protected StreamWriter writer
    {
        get
        {
            try
            {
                //  open file if not opened
                if (this._output_stream == null)
                {
                    if (File.Exists(this._filename))
                    {
                        File.Delete(this._filename);
                    }
                    this._output_stream = new StreamWriter(File.Open(this._filename, FileMode.Create));
                }
                return this._output_stream;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    private void _tick()
    {
        while (this._traces.Count > 1)
        {
            TraceLogFrame prev = this._traces[0];
            TraceLogFrame curr = this._traces[1];
            bool different = (prev.Count < 1);
            bool needs_count = true;
            for (int i = 0; i < curr.Count; i++)
            {
                different = different || i >= prev.Count || prev.Sequence != curr.Sequence;
                if (different)
                {
                    curr.WriteFrame(this.writer, i, needs_count);
                    needs_count = false;
                }
            }
            if (!different)
            {
                curr.WriteFrame(this.writer, curr.Count - 1, true);
            }
            this._traces.RemoveAt(0);
        }
    }

    private bool _enabled;
    private int _count;
    private Object _lock;
    private List<TraceLogFrame> _traces;
    private string _filename;
    private StreamWriter _output_stream;
    private Thread _listener;
}