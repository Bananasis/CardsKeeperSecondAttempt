using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace DefaultNamespace
{
    public class RoomCard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI cardName;
        [SerializeField] private List<Image> roomForm;
        [SerializeField] private Image sprite;
        [SerializeField] private Image icon1;
        [SerializeField] private Image icon2;
        [SerializeField] private MobCard mobCard;

        [SerializeField] private Button leftMob;
        [SerializeField] private Button rightMob;

        private List<MobData> mobs = new();
        private int mobIndex = 0;

        private void Awake()
        {
            leftMob?.onClick.Subscribe(PrevMob);
            rightMob?.onClick.Subscribe(NextMob);
          
        }

        public RoomData room { get; private set; }

        public void NextMob()
        {
            mobIndex = (mobIndex + 1) % mobs.Count;
            UpdateMobs();
        }

        public void PrevMob()
        {
            mobIndex = (mobIndex + mobs.Count - 1) % mobs.Count;
            UpdateMobs();
        }

        private void UpdateMobs()
        {
            mobCard.UpdateCard(new Mob(mobs[mobIndex],mobs.SelectMany(m => m.tags).ToList()));

            if (mobs.Count <= 1) return;
            icon1.enabled = true;
            icon1.sprite = mobs[(mobIndex+1) % mobs.Count].sprite;
            if (mobs.Count <= 2) return;
            icon2.enabled = true;
            icon2.sprite = mobs[(mobIndex+2) % mobs.Count].sprite;
        }


        public void UpdateCard(RoomData room)
        {
            mobIndex = 0;
            icon1.enabled = false;
            icon2.enabled = false;
            this.room = room;
            var roomShape = room.tiles.Keys;
            roomForm.ForEach((i) => i.enabled = false);
            var xOffset = roomShape.Max((t) => t.x) < 3 || roomShape.Min((t) => t.x) == -1 ? 1 : 0;
            var yOffset = roomShape.Max((t) => t.y) < 3 || roomShape.Min((t) => t.y) == -1 ? 1 : 0;


            foreach (var valueTuple in roomShape)
            {
                roomForm[(valueTuple.x + xOffset) * 4 + valueTuple.y + yOffset].enabled = true;
            }

            if (room.type != RoomType.Room)
            {
                mobCard.Clear();
                sprite.sprite = room.sprite;
                description.text = "";
                cardName.text = "";
                if (room.type == RoomType.Treasury)
                {
                    cardName.text = "Treasury";
                    description.text = "Generates and Stores money";
                    return;
                }

                cardName.text = "Entrance";
                description.text = "... to the dungeon";
                return;
            }

            mobs = room.mobs.ToList();

            UpdateMobs();
        }
    }
}