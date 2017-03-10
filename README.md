# ResizerTest


##ResizerTestConsole

Console App for bulk testing image resizing operations

The chosen library will be asked to resize 13 test images, at 6 resolutions - each operation will be started on a new thread.

By default - 10 threads will be used (giving a total of 780 concurrent resize operations) - though this is configurable - I suggest you start setting Threads to 1 - for only 78 concurrent operations.

*USAGE*: ResizerTestConsole \<TestLib, (ImageResizer|ImageProcessor|ImageSharp|SkiaSharp|CoreCompat)>,  'Library to test'

     :                    [\<Interactive (Y|True|1|N|False|0)>,]  'optional interactive mode: default No'

     :                    [\<Threads (Number)>,]  'optional Threads per processor: default 10'

     :                    [\<Algorithm,  (Bicubic|Triangle|NearestNeighbor|MitchellNetravali)>]  'optional ImageSharp Resampler Algorithm: default NearestNeighbor'

##ResizerTestCore

Dotnetcore version of **ResizerTestConsole** with some restructuring.

