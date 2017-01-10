using UnityEngine;
using System.Collections;

public partial class TrackBehaviour : MonoBehaviour
{
    public class TrackRenderer
    {
        public TrackRenderer(TrackBehaviour Behaviour)
        {
            this._behaviour = Behaviour;
        }

        public bool RenderPath(TrackPath Path)
        {
            for (int i = 0; i < Path.Road.Length; i++)
            {
                this.PlaceRoad(Path.Road[i].start, Path.Road[i].end, Path.RoadWidth);
            }
            for (int i = 0; i < Path.Rails.Length; i++)
            {
                this.PlaceFence(Path.Rails[i].start, Path.Rails[i].end);
                this.PlacePost(Path.Rails[i].start);
            }
            return true;
        }

        public bool PlaceRoad(Vector3 start, Vector3 end, float width)
        {
            Vector3 diff = end - start;
            if (diff == Vector3.zero)
            {
                Debug.Log("zero diff at " + start);
            }
            Transform newRoad = MonoBehaviour.Instantiate(this._behaviour.Road);
            newRoad.parent = this._behaviour.transform;
            newRoad.localPosition = start + (diff / 2);
            newRoad.rotation = Quaternion.LookRotation(diff);
            newRoad.localScale = new Vector3(width, 1, diff.magnitude);
            return true;
        }

        public bool PlaceFence(Vector3 start, Vector3 end)
        {
            Vector3 diff = end - start;
            Transform newFence = MonoBehaviour.Instantiate(this._behaviour.Fence);
            newFence.parent = this._behaviour.transform;
            newFence.localPosition = start + (diff / 2);
            newFence.rotation = Quaternion.LookRotation(diff);
            newFence.localScale = new Vector3(newFence.localScale.x, newFence.localScale.y, diff.magnitude);
            return true;
        }

        public bool PlacePost(Vector3 pos)
        {
            Transform newPost = MonoBehaviour.Instantiate(this._behaviour.Post);
            newPost.parent = this._behaviour.transform;
            newPost.localPosition = pos;
            return true;
        }


        /// <summary>
        /// Assumption: the angle from start to end is less than 180
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="start">relative to specified pivot</param>
        /// <param name="end">relative to specified pivot</param>
        /// <param name="TrackWidth"></param>
        /// <param name="SectionLength"></param>
        /// <returns>true, if the section was deployed</returns>
        public bool CurvedRail(Vector3 pivot, Vector3 start, Vector3 end, float RailLength)
        {
            //  calculate the curved distance between points
            float theta = Vector3.Angle(start, end);
            float circum = 2 * Mathf.PI * start.magnitude;
            float dist = circum * theta / 360;
            //  calculate the number of sections
            float count = Mathf.CeilToInt(dist / RailLength);
            //  calculate the change per increment
            Vector3 increment = (end - start) / (count);
            this.PlacePost(pivot + start);
            Vector3 curr = pivot + start;
            for (int i = 1; i <= count; i++)
            {
                Vector3 next = pivot + (start + (i * increment)).normalized * start.magnitude;
                this.PlaceFence(curr, next);
                this.PlacePost(next);
                curr = next;
            }
            return true;
        }


        /// <summary>
        /// Assumption: the angle from start to end is less than 180
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="start">relative to specified pivot</param>
        /// <param name="end">relative to specified pivot</param>
        /// <param name="TrackWidth"></param>
        /// <param name="RoadWidth"></param>
        /// <returns>true, if the section was deployed</returns>
        public bool CurvedRoad(Vector3 pivot, Vector3 start, Vector3 end, float RoadWidth)
        {
            //  calculate the curved distance between points for the outmost part of the road
            float theta = Vector3.Angle(start, end);
            float circum = 2 * Mathf.PI * (start.magnitude + (RoadWidth / 2));
            float dist = circum * theta / 360;
            //  calculate the number of sections
            float count = Mathf.CeilToInt(dist);
            //  calculate the change per increment
            Vector3 increment = (end - start) / (count);
            Vector3 outer_increment = (StaticHelper.BuildVector(end, end.magnitude + (RoadWidth / 2)) - StaticHelper.BuildVector(start, start.magnitude + (RoadWidth / 2))) / count;
            Vector3 edging = ((outer_increment - increment) / 2);
            Vector3 curr_start = start;
            Vector3 curr_end = start + outer_increment;
            for (int i = 0; i < count; i++)
            {
                curr_start = StaticHelper.BuildVector(start - edging + (i * increment), start.magnitude);
                curr_end = StaticHelper.BuildVector(start + edging + outer_increment + (i * increment), start.magnitude);
                this.PlaceRoad(pivot + curr_start, pivot + curr_end, RoadWidth);
            }
            return true;
        }

