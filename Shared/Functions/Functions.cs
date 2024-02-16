using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;

public static class Functions
{
    public static bool CompareBytes(byte[] a, byte[] b)
    {
        if (a == b) return true;

        if (a == null || b == null || a.Length != b.Length) return false;

        for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;

        return true;
    }

    public static string ConvertByteSize(double byteCount)
    {
        string size = "0 Bytes";
        if (byteCount >= 1073741824.0)
            size = String.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
        else if (byteCount >= 1048576.0)
            size = String.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
        else if (byteCount >= 1024.0)
            size = String.Format("{0:##.##}", byteCount / 1024.0) + " KB";
        else if (byteCount > 0 && byteCount < 1024.0)
            size = byteCount.ToString() + " Bytes";

        return size;
    }

    public static bool TryParse(string s, out Point temp)
    {
        temp = Point.Empty;

        if (String.IsNullOrWhiteSpace(s)) return false;

        string[] data = s.Split(',');
        if (data.Length <= 1) return false;

        if (!Int32.TryParse(data[0], out int tempX))
            return false;

        if (!Int32.TryParse(data[1], out int tempY))
            return false;

        temp = new Point(tempX, tempY);
        return true;
    }
    public static Point Subtract(this Point p1, Point p2)
    {
        return new Point(p1.X - p2.X, p1.Y - p2.Y);
    }
    public static Point Subtract(this Point p1, int x, int y)
    {
        return new Point(p1.X - x, p1.Y - y);
    }
    public static Point Add(this Point p1, Point p2)
    {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }
    public static Point Add(this Point p1, int x, int y)
    {
        return new Point(p1.X + x, p1.Y + y);
    }
    public static string PointToString(Point p)
    {
        return String.Format("{0}, {1}", p.X, p.Y);
    }
    public static bool InRange(Point a, Point b, int i)
    {
        return Math.Abs(a.X - b.X) <= i && Math.Abs(a.Y - b.Y) <= i;
    }

    public static bool FacingEachOther(ExineDirection dirA, Point pointA, ExineDirection dirB, Point pointB)
    {
        if (dirA == DirectionFromPoint(pointA, pointB) && dirB == DirectionFromPoint(pointB, pointA))
        {
            return true;
        }

        return false;
    }

    public static string PrintTimeSpanFromSeconds(double secs, bool accurate = true)
    {
        TimeSpan t = TimeSpan.FromSeconds(secs);
        string answer;
        if (t.TotalMinutes < 1.0)
        {
            answer = string.Format("{0}s", t.Seconds);
        }
        else if (t.TotalHours < 1.0)
        {
            answer = accurate ? string.Format("{0}m {1:D2}s", t.Minutes, t.Seconds) : string.Format("{0}m", t.Minutes);
        }
        else if (t.TotalDays < 1.0)
        {
            answer = accurate ? string.Format("{0}h {1:D2}m {2:D2}s", (int)t.Hours, t.Minutes, t.Seconds) : string.Format("{0}h {1:D2}m", (int)t.TotalHours, t.Minutes);
        }
        else // more than 1 day
        {
            answer = accurate ? string.Format("{0}d {1:D2}h {2:D2}m {3:D2}s", (int)t.Days, (int)t.Hours, t.Minutes, t.Seconds) : string.Format("{0}d {1}h {2:D2}m", (int)t.TotalDays, (int)t.Hours, t.Minutes);
        }

        return answer;
    }

    public static string PrintTimeSpanFromMilliSeconds(double milliSeconds)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(milliSeconds);
        string answer;
        if (t.TotalMinutes < 1.0)
        {
            answer = string.Format("{0}.{1}s", t.Seconds, (decimal)(t.Milliseconds / 100));
        }
        else if (t.TotalHours < 1.0)
        {
            answer = string.Format("{0}m {1:D2}s", t.TotalMinutes, t.Seconds);
        }
        else if (t.TotalDays < 1.0)
        {
            answer = string.Format("{0}h {1:D2}m {2:D2}s", (int)t.TotalHours, t.Minutes, t.Seconds);
        }
        else
        {
            answer = string.Format("{0}d {1}h {2:D2}m {3:D2}s", (int)t.Days, (int)t.Hours, t.Minutes, t.Seconds);
        }

