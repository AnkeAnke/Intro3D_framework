using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Vector of 2 integer values
/// 
/// author: Anke Friederici
/// </summary>
struct VectorInt
{
    public static VectorInt Zero
    {
        get { return new VectorInt(0, 0); }
    }

    public static VectorInt UnitX
    {
        get { return new VectorInt(1, 0); }
    }

    public static VectorInt UnitY
    {
        get { return new VectorInt(0, 1); }
    }

    public int X;
    public int Y;

    public VectorInt(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public VectorInt(VectorInt vec)
    {
        this.X = vec.X;
        this.Y = vec.Y;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException("BananaHash!");
    }

    public override bool Equals(object obj)
    {
        throw new NotImplementedException("BananaEquals!");
    }

    public static VectorInt operator +(VectorInt vec1, VectorInt vec2)
    {
        return new VectorInt(vec1.X + vec2.X, vec1.Y + vec2.Y);
    }

    public static VectorInt operator -(VectorInt vec1, VectorInt vec2)
    {
        return new VectorInt(vec1.X - vec2.X, vec1.Y - vec2.Y);
    }

    public static VectorInt operator *(VectorInt vec1, VectorInt vec2)
    {
        return new VectorInt(vec1.X * vec2.X, vec1.Y * vec2.Y);
    }

    public static VectorInt operator /(VectorInt vec1, VectorInt vec2)
    {
        return new VectorInt(vec1.X / vec2.X, vec1.Y / vec2.Y);
    }

    public static VectorInt operator *(float multiply, VectorInt vec2)
    {
        return new VectorInt((int)(multiply * vec2.X), (int)(multiply * vec2.Y));
    }

    public static VectorInt operator *(VectorInt vec2, float multiply)
    {
        return new VectorInt((int)(multiply * vec2.X), (int)(multiply * vec2.Y));
    }

    public static VectorInt operator *(int multiply, VectorInt vec2)
    {
        return new VectorInt((int)(multiply * vec2.X), (int)(multiply * vec2.Y));
    }

    public static VectorInt operator /(VectorInt vec2, float f)
    {
        return new VectorInt((int)(vec2.X / f), (int)(vec2.Y / f));
    }

    public static VectorInt operator /(VectorInt vec2, int f)
    {
        return new VectorInt((int)(vec2.X / f), (int)(vec2.Y / f));
    }
    public static bool operator >(VectorInt vec1, VectorInt vec2)
    {
        return (vec1.X > vec2.X && vec1.Y > vec2.Y);
    }

    public static bool operator >=(VectorInt vec1, VectorInt vec2)
    {
        return (vec1.X >= vec2.X && vec1.Y >= vec2.Y);
    }

    public static bool operator <(VectorInt vec1, VectorInt vec2)
    {
        return (vec1.X < vec2.X && vec1.Y < vec2.Y);
    }

    public static bool operator <=(VectorInt vec1, VectorInt vec2)
    {
        return (vec1.X <= vec2.X && vec1.Y <= vec2.Y);
    }

    public static bool operator ==(VectorInt vec1, VectorInt vec2)
    {
        return !(vec1 != vec2);
    }

    public static bool operator !=(VectorInt vec1, VectorInt vec2)
    {
        if ((object)vec1 == null || (object)vec2 == null)
        {
            if ((object)vec1 == null && (object)vec2 == null)
                return false;
            return true;
        }
        return (vec1.X != vec2.X || vec1.Y != vec2.Y);
    }

    public static VectorInt operator %(VectorInt vec, int i)
    {
        return new VectorInt(vec.X % i, vec.Y % i);
    }

    public static VectorInt operator %(VectorInt vec, VectorInt vec2)
    {
        return new VectorInt(vec.X % vec2.X, vec.Y % vec2.Y);
    }

    public VectorInt InterpolateTo(VectorInt vec, float t)
    {
        return (1 - t) * this + t * vec;
    }

    /// <summary>
    /// Aka distance in 4 neighbourhood.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CityblockDistance(VectorInt other)
    {
        return Math.Abs(other.X - this.X) + Math.Abs(other.Y + this.Y);
    }

    /// <summary>
    /// Aka distance in 8 neighbourhood.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int ChessboardDistance(VectorInt other)
    {
        return Math.Max(Math.Abs(other.X - this.X), Math.Abs(other.Y - this.Y));
    }

    public override String ToString()
    {
        return "[X= " + this.X + "; Y= " + this.Y + "]";
    }

    public bool Equals(VectorInt other)
    {
        return (this.X == other.X) && (this.Y == other.Y);
    }

    public static explicit operator Vector2(VectorInt vec)
    {
        return new Vector2(vec.X, vec.Y);
    }
}

