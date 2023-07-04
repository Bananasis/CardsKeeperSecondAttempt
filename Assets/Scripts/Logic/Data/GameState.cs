using System;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Data
{
    [Serializable]
    public class GameState
    {
        public int money;
        public List<RoomData> hand;
        public List<(RoomData, GridDirection)> field;

        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public GameState(string json)
        {
            JsonUtility.FromJsonOverwrite(json,this);
        }
        
        
    }
    
    
}