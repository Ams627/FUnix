using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FUnix
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var files = FindFiles(Directory.GetCurrentDirectory(), "*");
                var extensionsToCheck = new HashSet<string> {"cs", "cpp", "h", "sql", "xml", "csproj", "sln", "sqlproj" };
                foreach (var filename in files)
                {
                    var ext = Path.GetExtension(filename);
                    if (ext.Any())
                    {
                        var extWithoutDot = ext.Substring(1);
                        if (extensionsToCheck.Contains(extWithoutDot))
                        {
                            var arr = File.ReadAllBytes(filename);
                            if (IsUnix(arr))
                            {
                                Console.WriteLine($"{filename}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine($"{progname} Error: {ex.Message}");
            }
        }

        private static bool IsUnix(byte[] arr)
        {
            bool waitingForLf = false;

            foreach (var b in arr)
            {
                if (waitingForLf)
                {
                    if (b == 10)
                    {
                        waitingForLf = false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (b == 13)
                {
                    waitingForLf = true;
                }
                else if (b == 10)
                {
                    return true;
                }
            }
            return waitingForLf;
        }

        static List<string> FindFiles(string startingDir, string pattern)
        {
            var result = new List<string>();
            var stack = new Stack<string>();
            stack.Push(startingDir);

            while (stack.Any())
            {
                var dir = stack.Pop();
                var files = Directory.GetFiles(dir, pattern);
                result.AddRange(files);
                foreach (var subDir in Directory.GetDirectories(dir))
                {
                    stack.Push(subDir);
                }
            }
            return result;
        }
    }
}
