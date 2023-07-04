using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public abstract class PositionController : MonoBehaviour, ISelfManagedPosition, IBindableComponent
    {
        protected Tween _rotationTween;
        protected Tween _positionTween;
        protected bool _preferLocalPosition;
        protected bool _preferLocalRotation;

        public virtual void OnEnable()
        {
            _rotationTween?.Kill();
            _positionTween?.Kill();
        }

        private void OnDisable()
        {
            _rotationTween?.Kill();
            _positionTween?.Kill();
        }


        public abstract void Bind(DiContainer container);


        protected Vector3 _preferredPosition;
        protected Quaternion _preferredRotation;
        protected bool _selfManagedRotation = true;
        protected bool _selfManagedPosition = true;

        protected virtual void GoTo(Vector3 pos)
        {
            _positionTween?.Kill();
            if (!_preferLocalPosition)
            {
                _positionTween = transform.DOMove(pos, 0.5f);
                return;
            }

            _positionTween = transform.DOLocalMove(pos, 0.5f);
        }

        protected virtual void RotateTo(Quaternion rot)
        {
            _rotationTween?.Kill();
            if (!_preferLocalRotation)
            {
                _rotationTween = transform.DORotateQuaternion(rot, 0.5f);
                return;
            }

            _rotationTween = transform.DOLocalRotateQuaternion(rot, 0.5f);
        }

        protected virtual void Set(Vector3 pos, bool local)
        {
            if (!local)
            {
                transform.position = pos;
                return;
            }

            transform.localPosition = pos;
        }

        protected virtual void Set(Quaternion rot, bool local)
        {
            _rotationTween?.Kill();
            _rotationTween = transform.DORotateQuaternion(rot, 0.5f);
        }

        protected virtual Quaternion GetRotation(bool local)
        {
            return local ? transform.localRotation : transform.rotation;
        }

        protected virtual Vector3 GetPosition(bool local)
        {
            return local ? transform.localPosition : transform.position;
        }


        public Vector3 preferredPosition
        {
            set
            {
                if (_preferredPosition == value && !_preferLocalPosition) return;
                _preferLocalPosition = true;
                _preferredPosition = value;
                if (_selfManagedPosition) GoTo(value);
            }
        }

        public Vector3 preferredWorldPosition
        {
            set
            {
                if (_preferredPosition == value && _preferLocalPosition) return;
                _preferLocalPosition = false;
                _preferredPosition = value;
                if (_selfManagedPosition) GoTo(value);
            }
        }

        public Quaternion preferredRotation
        {
            set
            {
                if (_preferredRotation == value && !_preferLocalRotation) return;
                _preferLocalRotation = true;
                _preferredRotation = value;
                if (_selfManagedRotation && !lockedRotation) RotateTo(value);
            }
        }

        public Quaternion preferredWorldRotation
        {
            set
            {
                if (_preferredRotation == value && _preferLocalRotation) return;
                _preferLocalRotation = false;
                _preferredRotation = value;
                if (_selfManagedRotation && !lockedRotation) RotateTo(value);
            }
        }

        public bool selfManaged
        {
            set
            {
                selfManagedPosition = value;
                selfManagedRotation = value;
            }
        }

        public bool selfManagedRotation
        {
            get => _selfManagedRotation;
            set
            {
                if (value == _selfManagedRotation) return;
                _selfManagedRotation = value;
                if (_selfManagedRotation && !lockedRotation)
                {
                    RotateTo(_preferredRotation);
                    return;
                }

                _rotationTween?.Kill();
            }
        }

        public bool selfManagedPosition
        {
            get => _selfManagedPosition;
            set
            {
                if (value == _selfManagedPosition) return;
                _selfManagedPosition = value;
                if (_selfManagedPosition && !lockedPosition)
                {
                    GoTo(_preferredPosition);
                    return;
                }

                _positionTween?.Kill();
            }
        }

        public Vector3 currentPosition
        {
            get => GetPosition(true);
            set
            {
                Set(value, true);
                if (selfManagedPosition)
                    GoTo(_preferredPosition);
            }
        }

        public Quaternion currentRotation
        {
            get => GetRotation(true);
            set
            {
                Set(value, true);
                if (selfManagedRotation)
                    RotateTo(_preferredRotation);
            }
        }

        public Vector3 currentWorldPosition
        {
            get => GetPosition(false);
            set
            {
                Set(value, false);
                if (selfManagedPosition)
                    GoTo(_preferredPosition);
            }
        }

        public Quaternion currentWorldRotation
        {
            get => GetRotation(false);
            set
            {
                Set(value, false);
                if (selfManagedRotation)
                    RotateTo(_preferredRotation);
            }
        }

        public bool locked
        {
            set
            {
                lockedPosition = value;
                lockedRotation = value;
            }
        }


        protected bool _lockedPosition;
        protected bool _lockedRotation;

        public bool lockedPosition
        {
            get => _lockedPosition;
            set
            {
                if (value == _lockedPosition) return;
                _lockedPosition = value;
                if (_selfManagedPosition && !_lockedPosition)
                {
                    GoTo(_preferredPosition);
                }
            }
        }

        public bool lockedRotation
        {
            get => _lockedRotation;
            set
            {
                if (value == _lockedRotation) return;
                _lockedRotation = value;
                if (_selfManagedRotation && !_lockedRotation)
                {
                    RotateTo(_preferredRotation);
                }
            }
        }
    }
}