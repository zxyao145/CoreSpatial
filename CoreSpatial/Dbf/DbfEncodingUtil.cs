using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CoreSpatial.Dbf
{
    class DbfEncodingUtil
    {
        private static readonly
            Dictionary<byte, int> LangIdToEncoding
            = new Dictionary<byte, int>()
            {
                {0x00 ,65001},
                {0x01, 437},
                {0x02, 850},
                {0x08, 865},
                {0x09, 437},
                {0x0A, 850},
                {0x0B, 437},
                {0x0D, 437},
                {0x0E, 850},
                {0x0F, 437},
                {0x10, 850},
                {0x11, 437},
                {0x12, 850},
                {0x13, 932},
                {0x14, 850},
                {0x15, 437},
                {0x16, 850},
                {0x17, 865},
                {0x18, 437},
                {0x19, 437},
                {0x1A, 850},
                {0x1B, 437},
                {0x1C, 863},
                {0x1D, 850},
                {0x1F, 852},
                {0x22, 852},
                {0x23, 852},
                {0x24, 860},
                {0x25, 850},
                {0x26, 866},
                {0x37, 850},
                {0x40, 852},
                {0x4D, 936},
                {0x4E, 949},
                {0x4F, 950},
                {0x50, 874},
                {0x58, 1252},
                {0x59, 1252},
                {0x64, 852},
                {0x65, 866},
                {0x66, 865},
                {0x67, 861},
                {0x6A, 737},
                {0x6B, 857},
                {0x6C, 863},
                {0x78, 950},
                {0x79, 949},
                {0x7A, 936},
                {0x7B, 932},
                {0x7C, 874},
                {0x86, 737},
                {0x87, 852},
                {0x88, 857},
                {0xC8, 1250},
                {0xC9, 1251},
                {0xCA, 1254},
                {0xCB, 1253},
                {0xCC, 1257}
            };


        public static Encoding DefaultEncoding
            => Thread.CurrentThread.CurrentCulture.Name != "zh-CN"
                ? CodePagesEncodingProvider.Instance.GetEncoding(1512)
                : CodePagesEncodingProvider.Instance.GetEncoding(936);

        private static readonly Dictionary<int, byte> EncodingToLangId;

        static DbfEncodingUtil()
        {
            LangIdToEncoding.Add(0x03, Encoding.Default.CodePage);
            LangIdToEncoding.Add(0x57, Encoding.Default.CodePage);

            EncodingToLangId = new Dictionary<int, byte>();
            foreach (var keyValue in LangIdToEncoding)
            {
                EncodingToLangId.TryAdd(keyValue.Value, keyValue.Key);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        public static Encoding GetEncoding(byte id)
        {
            if (LangIdToEncoding.ContainsKey(id))
            {
                var codePage = LangIdToEncoding[id];
                Encoding encoding = Encoding.GetEncoding(codePage);
                return encoding;
            }
            else
            {
                return null;
            }
        }

        public static byte GetLangId(Encoding encoding)
        {
            int codePage = encoding.CodePage;
            if (EncodingToLangId.ContainsKey(codePage))
            {
                var langId = EncodingToLangId[codePage];
                return langId;
            }
            else
            {
                return 0x03;
            }
        }

    }
}