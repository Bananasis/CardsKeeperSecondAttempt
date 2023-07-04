using System;

[Serializable]
public class Spaces
{
    public bool space0;
    public bool space1;
    public bool space2;
    public bool space3;
    public bool space4;
    public bool space5;
    public bool space6;
    public bool space7;
    public bool space8;
    public bool space9;
    public bool space10;
    public bool space11;
    public bool space12;
    public bool space13;
    public bool space14;
    public bool space15;

  

    public bool this[int index] =>
        index switch
        {
            0 => space0,
            1 => space1,
            2 => space2,
            3 => space3,
            4 => space4,
            5 => space5,
            6 => space6,
            7 => space7,
            8 => space8,
            9 => space9,
            10 => space10,
            11 => space11,
            12 => space12,
            13 => space13,
            14 => space14,
            15 => space15,
            
            
            
            
            _ => throw new ArgumentOutOfRangeException(),
        };

    public bool this[DVector2 index] =>
        this[index.x + 1 + 4 * (index.y + 1)];
}