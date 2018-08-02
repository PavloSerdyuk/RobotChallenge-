using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Robot.Common;

namespace Robot.Tournament
{
    public class ReflectionScanner
    {

        public static string DirPath = AppDomain.CurrentDomain.BaseDirectory + @"AlgorithmDlls\";

        public static string[] ScanLibs()
        {
            return Directory.GetFiles(DirPath, "*.dll");
        }

        public static List<IRobotAlgorithm> Scan()
        {
            var result = new List<IRobotAlgorithm>();
            var filePaths = ScanLibs();

            foreach (var filePath in filePaths)
            {
                var a = Assembly.LoadFrom(filePath);
                var allTypes = a.GetTypes();

                var list = allTypes.Where(t => typeof(IRobotAlgorithm).IsAssignableFrom(t)).ToList();
                if (list.Count <= 0) continue;

                var algor = list[0];
                var newInstance = (IRobotAlgorithm) a.CreateInstance(algor.ToString());

                if (newInstance == null)
                    throw new Exception($"Could not initialize instance in dll: {filePath}");

                if (newInstance.Author == null)
                    throw new Exception($"Author name could not be null in dll: {filePath}");

                result.Add(newInstance);
            }
            return result;
        }


    }
}
