using System;
using System.IO;
using QHSalesApp.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace QHSalesApp.Droid
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }

        public void CopyFile(string sourceFilename, string destinationFilename, bool overwrite)
        {
            var sourcePath = GetLocalFilePath(sourceFilename);
            var destinationPath = GetLocalFilePath(destinationFilename);
            System.IO.File.Copy(sourcePath, destinationPath, overwrite);
        }
        public bool IsDbFileExist(string filename)
        {
            return File.Exists(GetLocalFilePath(filename));
        }
    }
}