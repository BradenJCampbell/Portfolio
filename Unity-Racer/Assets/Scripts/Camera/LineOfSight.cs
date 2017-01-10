using UnityEngine;
using System.Collections;

public partial class CameraCollisionHandler
{
    public class LineOfSightCollision
    {
        public LineOfSightCollision(Vector3 CameraPosition, Vector3 TargetPosition, LayerMask Mask)
        {
            this._has_hit = Physics.Raycast(TargetPosition, CameraPosition - TargetPosition, out this._hit, (CameraPosition - TargetPosition).magnitude, Mask);
        }
        public bool HasHit
        {
            get { return this._has_hit; }
        }
        public Vector3 HitPoint
        {
            get { return this._hit.point; }
        }
        public Vector3 ClosestPointOnColliderBounds(Vector3 point)
        {
            return this._hit.collider.ClosestPointOnBounds(point);
        }
        private RaycastHit _hit;
        private bool _has_hit;
    }
}