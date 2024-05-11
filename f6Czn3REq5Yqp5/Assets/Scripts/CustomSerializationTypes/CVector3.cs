using UnityEngine;
using System;

/// <summary>
/// Used as Vector3 replacement when serializing
/// </summary>
[Serializable]
public class CVector3
{ 
    private float _x;
    private float _y;
    private float _z;

    public CVector3(float _x, float _y, float _z)
    {
        this._x = _x;
        this._y = _y;
        this._z = _z;
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

    public float z
    {
        get { return this._z; }
        set { this._z = value; }
    }

    public override string ToString()
    {
        return string.Format("CVector3: X:{0} Y:{1} Z:{2}",_x,_y,_z);
    }

    /// <summary>
    /// Cast to Vector3
    /// </summary>
    /// <param name="cVector"></param>
    public static implicit operator Vector3(CVector3 cVector)
    {
        return new Vector3(cVector.x, cVector.y, cVector.z);
    }

}