        return answer;
    }

    public static ExineDirection PreviousDir(ExineDirection d)
    {
        switch (d)
        {
            case ExineDirection.Up:
                return ExineDirection.UpLeft;
            case ExineDirection.UpRight:
                return ExineDirection.Up;
            case ExineDirection.Right:
                return ExineDirection.UpRight;
            case ExineDirection.DownRight:
                return ExineDirection.Right;
            case ExineDirection.Down:
                return ExineDirection.DownRight;
            case ExineDirection.DownLeft:
                return ExineDirection.Down;
            case ExineDirection.Left:
                return ExineDirection.DownLeft;
            case ExineDirection.UpLeft:
                return ExineDirection.Left;
            default: return d;
        }
    }
    public static ExineDirection NextDir(ExineDirection d)
    {
        switch (d)
        {
            case ExineDirection.Up:
                return ExineDirection.UpRight;
            case ExineDirection.UpRight:
                return ExineDirection.Right;
            case ExineDirection.Right:
                return ExineDirection.DownRight;
            case ExineDirection.DownRight:
                return ExineDirection.Down;
            case ExineDirection.Down:
                return ExineDirection.DownLeft;
            case ExineDirection.DownLeft:
                return ExineDirection.Left;
            case ExineDirection.Left:
                return ExineDirection.UpLeft;
            case ExineDirection.UpLeft:
                return ExineDirection.Up;
            default: return d;
        }
    }
    public static ExineDirection DirectionFromPoint(Point source, Point dest)
    {
        if (source.X < dest.X)
        {
            if (source.Y < dest.Y)
                return ExineDirection.DownRight;
            if (source.Y > dest.Y)
                return ExineDirection.UpRight;
            return ExineDirection.Right;
        }

        if (source.X > dest.X)
        {
            if (source.Y < dest.Y)
                return ExineDirection.DownLeft;
            if (source.Y > dest.Y)
                return ExineDirection.UpLeft;
            return ExineDirection.Left;
        }

        return source.Y < dest.Y ? ExineDirection.Down : ExineDirection.Up;
    }

    public static ExineDirection ShiftDirection(ExineDirection dir, int i)
    {
        return (ExineDirection)(((int)dir + i + 8) % 8);
    }

    public static Size Add(this Size p1, Size p2)
    {
        return new Size(p1.Width + p2.Width, p1.Height + p2.Height);
    }
    public static Size Add(this Size p1, int width, int height)
    {
        return new Size(p1.Width + width, p1.Height + height);
    }

    public static Point PointMove(Point p, ExineDirection d, int i)
    {
        switch (d)
        {
            case ExineDirection.Up:
                p.Offset(0, -i);
                break;
            case ExineDirection.UpRight:
                p.Offset(i, -i);
                break;
            case ExineDirection.Right:
                p.Offset(i, 0);
                break;
            case ExineDirection.DownRight:
                p.Offset(i, i);
                break;
            case ExineDirection.Down:
                p.Offset(0, i);
                break;
            case ExineDirection.DownLeft:
                p.Offset(-i, i);
                break;
            case ExineDirection.Left:
                p.Offset(-i, 0);
                break;
            case ExineDirection.UpLeft:
                p.Offset(-i, -i);
                break;
        }
        return p;
    }
    public static Point Left(Point p, ExineDirection d)
    {
        switch (d)
        {
            case ExineDirection.Up:
                p.Offset(-1, 0);
                break;
            case ExineDirection.UpRight:
                p.Offset(-1, -1);
                break;
            case ExineDirection.Right:
                p.Offset(0, -1);
                break;
            case ExineDirection.DownRight:
                p.Offset(1, -1);
                break;
            case ExineDirection.Down:
                p.Offset(1, 0);
                break;
            case ExineDirection.DownLeft:
                p.Offset(1, 1);
                break;
            case ExineDirection.Left:
                p.Offset(0, 1);
                break;
            case ExineDirection.UpLeft:
                p.Offset(-1, 1);
                break;
        }
        return p;
    }

    public static Point Right(Point p, ExineDirection d)
    {
        switch (d)
        {
            case ExineDirection.Up:
                p.Offset(1, 0);
                break;
            case ExineDirection.UpRight:
                p.Offset(1, 1);
                break;
            case ExineDirection.Right:
                p.Offset(0, 1);
                break;
            case ExineDirection.DownRight:
                p.Offset(-1, 1);
                break;
            case ExineDirection.Down:
                p.Offset(-1, 0);
                break;
            case ExineDirection.DownLeft:
                p.Offset(-1, -1);
                break;
            case ExineDirection.Left:
                p.Offset(0, -1);
                break;
            case ExineDirection.UpLeft:
                p.Offset(1, -1);
                break;
        }
        return p;
    }

    public static int MaxDistance(Point p1, Point p2)
    {
        return Math.Max(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));

    }

    public static ExineDirection ReverseDirection(ExineDirection dir)
    {
        switch (dir)
        {
            case ExineDirection.Up:
                return ExineDirection.Down;
            case ExineDirection.UpRight:
                return ExineDirection.DownLeft;
            case ExineDirection.Right:
                return ExineDirection.Left;
            case ExineDirection.DownRight:
                return ExineDirection.UpLeft;
            case ExineDirection.Down:
                return ExineDirection.Up;
            case ExineDirection.DownLeft:
                return ExineDirection.UpRight;
            case ExineDirection.Left:
                return ExineDirection.Right;
            case ExineDirection.UpLeft:
                return ExineDirection.DownRight;
            default:
                return dir;
        }
    }
    public static ItemInfo GetRealItem(ItemInfo Origin, ushort Level, ExineClass job, List<ItemInfo> ItemList)
    {
        if (Origin.ClassBased && Origin.LevelBased)
            return GetClassAndLevelBasedItem(Origin, job, Level, ItemList);
        if (Origin.ClassBased)
            return GetClassBasedItem(Origin, job, ItemList);
        if (Origin.LevelBased)
            return GetLevelBasedItem(Origin, Level, ItemList);
        return Origin;
    }
    public static ItemInfo GetLevelBasedItem(ItemInfo Origin, ushort level, List<ItemInfo> ItemList)
    {
        ItemInfo output = Origin;
        for (int i = 0; i < ItemList.Count; i++)
        {
            ItemInfo info = ItemList[i];
            if (info.Name.StartsWith(Origin.Name))
                if ((info.RequiredType == RequiredType.Level) && (info.RequiredAmount <= level) && (output.RequiredAmount < info.RequiredAmount) && (Origin.RequiredGender == info.RequiredGender))
                    output = info;
        }
        return output;
    }
    public static ItemInfo GetClassBasedItem(ItemInfo Origin, ExineClass job, List<ItemInfo> ItemList)
    {
        for (int i = 0; i < ItemList.Count; i++)
        {
            ItemInfo info = ItemList[i];
            if (info.Name.StartsWith(Origin.Name))
                if (((byte)info.RequiredClass == (1 << (byte)job)) && (Origin.RequiredGender == info.RequiredGender))
                    return info;
        }
        return Origin;
    }

    public static ItemInfo GetClassAndLevelBasedItem(ItemInfo Origin, ExineClass job, ushort level, List<ItemInfo> ItemList)
    {
        ItemInfo output = Origin;
        for (int i = 0; i < ItemList.Count; i++)
        {
            ItemInfo info = ItemList[i];
            if (info.Name.StartsWith(Origin.Name))
                if ((byte)info.RequiredClass == (1 << (byte)job))
                    if ((info.RequiredType == RequiredType.Level) && (info.RequiredAmount <= level) && (output.RequiredAmount <= info.RequiredAmount) && (Origin.RequiredGender == info.RequiredGender))
                        output = info;
        }
        return output;
    }

    public static string StringOverLines(string line, int maxWordsPerLine, int maxLettersPerLine)
    {
        string newString = string.Empty;

        string[] words = line.Split(' ');

        int lineLength = 0;

        for (int i = 0; i < words.Length; i++)
        {
            lineLength += words[i].Length + 1;

            newString += words[i] + " ";
            if (i > 0 && i % maxWordsPerLine == 0 && lineLength > maxLettersPerLine)
            {
                lineLength = 0;
                newString += "\r\n";
            }
        }

        return newString;
    }

    public static IEnumerable<byte[]> SplitArray(byte[] value, int bufferLength)
    {
        int countOfArray = value.Length / bufferLength;
        if (value.Length % bufferLength > 0)
            countOfArray++;
        for (int i = 0; i < countOfArray; i++)
        {
            yield return value.Skip(i * bufferLength).Take(bufferLength).ToArray();
        }
    }

    public static byte[] CombineArray(List<byte[]> arrays)
    {
        byte[] rv = new byte[arrays.Sum(x => x.Length)];
        int offset = 0;
        foreach (byte[] array in arrays)
        {
            System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
            offset += array.Length;
        }
        return rv;
    }
    public static byte[] SerializeToBytes<T>(T item)
    {
#pragma warning disable SYSLIB0011
        var formatter = new BinaryFormatter();
        using (var stream = new MemoryStream())
        {
            formatter.Serialize(stream, item);
            stream.Seek(0, SeekOrigin.Begin);
#pragma warning restore SYSLIB0011
            return stream.ToArray();
        }
    }
    public static object DeserializeFromBytes(byte[] bytes)
    {
#pragma warning disable SYSLIB0011
        var formatter = new BinaryFormatter();
        using (var stream = new MemoryStream(bytes))
        {
            var deserialized = formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
            return deserialized;
        }
    }
}