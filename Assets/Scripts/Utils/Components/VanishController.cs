using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace DefaultNamespace
{
    [RequireComponent(typeof(Image))]
    public class VanishController : MonoBehaviour, IVanishController, IBindableComponent
    {
        private Tween _vanishTween;
        private Tween _vanishTextTween;

        public bool vanishing { get; private set; }

        //private List<Image> images;
        private List<TextMeshProUGUI> texts;
        private Material mat;
        private static readonly int Vanishing = Shader.PropertyToID("_Vanishing");

        public void Awake()
        {
            texts = GetComponentsInChildren<TextMeshProUGUI>().ToList();
            var image = GetComponent<Image>();
            mat = new Material(image.material);
            image.material = mat;
            foreach (var componentsInChild in GetComponentsInChildren<Image>())
            {
                componentsInChild.material = mat;
            }
        }

        public void Vanish(float vanishDegree, Action onComplete = null)
        {
            vanishing = vanishDegree != 0;
            _vanishTween?.Kill();
            _vanishTextTween?.Kill();
            _vanishTextTween = DOTween.To(() => texts.Count == 0 ? 0 : texts[0].color.a,
                (a) => texts.ForEach((text) =>
                {
                    var color = text.color;
                    color.a = a;
                    text.color = color;
                }), 1 - vanishDegree, 1);
            _vanishTween = mat.DOFloat(1 - vanishDegree,
                Vanishing, 1);
            if (onComplete != null)
                _vanishTween.onComplete = onComplete.Invoke;
        }

        public void VanishInstant(float degree)
        {
            _vanishTween?.Kill();
            _vanishTextTween.Kill();
            texts.ForEach(t =>
            {
                var color = t.color;
                color.a = 1 - degree;
                t.color = color;
            });
            mat.SetFloat(Vanishing, 1 - degree);
        }

        public void Bind(DiContainer container)
        {
            container.Bind<IVanishController>().To<VanishController>().FromNewComponentSibling();
        }
    }
}