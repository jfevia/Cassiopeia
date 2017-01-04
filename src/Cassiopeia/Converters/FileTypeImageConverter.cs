using System;
using System.Globalization;
using System.Windows.Data;
using Cassiopeia.Models;

namespace Cassiopeia.Converters
{
    internal class FileTypeImageConverter : IValueConverter
    {
        public object Convert(object obj, Type type, object parameter, CultureInfo culture)
        {
            if (!(obj is FileType))
                throw new ArgumentException(nameof(obj));

            var fileType = (FileType) obj;
            switch (fileType)
            {
                case FileType.Folder:
                    return "/Images/Folder.png";
                case FileType.Iso:
                    return "/Images/Iso.png";
                case FileType.WinExecutable:
                    return "/Images/WinExecutable.png";
                case FileType.Audio:
                    return "/Images/Audio.png";
                case FileType.Image:
                    return "/Images/Image.png";
                case FileType.Video:
                    return "/Images/Video.png";
                default:
                    return "/Images/Unknown.png";
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}