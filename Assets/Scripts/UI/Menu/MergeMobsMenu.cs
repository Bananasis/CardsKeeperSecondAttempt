using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Menu
{
    public class MergeMobsMenu : MonoBehaviour
    {
        [Inject] private RoomHolder _roomHolder;
        [SerializeField] private List<MergeFormulaDisplay> formulas;
        [SerializeField] private RectTransform cardPos;
        [SerializeField] private Button cancel;
        [Inject] private GameManager _gameManager;
        private RoomCardUI _card;
    
        private void Awake()
        {
            cancel.onClick.Subscribe(Cancel);
        }
    
        public void BeginMerge(RoomCardUI card)
        {
            _card = card;
            card.position.preferredWorldPosition = cardPos.position;
            card.LockInteractable(false);
            card.Hold();
            gameObject.SetActive(true);
            formulas.ForEach(f => f.UpdateFormula());
            var merges = _roomHolder.GerMobsToUpgrade(card.room);
            int i = 0;
            foreach (var (key, value) in merges)
            {
                formulas[i++].UpdateFormula((key, value));
            }
        }
    
        public void Merge((((MobData, int), (MobData, int)), MobData) formula)
        {
            int cost = formula.Item2.cost - formula.Item1.Item1.Item1.cost * formula.Item1.Item1.Item2;
            cost -= formula.Item1.Item2.Item2 == 0 ? 0 : formula.Item1.Item2.Item1.cost * formula.Item1.Item2.Item2;
            if(_gameManager.money.val < cost+1) return;
            _gameManager.money.val -= cost;
            _card.UpdateCard(_roomHolder.Merge(formula, _card.room));
            Cancel();
        }
        
        public void Cancel()
        {
            _card?.LockInteractable(true);
            _card?.Release();
            _card = null;
            gameObject.SetActive(false);
        }
    }
}
