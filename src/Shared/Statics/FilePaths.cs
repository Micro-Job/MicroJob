using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Statics
{
    public class FilePaths
    {
        public static string image => Path.Combine("Files", "Images");
        //public static string highImage => Path.Combine("Files", "Images", "High");
        //public static string lowImage => Path.Combine("Files", "Images", "Low");
        public static string document => Path.Combine("Files", "Documents");
    }
}
