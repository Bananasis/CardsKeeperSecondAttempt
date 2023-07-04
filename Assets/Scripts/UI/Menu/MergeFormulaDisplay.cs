
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Menu
{


    public class MergeFormulaDisplay : MonoBehaviour
    {
        [SerializeField] private List<GameObject> formulaObjects;
        [SerializeField] private List<Image> sprites;
        [SerializeField] private TextMeshProUGUI price;
        [SerializeField] private Button confirm;
        private (((MobData, int), (MobData, int)), MobData) _formula;
        [Inject] private MergeMobsMenu _mergeMobs;

        private void Awake()
        {
            confirm.onClick.AddListener(Confirm);
        }

        public void UpdateFormula()
        {
            gameObject.SetActive(false);
        }

        private void Confirm()
        {
            _mergeMobs.Merge(_formula);
        }

        public void UpdateFormula((((MobData, int), (MobData, int)), MobData) formula)

        {
            int cost = formula.Item2.cost - formula.Item1.Item1.Item1.cost * formula.Item1.Item1.Item2;
            cost -= formula.Item1.Item2.Item2 == 0 ? 0 : formula.Item1.Item2.Item1.cost * formula.Item1.Item2.Item2;
            price.text = $"{cost}";
            gameObject.SetActive(true);
            _formula = formula;
            formulaObjects.ForEach(o => o.SetActive(false));
            int objectIndex = formulaObjects.Count - 1;
            int spriteIndex = sprites.Count - 1;
            sprites[spriteIndex--].sprite = formula.Item2.sprite;
            formulaObjects[objectIndex--].SetActive(true);
            for (int i = 0; i < formula.Item1.Item1.Item2; i++)
            {
                formulaObjects[objectIndex--].SetActive(true);
                sprites[spriteIndex--].sprite = formula.Item1.Item1.Item1.sprite;
                formulaObjects[objectIndex--].SetActive(true);
            }

            for (int i = 0; i < formula.Item1.Item2.Item2; i++)
            {
                formulaObjects[objectIndex--].SetActive(true);
                sprites[spriteIndex--].sprite = formula.Item1.Item2.Item1.sprite;
                formulaObjects[objectIndex--].SetActive(true);
            }
        }
    }
}