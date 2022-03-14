using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nezmatematika.ViewModel.Helpers
{
    public static class ZipHelper
    {
        public static bool ZipUpDirectory(string dirToZipUp, string zipUpIntoDir) //dirToZipUp bude už přímo meziadresář nachystaný v Exports
        {
            try
            {
                ZipFile.CreateFromDirectory(dirToZipUp, zipUpIntoDir);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        public static bool UnzipDirectory(string zipPath, string destinationPath)
        {
            try
            {
                ZipFile.ExtractToDirectory(zipPath, destinationPath);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }
    }
}
