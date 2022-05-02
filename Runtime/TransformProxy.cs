using MoonSharp.Interpreter;
using UnityEngine;

namespace Hasklee {

    class TransformProxy
    {
        Transform transform;

        [MoonSharpHidden]
        public TransformProxy(Transform transform)
        {
            this.transform = transform;
        }

        public Vector3 forward
        {
            get => transform.forward;
            set => transform.forward = value;
        }

        public Vector3 lossyScale
        {
            get => transform.lossyScale;
        }

        public Transform parent
        {
            get => transform.parent;
            set => transform.parent = value;
        }

        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Vector3 right
        {
            get => transform.right;
            set => transform.right = value;
        }

        public Quaternion rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public Vector3 up
        {
            get => transform.up;
            set => transform.up = value;
        }


        public void Translate(Vector3 translation)
        {
            transform.Translate(translation);
        }

        public void Translate(Vector3 translation, Transform relativeTo)
        {
            transform.Translate(translation, relativeTo);
        }

        public void RotateAround(Vector3 point, Vector3 axis, float angle)
        {
            transform.RotateAround(point, axis, angle);
        }

        public void SetParent(Transform t, bool b)
        {
            transform.SetParent(t, b);
        }

        public void SetParent(Transform t)
        {
            transform.SetParent(t, true);
        }

    }

}
