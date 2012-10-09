using System;
using System.IO;
using System.Linq;

namespace MvcInstaller
{
    class FileWriter
    {
        private string _fileName;

        public FileWriter(string fileName)
        {
            this._fileName = fileName;
        }

        public void Write(string msg)
        {
            FileStream fs;

            try
            {
                fs = new FileStream(_fileName, FileMode.Append);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory((new FileInfo(_fileName)).DirectoryName);
                fs = new FileStream(_fileName, FileMode.Append);
            }

            StreamWriter sw = new StreamWriter(fs);
            try
            {
                sw.WriteLine(msg);
            }
            catch
            {
            }
            finally
            {
                try
                {
                    sw.Close();
                }
                catch
                {

                }
            }
        }
    }
}
