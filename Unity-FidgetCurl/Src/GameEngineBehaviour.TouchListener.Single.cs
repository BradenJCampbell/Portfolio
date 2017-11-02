using BCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class GameEngineBehaviour
{
    public partial class TouchListener
    {
        public class SingleTouchListener
        {
            delegate void StartEvent(SingleTouchListener instance);
            delegate void DragEvent(SingleTouchListener instance);
            delegate void EndEvent(SingleTouchListener instance);

            public SingleTouchListener(GameEngineBehaviour Parent)
            {
                this._parent = Parent;
                this._path = new PathTracker();
                this.State = TouchState.None;
                this._last_frame = -1;
            }

            public bool InState(params TouchState[] states)
            {
                this.Update();
                foreach (TouchState ts in states)
                {
                    if (this.State == ts)
                    {
                        return true;
                    }
                }
                return false;
            }

            public void Update()
            {
                if (this._last_frame < UnityEngine.Time.frameCount)
                {
                    this._last_frame = UnityEngine.Time.frameCount;
                    // if a single touch or a mouse left-click happens
                    if (Input.touchCount == 1 || Input.GetMouseButton(0))
                    {
                        float x = 0;
                        float y = 0;
                        if (Input.touchCount == 1)
                        {
                            x = Input.touches[0].position.x;
                            y = Input.touches[0].position.y;
                        }
                        else
                        {
                            x = Input.mousePosition.x;
                            y = Input.mousePosition.y;
                        }
                        SmartVector world_touch;
                        if (GameEngine.GameCamera.ScreenToWorldPosition(x, y, out world_touch))
                        {
                            this._path.Capture(world_touch);
                        }
                        switch (this.State)
                        {
                            case TouchState.None:
                            case TouchState.End:
                                this.State = TouchState.Start;
                                break;
                            case TouchState.Start:
                                this.State = TouchState.Touch;
                                break;
                        }
                    }
                    //  no touch, but touch previous frame
                    else if (this.InState(TouchState.Start, TouchState.Touch))
                    {
                        this.State = TouchState.End;
                    }
                    else
                    {
                        this._path.Clear();
                        this.State = TouchState.None;
                    }
                }
            }

            public TouchState State
            {
                protected set
                {
                    this._state = value;
                }
                get
                {
                    this.Update();
                    return this._state;
                }
            }

            public bool Moved
            {
                get
                {
                    return this._path.Moved;
                }
            }

            public TouchMovement LastMovement
            {
                get
                {
                    this.Update();
                    return (TouchMovement)this._path.LastMovement;
                }
            }

            public TouchMovement CompoundMovement(float DegreesOfFreedom)
            {
                this.Update();
                return (TouchMovement)this._path.CompoundMovement(DegreesOfFreedom);
            }

            private GameEngineBehaviour _parent;
            private PathTracker _path;
            private TouchState _state;
            private int _last_frame;
        }

    }
}
