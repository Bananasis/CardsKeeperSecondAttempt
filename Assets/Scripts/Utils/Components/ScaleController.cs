using System;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace a
{
    public class ScaleController : MonoBehaviour, IScaleController, IBindableComponent
    {
        private Tween _rescale;
        private Tween _pulsate;
        private float _scale = 1;
        private RectTransform _rect;
        private RectTransform rect => _rect ??= GetComponent<RectTransform>();
        public void Bind(DiContainer container)
        {
            container.Bind<IScaleController>().To<ScaleController>().FromNewComponentSibling();
        }

        public void Scale(float scale,float time =0.5f)
        {
            _scale = scale;
            if(_locked) return;
            _rescale?.Kill();
            _pulsate?.Kill();
            _rescale = rect.DOScale(scale, time);
            _rescale.onComplete =() => onFinish?.Invoke();
        }

        public void Pulsate(float scale)
        {
            _pulsate?.Kill();
            if (_rescale?.IsPlaying() ?? false)
            {
                _rescale.onComplete = () =>
                {
                    _pulsate = _pulsate = rect.DOScale(scale, 0.7f).SetLoops(-1, LoopType.Yoyo);
                    onFinish?.Invoke();
                };
            }

            _pulsate = _pulsate = rect.DOScale(scale, 0.7f).SetLoops(-1, LoopType.Yoyo);
        }

        private bool _locked;

        public bool locked
        {
            get => _locked;
            set
            {
                if (value == _locked) return;
                _locked = value;
                if (!locked) Scale(_scale);
            }
        }

        public Action onFinish { get; set; }
    }
}