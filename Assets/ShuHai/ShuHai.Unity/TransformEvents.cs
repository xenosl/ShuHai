using System;
using UnityEngine;

namespace ShuHai.Unity
{
    public sealed class TransformEvents : MonoBehaviour
    {
        #region Parent Changed

        public event Action<Transform, Transform> ParentChanged;

        private Transform _lastParent;

        private void ParentChangedEnable() { _lastParent = transform.parent; }

        private void ParentChangedUpdate()
        {
            if (transform.parent == _lastParent)
                return;
            ParentChanged?.Invoke(transform, _lastParent);
            _lastParent = transform.parent;
        }

        #endregion Parent Changed

        //private class Event<TValue>
        //{
        //    public readonly Transform Owner;

        //    public readonly Action Enable;
        //    public readonly Action Disable;

        //    public Event(Transform owner, Action enable = null, Action disable = null)
        //    {
        //        Owner = owner;
        //        this.Enable = enable;
        //        this.Disable = disable;
        //    }

        //    public void Update(Action<Transform, TValue> evt, TValue value)
        //    {
        //        bool valid = evt != null;
        //        if (!lastValid && valid)
        //            Enable?.Invoke();
        //        else if (lastValid && !valid)
        //            Disable?.Invoke();
        //        lastValid = evt != null;

        //        if (!valid)
        //            return;

        //        if (EqualityComparer<TValue>.Default.Equals(value, lastValue))
        //            return;
        //        evt(Owner, lastValue);
        //        lastValue = value;
        //    }

        //    private bool lastValid;

        //    private TValue lastValue;
        //}

        #region Unity Events

        private void OnEnable() { ParentChangedEnable(); }

        private void Update() { ParentChangedUpdate(); }

        #endregion Unity Events
    }
}
