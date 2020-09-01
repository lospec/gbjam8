// Left this here in case there's some conflict or error
//public enum Direction
//{
//    N,
//    NE,
//    E,
//    SE,
//    S,
//    SW,
//    W,
//    NW
//}

using UnityEngine;

[System.Flags]
public enum Direction
{
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,

    Vertical = Up | Down,
    Horizontal = Left | Right,
    All = ~0,
    None = 0,
}

static class DirectionEnumExt
{

    /// <summary>
    /// Checks if this direction is valid, as in does the direction exist on a compass
    /// Invalid directions include: Left-Right, Up-Down, None, and other variant of them
    /// </summary>
    public static bool IsValid(this Direction d)
    {
        return !((d & Direction.Horizontal) == Direction.Horizontal || (d & Direction.Vertical) == Direction.Vertical || d == Direction.None);
    }

    /// <summary>
    /// Checks if a given direction flag is used
    /// </summary>
    /// <param name="flag">The direction flag to check, can be a combination, for example: Horizontal, if either of them(Left or Right) is used, this will still return true</param>
    public static bool FlagActive(this Direction d, Direction flag) { return ((d & flag) != 0); }

    /// <summary>
    /// Checks if this direction has only 1 flag(Up, Down, Left, or Right)
    /// </summary>
    public static bool HasOnly1Flag(this Direction d)
    {
        return (d != 0 && (d & (d - 1)) == 0);
    }

    /// <summary>
    /// Count how many direction flag is used in this direction
    /// </summary>
    public static int ActiveFlagCount(this Direction d)
    {
        int count = 0; int n = (int)d;
        while (n != 0) { n = n & (n - 1); count++; }
        return count;
    }

    /// <summary>
    /// Convert this direction into x and y int, this only works if the direction is valid (see <seealso cref="Direction.IsValid()")/>
    /// </summary>
    /// <param name="x">The variable to store the x component</param>
    /// <param name="y">The variable to store the y component</param>
    public static void ToXY(this Direction d, out int x, out int y)
    {
        x = y = 0;

        if (d.IsValid())
        {
            if (d.FlagActive(Direction.Horizontal))
                x = d.FlagActive(Direction.Left) ? -1 : 1;

            if (d.FlagActive(Direction.Vertical))
                y = d.FlagActive(Direction.Down) ? -1 : 1;
        }
    }

    /// <summary>
    /// Convert this direction into x int discarding the y, this only works if the direction is valid (see <seealso cref="Direction.IsValid()")/>
    /// </summary>
    /// <returns>The x/horizontal component of the direction</returns>
    public static int ToX(this Direction d)
    {
        if (!d.IsValid()) { return 0; }
        if (!d.FlagActive(Direction.Horizontal)) { return 0; }

        return d.FlagActive(Direction.Left) ? -1 : 1;
    }

    /// <summary>
    /// Convert this direction into x int discarding the y, this only works if the direction is valid (see: <see cref="Direction.IsValid()")/>
    /// </summary>
    /// <returns>The y/vertical component of the direction in int value</returns>
    public static int ToY(this Direction d)
    {
        if (!d.IsValid()) return 0;
        if (!d.FlagActive(Direction.Vertical)) return 0;

        return d.FlagActive(Direction.Down) ? -1 : 1;
    }

    /// <summary>
    /// Sets this direction based on a given x and y component
    /// </summary>
    /// <param name="x">The x component of the direction (absolute value can be more than 1)</param>
    /// <param name="y">The y component of the direction (absolute value can be more than 1)</param>
    public static Direction FromXY(this Direction d, int x, int y)
    {
        d = Direction.None;
        if (y > 0) d |= Direction.Up;
        if (y < 0) d |= Direction.Down;
        if (x < 0) d |= Direction.Left;
        if (x > 0) d |= Direction.Right;

        return d;
    }

    /// <summary>
    /// Convert this direction into Vector2, this only works if the direction is valid (see <seealso cref="Direction.IsValid()")/>
    /// Note: Vector is not normalized, if the direction is diagonal it'll return variation of 1,1 values
    /// </summary>
    /// <returns>This direction represented as Vector2</returns>
    public static Vector2 ToVector2(this Direction d)
    {
        Vector2 v = Vector2.zero;

        if (d.IsValid())
        {
            if (d.FlagActive(Direction.Horizontal))
                v.x = d.FlagActive(Direction.Left) ? -1 : 1;

            if (d.FlagActive(Direction.Vertical))
                v.y = d.FlagActive(Direction.Down) ? -1 : 1;
        }

        return v;
    }


    /// <summary>
    /// Sets this direction based on a given 2d vector
    /// </summary>
    /// <param name="v">The vector to be converted (components absolute value can be more than 1)</param>
    public static Direction FromVector2(this Direction d, Vector2 v)
    {
        d = Direction.None;
        if (v.y > 0) d |= Direction.Up;
        if (v.y < 0) d |= Direction.Down;
        if (v.x < 0) d |= Direction.Left;
        if (v.x > 0) d |= Direction.Right;

        return d;
    }

    /// <summary>
    /// Create a new direction that is the opposite of this direction
    /// </summary>
    /// <returns>The opposite direction of this direction, if this direction is invalid (see: <see cref="Direction.IsValid()") then returns None</returns>
    public static Direction Inverted(this Direction d)
    {
        if (!d.IsValid()) return Direction.None;

        Direction invert = Direction.None;

        if (d.FlagActive(Direction.Up)) invert |= Direction.Down;
        if (d.FlagActive(Direction.Down)) invert |= Direction.Up;
        if (d.FlagActive(Direction.Left)) invert |= Direction.Right;
        if (d.FlagActive(Direction.Right)) invert |= Direction.Left;

        return invert;
    }
}
