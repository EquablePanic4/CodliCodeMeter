using System;
using System.Collections.Generic;
using System.Text;

namespace CodliCodeMeter
{
    class MetricObject
    {
        public string FileName { get; set; }
        public long commentChars { get; set; }
        public long commentLines { get; set; }
        public long codeChars { get; set; }
        public long codeLines { get; set; }
    }
}
