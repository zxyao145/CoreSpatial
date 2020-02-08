using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CoreSpatial.Converter.KML
{
    internal class Utf8StringWriter : StringWriter
    {
        private readonly Encoding _encoding;

        public Utf8StringWriter()
        {
        }

        public Utf8StringWriter(IFormatProvider formatProvider)
            : base(formatProvider)
        {
        }

        public Utf8StringWriter(StringBuilder sb)
            : base(sb)
        {
        }

        public Utf8StringWriter(StringBuilder sb, IFormatProvider formatProvider)
            : base(sb, formatProvider)
        {
        }


        public Utf8StringWriter(Encoding encoding)
        {
            _encoding = encoding;
        }

        public Utf8StringWriter(IFormatProvider formatProvider, Encoding encoding)
            : base(formatProvider)
        {
            _encoding = encoding;
        }

        public Utf8StringWriter(StringBuilder sb, Encoding encoding)
            : base(sb)
        {
            _encoding = encoding;
        }

        public Utf8StringWriter(StringBuilder sb, IFormatProvider formatProvider, Encoding encoding)
            : base(sb, formatProvider)
        {
            _encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return (null == _encoding) ? base.Encoding : _encoding; }
        }
    }
}
