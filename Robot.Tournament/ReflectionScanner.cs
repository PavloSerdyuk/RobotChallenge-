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
                try
                {
                    var a = Assembly.LoadFrom(filePath);
                    var allTypes = a.GetTypes();

                    var list = allTypes.Where(t => typeof(IRobotAlgorithm).IsAssignableFrom(t)).ToList();
                    if (list.Count > 0)
                    {
                        var algor = list[0];
                        var newInstance = a.CreateInstance(algor.ToString());
                        result.Add((IRobotAlgorithm) newInstance);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error loading dll from: {filePath}. Internal error: {ex.Message}");
                }

            }
            return result;
        }

        
    }
}
