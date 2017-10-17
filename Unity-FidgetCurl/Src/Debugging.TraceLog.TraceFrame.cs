using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

public partial class TraceLog
{
    private struct TraceLogFrame
    {
        public int Sequence;
        public float TimeCaptured;
        public string Note;
        public StackTrace Trace;

        public string TimeString
        {
            get
            {
                int time_whole = UnityEngine.Mathf.FloorToInt(this.TimeCaptured);
                int time_decimal = UnityEngine.Mathf.CeilToInt((this.TimeCaptured - time_whole) * 1000);
                return time_whole.ToString().PadLeft(3, '0') + "." + time_decimal.ToString().PadRight(3, '0');

            }
        }

        public int Count
        {
            get
            {
                if (this.Trace == null)
                {
                    return 0;
                }
                return this.Trace.FrameCount;
            }
        }

        public TraceLogFrame(int Sequence, float TimeCaptured, string Note, StackTrace Trace)
        {
            this.Sequence = Sequence;
            this.TimeCaptured = TimeCaptured;
            this.Note = Note;
            this.Trace = Trace;
        }

        public StackFrame Frame(int index)
        {
            return this.Trace.GetFrame(this.Trace.FrameCount - 1 - index);
        }

        public string FrameString(int index)
        {
            try
            {
                StackFrame frame = this.Frame(index);
                string file_name = frame.GetFileName();
                int file_line = frame.GetFileLineNumber();
                int file_column = frame.GetFileColumnNumber();
                string method_name = frame.GetMethod().DeclaringType.Name.Trim() + "." + frame.GetMethod().Name.Trim();
                List<string> param_names = new List<string>();
                for (int i = 0; i < frame.GetMethod().GetParameters().Length; i++)
                {
                    param_names.Add(frame.GetMethod().GetParameters()[i].ParameterType.Name + " " + frame.GetMethod().GetParameters()[i].Name);
                }
                method_name = method_name + "(" + String.Join(", ", param_names.ToArray()) + ")";
                if (file_name == null || file_name.Length < 1 || file_line < 1 || file_column < 1)
                {
                    return method_name;
                }
                return file_name + " at " + file_line + ":" + file_column + " " + method_name;
            }
            catch (Exception ex)
            {
                return "<UNKNOWN>";
            }
        }

        public bool WriteFrame(StreamWriter writer, int index, bool include_count = false)
        {
            if (writer == null)
            {
                return false;
            }
            try
            {
                if (include_count)
                {
                    writer.Write(this.Sequence.ToString().PadLeft(6, '0'));
                }
                else
                {
                    writer.Write("      ");
                }
                writer.Write(" " + this.TimeString + " " + this.Note + " ");
                for (int i = 0; i < index; i++)
                {
                    writer.Write("| ");
                }
                writer.Write(this.FrameString(index));
                writer.WriteLine();
                return true;
            }
            catch (Exception ex)
            {
                GameEngine.Debug.Log(ex);
                return false;
            }
        }
    }
}