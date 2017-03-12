using System;
using System.Collections.Generic;
using System.Text;

namespace ResizerServiceTest.Common
{
    public struct ErrorLogStruct
    {
        public Exception Ex { get; set; }
        public string LogText { get; set; }
    }
}
