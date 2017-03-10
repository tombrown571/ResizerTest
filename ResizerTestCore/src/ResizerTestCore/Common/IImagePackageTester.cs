using System.Collections.Concurrent;

namespace ResizerTestCore.Common
{
    public interface IImagePackageTester
    {
        bool ProcessImage(string outputName, int maxDimension);

        ConcurrentBag<ErrorLogStruct> Exceptions { get; }
    }
}