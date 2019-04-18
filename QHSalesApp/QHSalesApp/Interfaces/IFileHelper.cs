using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHSalesApp
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
        void CopyFile(string sourceFilename, string destinationFilename, bool overwrite);
        bool IsDbFileExist(string filename);
    }
}
