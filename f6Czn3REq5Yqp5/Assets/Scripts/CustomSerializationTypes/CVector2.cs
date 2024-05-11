using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[Serializable]
public class CVector2
{ 
    private float _x;
    private float _y;

    public CVector2(float _x, float _y)
    {
        this._x = _x;
        this._y = _y;
    }

    public float x
    {
        get { return this._x; }
        set { this._x = value; }
    }

    public float y
    {
        get { return this._y; }
        set { this._y = value; }
    }


    public override string ToString()
    {
        return string.Format("CVector3: X:{0} Y:{1}",_x,_y);
    }

    public Vector2 ToVector2()
    {
        Vector2 vector2;
        vector2.x = _x;
        vector2.y = _y;
        return vector2;
    }

    public static implicit operator Vector2(CVector2 cVector)
    {
        return new Vector2(cVector.x, cVector.y);
    }

}
