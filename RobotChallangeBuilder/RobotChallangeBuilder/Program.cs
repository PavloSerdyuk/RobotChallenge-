using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RobotChallangeBuilder
{
    class Program
    {
        
        static List<string> sources;
        
        static void Main(string[] args)
        {
            var ioHelper = new IOHelper();
            sources = ioHelper.Load();



            for (int counter = 0; counter < sources.Count; counter++)
            {
                var source = sources[counter];
                Process.Start("git", GetGitCloneCommand(source, counter));

                if (!ioHelper.GitLoadSuceed(counter))
                {
                    Console.WriteLine("Student {0}. FAILED TO EXTRACT SOURCES: {1}", counter, source);
                    continue;
                }

                
                if (! ioHelper.FolderStructureIsCorrect(counter))
                {
                    Console.WriteLine("Student {0}. Wrong folder structure:{1}", counter, source);
                    continue;
                }

                try
                {
                    if (! ioHelper.ReflectionScanner(GetRootFolder(counter)))
                    {
                        Console.WriteLine("Student {0}. FOUND FORBIDDEN WORDS in: {1}", counter, source);
                        continue;
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine("Student {0}. FOUND BUG: {1}", counter, source);
                    Console.WriteLine(e.Message );
                }

                
                if (ioHelper.GetSourceFile(counter)!= null)
                {
                    var path = Path.GetFullPath(ioHelper.GetSourceFile(counter));
                    var folder = path.Substring(0, path.LastIndexOf('\\') + 1);

                    new RobotCompiler().Compile(folder, counter);
                }
            }
        }

        static string GetRootFolder(int counter)
        {
            return string.Format(@"Source{0}", counter);
        }

        static string GetSourceFolder(int counter)
        {
            return string.Format(@"Source{0}\KPZ\lab1", counter);
        }

        static string GetGitCloneCommand(string line, int counter)
        {
            var find = "bitbucket.org";
            string local = line.Substring(line.IndexOf(find) + find.Length);
            return string.Format("clone https://pavlo_serdyuk:g0gle123@bitbucket.org{0} Source{1}", local, counter);
        }


    }
}
