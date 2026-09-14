namespace TarkovCheat.Core.Memory;

/// <summary>IDA-style byte-pattern scanner ("48 8B 05 ?? ?? ?? ??").</summary>
public static class PatternScanner
{
    public static nint Scan(byte[] region, string pattern)
    {
        var (bytes, mask) = ParsePattern(pattern);
        if (bytes.Length == 0) return nint.Zero;

        for (int i = 0; i <= region.Length - bytes.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < bytes.Length; j++)
            {
                if (mask[j] && region[i + j] != bytes[j])
                {
                    found = false;
                    break;
                }
            }
            if (found)
                return (nint)i;
        }
        return nint.Zero;
    }

    public static nint ScanModule(MemoryManager mem, string moduleName, string pattern)
    {
        nint moduleBase = mem.GetModuleBase(moduleName);
        if (moduleBase == nint.Zero) return nint.Zero;

        int moduleSize = mem.GetModuleSize(moduleName);
        if (moduleSize <= 0) return nint.Zero;

        byte[] dump = mem.ReadBytes(moduleBase, moduleSize);
        nint offset = Scan(dump, pattern);
        return offset != nint.Zero ? moduleBase + offset : nint.Zero;
    }

    public static nint ResolveRelativeAddress(MemoryManager mem, nint instrAddr, int instrLen)
    {
        int rel = mem.Read<int>(instrAddr + instrLen - 4);
        return instrAddr + instrLen + rel;
    }

    public static List<nint> ScanAll(byte[] region, string pattern)
    {
        var (bytes, mask) = ParsePattern(pattern);
        var results = new List<nint>();
        if (bytes.Length == 0) return results;

        for (int i = 0; i <= region.Length - bytes.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < bytes.Length; j++)
            {
                if (mask[j] && region[i + j] != bytes[j])
                {
                    found = false;
                    break;
                }
            }
            if (found)
                results.Add((nint)i);
        }
        return results;
    }

    private static (byte[] bytes, bool[] mask) ParsePattern(string pattern)
    {
        string[] tokens = pattern.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        byte[] bytes = new byte[tokens.Length];
        bool[] mask = new bool[tokens.Length];

        for (int i = 0; i < tokens.Length; i++)
        {
            if (tokens[i] is "??" or "?")
            {
                bytes[i] = 0;
                mask[i] = false;
            }
            else
            {
                bytes[i] = Convert.ToByte(tokens[i], 16);
                mask[i] = true;
            }
        }
        return (bytes, mask);
    }
}
