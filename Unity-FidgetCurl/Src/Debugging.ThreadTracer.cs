using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

public class ThreadTracer
{
    private class TracerMechanic
    {
        public TracerMechanic(Thread target)
        {
            this._target = target;
            this._last_trace = new StackTrace(this._target, true);
            this._changed = true;
        }

        public void Update()
        {
            StackTrace new_trace = new StackTrace(this._target, true);
            if (this._trace_changed(new_trace))
            {
                this._last_trace = new_trace;
                this._changed = true;
            }
        }

        public string LastTrace
        {
            get
            {
                this._changed = false;
                return this._last_trace.ToString();
            }
        }

        public bool HasChanged
        {
            get
            {
                return this._changed;
            }
        }

        private bool _trace_changed(StackTrace new_trace = null)
        {
            if (this._last_trace == null)
            {
                return true;
            }
            if (new_trace == null)
            {
                new_trace = new StackTrace(this._target, true);
            }
            if (new_trace.FrameCount != this._last_trace.FrameCount)
            {
                return true;
            }
            for (int i = 0; i < new_trace.FrameCount; i++)
            {
                if (this._last_trace.GetFrame(i).ToString() != new_trace.GetFrame(i).ToString())
                {
                    return true;
                }
            }
            return false;
        }

        private Thread _target;
        private StackTrace _last_trace;
        private bool _changed;
    }

    public static float IntervalSeconds = (float)0.2;

    public static bool TrackerThread(string DumpFilePath)
    {
        if (_tracer_thread == null)
        {
            _tracer_file = DumpFilePath;
            _tracer_thread = new Thread(_trace_thread);
            _tracer_thread.Start();
            return true;
        }
        return false;
    }

    public static void AddTarget(Thread target)
    {
        if (_threads == null)
        {
            _threads = new Dictionary<int, TracerMechanic>();
        }
        if (!_threads.ContainsKey(target.ManagedThreadId))
        {
            _threads.Add(target.ManagedThreadId, new TracerMechanic(target));
        }
    }

    private static void _trace_thread()
    {
        int interval_ms = (int)(1000 * IntervalSeconds);
        while (true)
        {
            foreach (TracerMechanic t in _threads.Values)
            {
                t.Update();
                if (t.HasChanged)
                {
                    File.WriteAllText(_tracer_file, t.LastTrace);
                }
            }
            Thread.Sleep(interval_ms);
        }
    }

    private static Dictionary<int, TracerMechanic> _threads;
    private static Thread _tracer_thread;
    private static string _tracer_file;
}
