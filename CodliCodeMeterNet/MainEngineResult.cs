using System;
using System.Collections.Generic;
using System.Text;

namespace CodliCodeMeterNet
{
    class MainEngineResult
    {
        public long commentsChars { get; set; }
        public long commentsLines { get; set; }
        public long codeLines { get; set; }
        public long codeChars { get; set; }
        public long filesSkiped { get; set; }
        public long fileScaned { get; set; }
        public long miliseconds { get; set; }
        public string elapsedTime { get; set; }
    }
}
