using BCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

partial class GameEngineWorld
{
    public GameEngineWorldBounds Bounds
    {
        get
        {
            return new GameEngineWorldBounds(this);
        }
    }

    public struct GameEngineWorldBounds
    {
        public GameEngineWorldBounds(GameEngineWorld World)
        {
            this._world = World;
        }

        public GameEngineWorld World
        {
            get
            {
                return this._world;
            }
        }

        public SmartVector TopLeft
        {
            get
            {
                return SmartVector.CreateWorldPoint(this.Left, this.Top, 0);
            }
        }

        public SmartVector TopRight
        {
            get
            {
                return SmartVector.CreateWorldPoint(this.Right, this.Top, 0);
            }
        }

        public SmartVector BottomLeft
        {
            get
            {
                return SmartVector.CreateWorldPoint(this.Left, this.Bottom, 0);
            }
        }

        public SmartVector BottomRight
        {
            get
            {
                return SmartVector.CreateWorldPoint(this.Right, this.Bottom, 0);
            }
        }

        public float Left
        {
            get
            {
                return -250;
            }
        }

        public float Right
        {
            get
            {
                return 250;
            }
        }

        public float Top
        {
            get
            {
                return 250;
            }
        }

        public float Bottom
        {
            get
            {
                return -250;
            }
        }

        public override string ToString()
        {
            return this.BottomLeft + " => " + this.TopRight;
        }

        private GameEngineWorld _world;
    }

}

