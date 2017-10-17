using BCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class CurlSpinnerBehaviour : MonoBehaviour
{
    public struct Piece
    {
        public float Mass;

        public int ID
        {
            get
            {
                return this.Transform.GetInstanceID();
            }
        }

        public CurlSpinnerBehaviour Spinner
        {
            get
            {
                return this._set.Spinner;
            }
        }

        public bool Active
        {
            get
            {
                return this.Transform.gameObject.activeSelf;
            }
            set
            {
                this.Transform.gameObject.SetActive(value);
            }
        }

        public Transform Transform
        {
            get
            {
                return this._transform;
            }
        }

        public SmartVector Position
        {
            get
            {
                return SmartVector.CreateLocalPoint(this._transform.localPosition.x, this._transform.localPosition.y, this._transform.localPosition.z, this._transform.parent);
            }
        }

        public Piece(PieceSet Set, float Mass, Transform Template)
        {
            this._set = Set;
            this.Mass = Mass;
            this._transform = UnityEngine.Object.Instantiate(Template);
            this.Transform.gameObject.SetActive(true);
        }

        public Piece(PieceSet Set, float Mass, Transform Template, SmartVector Start, SmartVector End, float width) : this(Set, Mass, Template)
        {
            GameEngine.Place(this.Transform, Start, End, width, 1);
            GameEngine.Debug.Log(GameEngine.GameWorld.Bounds + " Spinner.Piece[" + this.ID + "] " + this.Position);
            this.Transform.parent = this.Spinner.Body;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this.Transform.gameObject);
        }

        private PieceSet _set;
        private Transform _transform;
    }

    public class PieceSet
    {
        public CurlSpinnerBehaviour Spinner
        {
            get
            {
                return this._parent;
            }
        }

        public Piece this[int index]
        {
            get
            {
                return this._pieces[index];
            }
        }

        public int Count
        {
            get
            {
                return this._pieces.Count;
            }
        }

        public PieceSet(CurlSpinnerBehaviour Parent, Rigidbody Body)
        {
            this._body = Body;
            this._parent = Parent;
            this._pieces = new List<CurlSpinnerBehaviour.Piece>();
        }

        public bool Add(float Mass, Transform Template, SmartVector End, float width)
        {
            SmartVector add_point;
            if (this.Spinner.ClosestPoint(End, out add_point))
            {
                return this.Add(Mass, Template, add_point, End, 2);
            }
            return false;
        }

        public bool Add(float Mass, Transform Template, SmartVector Start, SmartVector End, float width)
        {
            Piece newPiece = new Piece(this, Mass, Template, Start, End, width);
            this._body.mass += Mass;
            this._pieces.Add(newPiece);
            return true;
        }

        public bool Add(float Mass, Transform Template)
        {
            Piece newPiece = new Piece(this, Mass, Template);
            this._body.mass += Mass;
            this._pieces.Add(newPiece);
            return true;
        }

        public bool Remove(int index)
        {
            return this.Remove(this._pieces[index]);
        }

        protected bool Remove(Piece p)
        {
            if (this._pieces.Contains(p))
            {
                this._body.mass -= p.Mass;
                p.Destroy();
                return this._pieces.Remove(p);
            }
            return true;
        }

        public void Clear()
        {
            this._pieces.Clear();
        }

        private Rigidbody _body;
        private CurlSpinnerBehaviour _parent;
        private List<CurlSpinnerBehaviour.Piece> _pieces;
    }
}