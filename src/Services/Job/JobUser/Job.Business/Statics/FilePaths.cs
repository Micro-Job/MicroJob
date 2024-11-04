using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Statics
{
    public class FilePaths
    {
        public static string image => Path.Combine("Files", "Images");
        public static string highImage => Path.Combine("Files", "Images", "High");
        public static string lowImage => Path.Combine("Files", "Images", "Low");
        public static string video => Path.Combine("Files", "Videos");
        public static string highVideo => Path.Combine("Files", "Videos", "High");
        public static string lowVideo => Path.Combine("Files", "Videos", "Low");
        public static string audio => Path.Combine("Files", "Audios");
        public static string document => Path.Combine("Files", "Documents");
    }
}