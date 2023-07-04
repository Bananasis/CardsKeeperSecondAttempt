using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class CellTest : MonoBehaviour
{
    CellList<float> listCell = new();
    CellHashset<float> hashCell = new();
    CellDict<float, bool> dictCell = new();
    CellList<bool> listCellBool = new();
    private PassCell<bool> anyDict;
    private PassCell<bool> allDict;
    private PassCell<bool> anyList;
    private PassCell<bool> allList;
    private PassCell<float> sumList;
    private PassCell<float> sumSet;
    private ConnectionManager cm = new();

    void Start()
    {
        // hashCell.SubscribeAdd((f) => Debug.Log($"removeh {f}"), cm);
        // hashCell.SubscribeRemove((f) => Debug.Log($"addh {f}"), cm);
        //
        // listCell.SubscribeAdd((f) => Debug.Log($"removel {f}"), cm);
        // listCell.SubscribeRemove((f) => Debug.Log($"addl {f}"), cm);
        // listCell.SubscribeChange((f) => Debug.Log($"changel {f}"), cm);
        //
        // dictCell.AnyCell().Subscribe((f) => Debug.Log($"removeh {f}"), cm);
        //  dictCell.AllCell().Subscribe((f) => Debug.Log($"removeh {f}"), cm);
        listCellBool.AnyCell().Subscribe((f) => Debug.Log($"any {f}"), cm);
        listCellBool.AllCell().Subscribe((f) => Debug.Log($"all {f}"), cm);
        // listCell.SumCell().Subscribe((f) => Debug.Log($"removeh {f}"), cm);
        // hashCell.SumCell().Subscribe((f) => Debug.Log($"removeh {f}"), cm);
    }

    // Update is called once per frame
    private int i;

    void Update()
    {
        for (int k = 0; k < 1; k++)
        {
            listCellBool.Add(true);
        }

        if(i>2)  listCellBool.Add(false);
      
        i++;
    }
}