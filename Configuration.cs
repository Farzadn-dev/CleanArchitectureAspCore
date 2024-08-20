using System.Diagnostics;

namespace CleanAspCore
{
    public class Configuration
    {
        public string AppName { get; init; } = string.Empty;
        public bool UseDocker { get; init; } = false;//not ready for now
        public bool UseEFCore { get; init; } = false;
        public bool UseSwagger { get; init; } = false;//not ready for now
        public bool UseGit { get; init; } = false;
        public string DbContextName { get; init; } = string.Empty;
        public string DbConnectionString { get; init; } = string.Empty;//not ready for now
        public string OutPath { get; init; } = string.Empty;

        public Configuration(Dictionary<ArgType, string> dictionary)
        {
            if (!dictionary.ContainsKey(ArgType.AppName))
                ArgumentParser.ShowHelp();
            AppName = dictionary[ArgType.AppName];

            if (dictionary.ContainsKey(ArgType.OutPath))
                OutPath = dictionary[ArgType.OutPath];

            if (dictionary.ContainsKey(ArgType.UseDocker))
                UseDocker = bool.Parse(dictionary[ArgType.UseDocker]);

            if (dictionary.ContainsKey(ArgType.UseEFCore))
                UseEFCore = bool.Parse(dictionary[ArgType.UseEFCore]);

            if (dictionary.ContainsKey(ArgType.UseSwagger))
                UseSwagger = bool.Parse(dictionary[ArgType.UseSwagger]);

            if (dictionary.ContainsKey(ArgType.UseGit))
                UseGit = bool.Parse(dictionary[ArgType.UseGit]);

            if (dictionary.ContainsKey(ArgType.DbContextName))
                DbContextName = dictionary[ArgType.DbContextName];

            if (dictionary.ContainsKey(ArgType.DbConnectionString))
                DbConnectionString = dictionary[ArgType.DbConnectionString];
        }
        public static void CreateClassLibrary(string Name, string path, string srcPath, Configuration config)
        {
            Console.WriteLine($"Creating {Name} Class Library...");
            var p = CreateProcess(path, $"dotnet new classlib -n {config.AppName}.{Name}");
            p.WaitForExit();

            Console.WriteLine($"Deleting Class.cs file...");
            p = CreateProcess($"{path}\\{config.AppName}.{Name}", $"del Class1.cs");

            Console.WriteLine($"Adding {Name} To sln file...");
            p = CreateProcess(srcPath, $"dotnet sln {config.AppName}.sln add {path.Split('\\')[^1]}\\{config.AppName}.{Name}\\{config.AppName}.{Name}.csproj");
        }
        public static Process CreateProcess(string Path, string command)
        {
            // Create a new process start info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe", // Use cmd.exe to run the command
                Arguments = "/C " + command, // /C tells cmd.exe to run the command and then terminate
                RedirectStandardOutput = true, // Redirect the output
                UseShellExecute = false, // Do not use the shell to execute the command
                CreateNoWindow = true, // Do not create a new window
                WorkingDirectory = Path // Set the working directory to the specified path
            };


            // Create and start the process
            Process process = new Process { StartInfo = startInfo };
            // Wait for the process to exit
            process.Start();

            var temp = process.StandardOutput.ReadToEnd();
            return process;
        }
    }
}
