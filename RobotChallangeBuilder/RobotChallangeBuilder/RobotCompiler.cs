using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Robot.Common;

namespace RobotChallangeBuilder
{
    class RobotCompiler
    {
        private string compilerPath = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools";

        public void Compile(string folder, int counter)
        {
            //    string command = String.Format("csc /target:library /out:File2.dll /warn:0 /nologo /debug {0}*.cs", folder);
            //string pathToVsvars32 = "C:\\Program Files\\Microsoft Visual Studio 14.0\\Common7\\Tools";

            foreach (var file in Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories))
            {
                if (file.Contains(".cs") && !file.Contains("\\Properties\\"))
                {
                    folder = file.Substring(0, file.LastIndexOf('\\') + 1);
                    break;
                }
            }

            if (!File.Exists(folder + "Robot.Common.dll"))
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Robot.Common.dll", folder + "Robot.Common.dll");

            var arguments = string.Format(
                "cd /d {0} &" +
                "vsvars32.bat" + "&" +
                "cd /d {1} &" +
                "csc /t:library /r:Robot.Common.dll /out:algorithm.dll *.cs",
                compilerPath, folder);

            //arguments = "csc /t:library /r:Robot.Common.dll /out:algorithm.dll *.cs";

            var cmd = new Process
            {
                StartInfo =
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.GetEncoding(866),
                    RedirectStandardError = true,
                    FileName = @"cmd.exe",
                    Arguments = @"/C " + arguments
                }
            };

            cmd.Start();
            string output = cmd.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
            // Чекати на завершення
            cmd.WaitForExit();
            try
            {
                if (File.Exists(folder + "algorithm.dll"))
                {
                    File.Copy(folder + "algorithm.dll", @"C:\Robots\" + counter + ".dll");
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

        }

        private bool СheckRun(string path, out string author)
        {
            author = "";
            var a = Assembly.LoadFrom(path);
            Type[] allTypes = a.GetTypes();

            var list = allTypes.Where(t => typeof(IRobotAlgorithm).IsAssignableFrom(t)).ToList();
            if (list.Count > 0)
            {
                var algor = list[0];
                var newInstance = a.CreateInstance(algor.ToString());
                IRobotAlgorithm testRobotAlgorithm = (IRobotAlgorithm)newInstance;
                try
                {
                    author = testRobotAlgorithm.Author;
                }
                catch (Exception)
                {
                    throw new Exception("Помилка створення об'єкта алгоритму");
                    return false;
                }
            }
            return true;
        }
    }
}
