using UnityEngine;
using System.Collections;

public partial class CameraCollisionHandler
{
    public class OcclusionCollision
    {
        public OcclusionCollision(Camera cam, Vector3 DesiredCameraPosition, Vector3 WorldLookAtPoint, LayerMask CollisionLayersMask)
        {
            this._desired_pos = DesiredCameraPosition;
            this._offset = Vector3.zero;
            TransformWrapper cam_data = new TransformWrapper(cam.transform);
            this._plane = new CollisionPlane(DesiredCameraPosition, WorldLookAtPoint, cam.fieldOfView, cam.nearClipPlane, cam.aspect, cam_data.World.Up, cam_data.World.Right);
            this._has_hit = false;
            RaycastHit hit;
            for (int i = 0; i < this._plane.Corners.Length; i++)
            {
                if (StaticHelper.RaycastPoint(out hit, DesiredCameraPosition, this._plane.Corners[i], CollisionLayersMask))
                {
                    if (!this._has_hit || hit.distance < this._hit.distance)
                    {
                        this._hit = hit;
                        this._hit_corner = this._plane.Corners[i];
                        this._has_hit = true;
                    }
                }
            }
            if (this._has_hit)
            {
                //  this is a dirty calculation, as we assume that the desired position is "outside" the collider
                this._collider_closest = this._hit.collider.ClosestPointOnBounds(this._plane.Middle);
                this._bounds_closest = this._plane.ProjectToBounds(this._collider_closest);
                this._offset = this._collider_closest - this._bounds_closest;
            }
        }

        public void DebugRender()
        {
            if (this._has_hit)
            {
                StaticHelper.DebugDrawStar(this._bounds_closest, (float)0.5, Color.blue);
                StaticHelper.DebugDrawStar(this._collider_closest, (float)0.5, Color.black);
            }
        }

        public bool HasHit
        {
            get { return this._has_hit; }
        }

        public Vector3 CollisionOffset
        {
            get { return this._offset; }
        }

        public GameObject HitObject
        {
            get { return this._hit.collider.gameObject; }
        }

        private Vector3 _desired_pos;
        private bool _has_hit;
        private RaycastHit _hit;
        private Vector3 _hit_corner;
        private CollisionPlane _plane;
        private Vector3 _offset;
        private Vector3 _collider_closest;
        private Vector3 _bounds_closest;
    }
}
