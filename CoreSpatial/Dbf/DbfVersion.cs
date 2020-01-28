namespace CoreSpatial.Dbf
{
    internal enum DbfVersion : byte
    {
        Unknown = 0,
        FoxBase = 0x02,
        FoxBaseDBase3NoMemo = 0x03,
        VisualFoxPro = 0x30,
        VisualFoxProWithAutoIncrement = 0x31,
        dBase4SQLTableNoMemo = 0x43,
        dBase4SQLSystemNoMemo = 0x63,
        FoxBaseDBase3WithMemo = 0x83,
        dBase4WithMemo = 0x8B,
        dBase4SQLTableWithMemo = 0xCB,
        FoxPro2WithMemo = 0xF5,
        FoxBASE = 0xFB
    }
}