        public bool StraightRail(Vector3 start, Vector3 end, float RailLength)
        {
            int count = Mathf.CeilToInt((end - start).magnitude / RailLength);
            Vector3 offset = (end - start) / count;
            this.PlacePost(start);
            Vector3 curr = start;
            for (int i = 1; i <= count; i++)
            {
                Vector3 next = curr + offset;
                this.PlaceFence(curr, next);
                this.PlacePost(next);
                curr = next;
            }
            return true;
        }

        public bool StraightSection(Vector3 start, Vector3 end, float RoadWidth, float RailLength)
        {
            this.PlaceRoad(start, end, RoadWidth);
            Vector3 perpendicular = StaticHelper.BuildVector(Quaternion.AngleAxis(90, Vector3.up) * (end - start), RoadWidth / 2);
            this.StraightRail(start + perpendicular, end + perpendicular, RailLength);
            this.StraightRail(start - perpendicular, end - perpendicular, RailLength);
            return true;
        }

        /// <summary>
        /// Assumption: the angle from start to end is less than 180
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="start">relative to specified pivot</param>
        /// <param name="end">relative to specified pivot</param>
        /// <param name="TrackWidth"></param>
        /// <param name="RoadWidth"></param>
        /// <param name="RailLength"></param>
        /// <returns>true, if the section was deployed</returns>
        public bool CurvedSection(Vector3 pivot, Vector3 start, Vector3 end, float RoadWidth, float RailLength)
        {
            if (this.CurvedRoad(pivot, start, end, RoadWidth))
            {
                float halfWidth = RoadWidth / 2;
                if (this.CurvedRail(pivot, StaticHelper.BuildVector(start, start.magnitude - halfWidth), StaticHelper.BuildVector(end, end.magnitude - halfWidth), RailLength))
                {
                    return this.CurvedRail(pivot, StaticHelper.BuildVector(start, start.magnitude + halfWidth), StaticHelper.BuildVector(end, end.magnitude + halfWidth), RailLength);
                }
            }
            return false;
        }

        public bool SmartCurvedSection(Vector3 pivot, Vector3 start, Vector3 end, float RoadWidth, float RailLength)
        {
            //  find any "leftover" length
            if (Mathf.Abs(start.magnitude - end.magnitude) >= 0.01)
            {
                //  not a "pure curve", so add a straight section
                if (start.magnitude < end.magnitude)
                {
                    Vector3 new_start = start + ((end.magnitude - start.magnitude) * end.normalized);
                    //  pad with a straight section
                    if (this.StraightSection(pivot + start, pivot + new_start, RoadWidth, RailLength))
                    {
                        return this.CurvedSection(pivot, new_start, end, RoadWidth, RailLength);
                    }
                }
                else
                {
                    Vector3 new_end = end + ((start.magnitude - end.magnitude) * start.normalized);
                    //  pad with a straight section
                    if (this.StraightSection(pivot + new_end, pivot + end, RoadWidth, RailLength))
                    {
                        return this.CurvedSection(pivot, start, new_end, RoadWidth, RailLength);
                    }
                }
            }
            return this.CurvedSection(pivot, start, end, RoadWidth, RailLength);
        }

        private TrackBehaviour _behaviour;
    }
}
