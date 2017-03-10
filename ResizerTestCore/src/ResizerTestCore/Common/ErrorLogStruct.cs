using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResizerTestCore.Common
{
    public struct ErrorLogStruct
    {
        public Exception Ex { get; set; }
        public string LogText { get; set; }
    }
}
