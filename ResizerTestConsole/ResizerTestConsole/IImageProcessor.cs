using System.Collections.Concurrent;

namespace ResizerTestConsole
{
    public interface IImageProcessor
    {


        bool ProcessImage(string outputName, int maxDimension);

        ConcurrentBag<ErrorLogStruct> Exceptions { get; }
    }
}