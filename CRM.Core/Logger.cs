using System;
using System.IO;

namespace CRM.Core
{
    public static class Logger
    {
        private const string path = "../Logs/";
        private const string log = "log.txt";

        public static void Writer(string message)
        {
            if (!Directory.Exists(path))
            {
                try
                {

                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            using (var sWriter = new StreamWriter(path + log, true))
            {
                sWriter.WriteLine(DateTime.Now + " " + message);
            }
        }
    }
}