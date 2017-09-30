using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCUnity
{
    public struct Timer
    {

        public float ElapsedTime
        {
            get
            {
                this._tick();
                return this._elapsed_time;
            }
        }

        public bool IsRunning
        {
            get
            {
                return this._last_time > 0;
            }
        }

        public void Reset()
        {
            this._last_time = 0;
            this._elapsed_time = 0;
        }

        public void Start()
        {
            if (!this.IsRunning)
            {
                this._last_time = UnityEngine.Time.fixedTime;
            }
        }

        public void Pause()
        {
            this._tick();
            this._last_time = 0;
        }

        private void _tick()
        {
            if (this.IsRunning)
            {
                this._elapsed_time += UnityEngine.Time.fixedTime - this._last_time;
                this._last_time = UnityEngine.Time.fixedTime;
            }
        }

        private float _last_time;
        private float _elapsed_time;
    }
}