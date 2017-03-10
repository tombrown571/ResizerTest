using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ResizerTestCore.Processor
{
    public abstract class BaseImageProcessor
    {
        protected int _newWidth;
        protected int _newHeight;
        protected double _ratio;
        protected int _quality;
        protected string _mode;
        protected string _format;

        private Preset _preset;
        public Preset Preset
        {
            set
            {
                _preset = value;
                PresetCalculations();
            }
        }

        protected BaseImageProcessor()
        {
        }

        private void PresetCalculations()
        {
            _newWidth = _preset.Width;
            _newHeight = _preset.Height;
            _quality = _preset.Quality;
            _mode = _preset.Mode;
            //switch (_preset.BackgroundColour)
            //{
            //    case "black":
            //        _bgColor = Brushes.Black;
            //        break;
            //    case "purple":
            //        _bgColor = Brushes.Purple;
            //        break;
            //    default:
            //        _bgColor = Brushes.White;
            //        break;
            //}
            _format = _preset.Format;
            _ratio = (double)_newWidth / (double)_newHeight;
        }

        public MemoryStream ResizeImage(Stream inputStream)
        {
            if (inputStream == null || inputStream.Length == 0)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }
            if (_preset == null)
            {
                throw new Exception("Preset must be set prior to calling ResizeImage");
            }

            return CallLibraryResizer(inputStream);
        }

        protected abstract MemoryStream CallLibraryResizer(Stream inputStream);

    }
}
