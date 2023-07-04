using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
internal class RoomShapes
{
    [SerializeField] private Spaces spaces = new Spaces(){space5 = true};

    public IReadOnlyList<DVector2> GetRoomShape()
    {
        List<DVector2> shape = new List<DVector2>();
        for (int i = -1; i < 3; i++)
        {
            for (int j = -1; j < 3; j++)
            {
                var pos = new DVector2(i, j);
                if (spaces[pos]) shape.Add(pos);
            }
        }

        return shape;
    }

    public int size
    {
        get
        {
            var j = 0;
            for (var i = 0; i < 16; i++)
            {
                if (spaces[i]) j++;
            }

            return j;
        }
    }

    [SerializeField] public int complexity1;
    [SerializeField] public int complexity2;
    [SerializeField] public int complexity3;
    [SerializeField] public int complexity4;
    public (int, int, int, int) complexity => (complexity1, complexity2, complexity3, complexity4);
}