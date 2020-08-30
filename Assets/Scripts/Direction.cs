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
    public static bool IsValid(this Direction d)
    {
        return !((d & Direction.Horizontal) == Direction.Horizontal || (d & Direction.Vertical) == Direction.Vertical || d == Direction.None);
    }

    public static bool FlagActive(this Direction d, Direction flag) { return ((d & flag) != 0); }

    public static bool HasOnly1Flag(this Direction d)
    {
        return (d != 0 && (d & (d - 1)) == 0);
    }

    public static int ActiveFlagCount(this Direction d)
    {
        int count = 0; int n = (int)d;
        while (n != 0) { n = n & (n - 1); count++; }
        return count;
    }

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

    public static int ToX(this Direction d)
    {
        if (!d.IsValid()) { return 0; }
        if (!d.FlagActive(Direction.Horizontal)) { return 0; }

        return d.FlagActive(Direction.Left) ? -1 : 1;
    }

    public static int ToY(this Direction d)
    {
        if (!d.IsValid()) return 0;
        if (!d.FlagActive(Direction.Vertical)) return 0;

        return d.FlagActive(Direction.Down) ? -1 : 1;
    }

    public static Direction FromXY(this Direction d, int x, int y)
    {
        if (y > 0) d |= Direction.Up;
        if (y < 0) d |= Direction.Down;
        if (x < 0) d |= Direction.Left;
        if (x > 0) d |= Direction.Right;

        return d;
    }

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
