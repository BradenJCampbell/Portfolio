using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public partial class CameraCollisionHandler
{
    public class WorldCoords
    {
        public WorldCoords(Vector3 DesiredPosition, Vector3 LookAtPoint, Vector3 ClosestPoint)
        {
            this._desired_position = DesiredPosition;
            this._look_at_position = LookAtPoint;
            this._closest_position = ClosestPoint;
        }

        public Vector3 DesiredPosition
        {
            get { return this._desired_position; }
        }

        public Vector3 LookAtPoint
        {
            get { return this._look_at_position; }
        }

        public Vector3 ClosestPoint
        {
            get { return this._closest_position; }
        }

        private Vector3 _desired_position;
        private Vector3 _look_at_position;
        private Vector3 _closest_position;
    }

    public CameraCollisionHandler(Camera cam, Vector3 WorldDesiredPosition, Transform LocalSpace, Transform CameraTarget)
    {
        this._camera = cam;
        this._camera_wrap = new TransformWrapper(cam.transform);
        this._local_space = LocalSpace;
        this._look_at = CameraTarget;
        this._des = WorldDesiredPosition;
        this._needs_update = true;
        this._update();
    }

    public LayerMask LayerMask
    {
        get { return LayerMask.GetMask("Track", "Ground"); }
    }


    public Transform LookAt
    {
        set { this._look_at = value;  this._needs_update = true; }
        get { this._update(); return this._look_at; }
    }

    public LineOfSightCollision LineOfSight
    {
        get { this._update(); return this._los_collision; }
    }

    public OcclusionCollision Occlusion
    {
        get { this._update(); return this._occlusion_collision;  }
    }

    public CollisionPlane NearPlane
    {
        get { this._update(); return this._near_plane; }
    }

    public CollisionPlane FarPlane
    {
        get { this._update(); return this._far_plane; }
    }

    public bool HasCollision
    {
        get { this._update(); return this.LineOfSight.HasHit; }
        //get { this._update(); return this._all_rayhits.Length > 0 || this.LineOfSight.HasHit;  }
    }

    public WorldCoords World
    {
        get { this._update(); return this._world; }
    }

    public TransformWrapper LookAtData
    {
        get { this._update(); return new TransformWrapper(this.LookAt); }
    }
    public TransformWrapper CameraData
    {
        get { return this._camera_wrap; }
    }

    public void DebugRender()
    {
        this._debug_draw(this.NearPlane.Middle);
        this._debug_draw(this.NearPlane.TopLeft);
        this._debug_draw(this.NearPlane.TopRight);
        this._debug_draw(this.NearPlane.BottomLeft);
        this._debug_draw(this.NearPlane.BottomRight);
        this.NearPlane.DebugRender();
        StaticHelper.DebugDrawStar(this.World.DesiredPosition, (float)0.5, Color.cyan);
        StaticHelper.DebugDrawStar(this.World.ClosestPoint, (float)0.5, Color.blue);
    }

    private void _debug_draw(Vector3 pos)
    {
        RaycastHit hit;
        Vector3 dir = pos - this.World.DesiredPosition;
        if (Physics.Raycast(this.World.DesiredPosition, dir, out hit, dir.magnitude, this.LayerMask))
        {
            Debug.DrawLine(this.World.DesiredPosition, pos, Color.red);
            Debug.DrawLine(this.World.DesiredPosition, this.World.DesiredPosition + (dir * hit.distance), Color.green);
        }
        else
        {
            Debug.DrawLine(this.World.DesiredPosition, pos, Color.green);
        }
    }

    private void _update()
    {
        if (this._needs_update)
        {
            this._needs_update = false;
            this._calculate(this._des);
        }
    }

    private void _calculate(Vector3 desired_position)
    {
        Vector3 closest = desired_position;
        Vector3 look_at = this.LookAtData.World.Position;
        this._los_collision = new LineOfSightCollision(closest, look_at, this.LayerMask);
        if (this._los_collision.HasHit)
        {
            closest = this._los_collision.HitPoint;
        }
        this._occlusion_collision = new OcclusionCollision(this._camera, closest, look_at, this.LayerMask);
        this._occlusion_collision.DebugRender();
        if (this._occlusion_collision.HasHit)
        {
            closest += this._occlusion_collision.CollisionOffset;
        }
        this._near_plane = new CollisionPlane(closest, look_at, this._camera.fieldOfView, this._camera.nearClipPlane, this._camera.aspect, this.CameraData.World.Up, this.CameraData.World.Right);
        this._far_plane = new CollisionPlane(closest, look_at, this._camera.fieldOfView, this._camera.farClipPlane, this._camera.aspect, this.CameraData.World.Up, this.CameraData.World.Right);
        this._world = new WorldCoords(desired_position, look_at, closest);
    }

    /// <summary>
    /// calculates the distance from the desired camera position to the ray hit
    /// </summary>
    private float _origin_distance(Vector3 pos)
    {
        RaycastHit hit;
        if (this._raycast(pos, out hit))
        {
            return (this.World.DesiredPosition - pos).magnitude - hit.distance;
        }
        return 0;
    }

    private Ray _hit_ray(Vector3 pos)
    {
        RaycastHit hit;
        if (this._raycast(pos, out hit))
        {
            return new Ray(hit.point, pos - hit.point);
        }
        return new Ray(Vector3.zero, Vector3.zero);
    }

    private bool _closest_rayhit(out RaycastHit hit)
    {
        if (this._all_rayhits.Length <= 0)
        {
            hit = new RaycastHit();
            return false;
        }
        hit = this._all_rayhits[0];
        for (int i = 1; i < this._all_rayhits.Length; i++)
        {
            if (this._all_rayhits[i].distance > hit.distance)
            {
                hit = this._all_rayhits[i];
            }
        }
        return true;
    }

    private RaycastHit[] _all_rayhits
    {
        get
        {
            RaycastHit hit;
            List<RaycastHit> ret = new List<RaycastHit>();
            for (int i = 0; i < this.NearPlane.Corners.Length; i++)
            {
                if (this._raycast(this.NearPlane.Corners[i], out hit))
                {
                    ret.Add(hit);
                }
            }
            return ret.ToArray();
        }
    }

    /// <summary>
    /// performes a raycast from the given world position to the camera position
    /// </summary>
    private bool _raycast(Vector3 pos, out RaycastHit hit)
    {
        return Physics.Raycast(pos, this.World.DesiredPosition - pos, out hit, (this.World.DesiredPosition - pos).magnitude);
    }

    private bool _needs_update;
    private Camera _camera;
    private TransformWrapper _camera_wrap;
    private Transform _local_space;
    private Transform _look_at;
    private CollisionPlane _near_plane;
    private CollisionPlane _far_plane;
    private OcclusionCollision _occlusion_collision;
    private LineOfSightCollision _los_collision;
    private WorldCoords _world;
    private Vector3 _des;
}
