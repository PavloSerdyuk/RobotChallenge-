using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RobotChallangeBuilder
{
    class IOHelper
    {
        const string GitSources = @"RobotChallangeBuilder.Group.txt";


        internal List<string> Load()
        {
            var sources = new List<string>();
            int counter = 0;
            string line;

            var assembly = Assembly.GetExecutingAssembly();
            // Read the file and display it line by line.
            System.IO.StreamReader file =
                new System.IO.StreamReader(assembly.GetManifestResourceStream(GitSources));

            while ((line = file.ReadLine()) != null)
            {
                sources.Add(line);
            }
            file.Close();

            return sources;
        }

        internal bool GitLoadSuceed(int counter)
        {
            return Directory.Exists(string.Format(@"Source{0}", counter));
        }

        internal bool FolderStructureIsCorrect(int counter)
        {
            return !string.IsNullOrEmpty(GetRootFolder(counter));
        }

        static string GetRootFolder(int counter)
        {
            return string.Format(@"Source{0}", counter);
        }

        internal string GetLabFolder(int counter)
        {
            var path1 = string.Format(@"Source{0}\KPZ\lab1", counter);
            if (Directory.Exists(path1)) return path1;

            var path2 = string.Format(@"Source{0}\KPZ\Lab1", counter);
            if (Directory.Exists(path2)) return path2;

            return null;
        }

        internal string GetSourceFile(int counter)
        {
            var path =  string.Format(@"Source{0}", counter);

            string[] files = Directory.GetFiles(path, "*ithm.cs", SearchOption.AllDirectories);

            if (files.Length == 1) return files[0];

            if (files.Length == 0) files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

            foreach (var s in files)
            {
                {
                    string[] file = File.ReadAllLines(s);

                    foreach (string line in file)
                    {
                        if (line.Contains(": IRobotAlgorithm") || line.Contains("public string Description"))

                        {
                            return s;
                        }
                    }
                }
            }

            return null;
        }

        internal bool ReflectionScanner(string folder)
        {
            string[] files = Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories);
            foreach (var s in files)
            {
                if (!s.Contains("AssemblyInfo.cs"))
                {
                    string[] file = File.ReadAllLines(s);
                    foreach (string line in file)
                    {
                        if (line.Contains("System.Reflection") || line.Contains(".Reflection.")
                            || line.Contains("unsafe"))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }


    }
}
