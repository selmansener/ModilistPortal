using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModilistPortal.Infrastructure.Shared.Utils
{
    public static class FileSizeConverter
    {
        public static string Convert(long fileSizeInBytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (fileSizeInBytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                fileSizeInBytes = fileSizeInBytes / 1024;
            }

            return string.Format("{0:0.##} {1}", fileSizeInBytes, sizes[order]);
        }
    }
}
