using System.Runtime.InteropServices;

namespace MultiplayerMapEditor.Networking.System;

[StructLayout(LayoutKind.Explicit)]
internal ref struct GuidBuffer
{
    [FieldOffset(0)] public Guid Guid;

    [FieldOffset(0)] public ulong Part1;

    [FieldOffset(8)] public ulong Part2;

    public GuidBuffer(Guid guid)
    {
        Guid = guid;
    }

    public GuidBuffer(ulong part1, ulong part2)
    {
        Part1 = part1;
        Part2 = part2;
    }
}
