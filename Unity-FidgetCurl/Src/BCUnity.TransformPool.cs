using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BCUnity
{
    class TransformPool
    {
        public static void AddTemplate(String Name, Transform Template)
        {
            _instance._add_template(Name, Template);
        }

        public static bool HasTemplate(String TemplateName)
        {
            return _instance._has_template(TemplateName);
        }

        public static Transform Clone(String TemplateName)
        {
            return _instance._clone(TemplateName);
        }

        public static Transform Clone(Transform Template)
        {
            return _instance._clone(Template);
        }

        public static void Destroy(Transform t)
        {
            _instance._destroy(t);
        }

        private static TransformPool _instance
        {
            get
            {
                if (__instance == null)
                {
                    __instance = new TransformPool();
                }
                return __instance;
            }
        }

        private TransformPool()
        {
            this._clone_map = new Dictionary<int, int>();
            this._pool = new Dictionary<int, Stack<Transform>>();
            this._templates = new Dictionary<string, Transform>();
        }

        private void _add_template(String Name, Transform Template)
        {
            if (this._has_template(Name))
            {
                throw new Exception("TransformPool: template '" + Name + "' already exists");
            }
            this._templates.Add(Name, UnityEngine.Object.Instantiate(Template));
            this._templates[Name].gameObject.SetActive(false);
        }

        private bool _has_template(String Name)
        {
            return this._templates.ContainsKey(Name);
        }

        private Transform _clone(String TemplateName)
        {
            if (this._has_template(TemplateName))
            {
                return this._clone(this._templates[TemplateName]);
            }
            throw new Exception("TransformPool: template '" + TemplateName + "' not found");
        }

        private Transform _clone(Transform Template)
        {
            int index = Template.GetInstanceID();
            Transform ret;
            if (this._pool.ContainsKey(index) && this._pool[index].Count > 0)
            {
                ret = this._pool[index].Pop();
            }
            else
            {
                ret = UnityEngine.Object.Instantiate(Template);
            }
            this._clone_map[ret.GetInstanceID()] = index;
            ret.gameObject.SetActive(true);
            return ret;
        }

        private void _destroy(Transform t)
        {
            int index = t.GetInstanceID();
            if (this._clone_map.ContainsKey(index))
            {
                t.gameObject.SetActive(false);
                if (!this._pool.ContainsKey(this._clone_map[index]))
                {
                    this._pool[this._clone_map[index]] = new Stack<Transform>();
                }
                this._pool[this._clone_map[index]].Push(t);
            }
            else
            {
                UnityEngine.Object.Destroy(t.gameObject);
            }
        }

        private Dictionary<int, int> _clone_map;
        private Dictionary<int, Stack<Transform>> _pool;
        private Dictionary<String, Transform> _templates;
        private static TransformPool __instance;
    }
}