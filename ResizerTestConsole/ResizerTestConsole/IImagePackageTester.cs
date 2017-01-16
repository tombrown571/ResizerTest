using System.Collections.Concurrent;

namespace ResizerTestConsole
{
    public interface IImagePackageTester
    {


        bool ProcessImage(string outputName, int maxDimension);

        ConcurrentBag<ErrorLogStruct> Exceptions { get; }
    }
}