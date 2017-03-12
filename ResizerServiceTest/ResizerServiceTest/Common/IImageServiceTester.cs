﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ResizerServiceTest.Common
{
    public interface IImageServiceTester
    {
        Task<bool> GetRequestAsync();
        bool GetRequest();
        ConcurrentBag<ErrorLogStruct> Exceptions { get; }
    }
}
