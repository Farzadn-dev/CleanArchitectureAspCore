using CleanAspCore;
var argsList = args.Parse();

var config = new Configuration(argsList);

#region Creating Base Folders

string basePath = string.Empty;
if (config.OutPath != string.Empty)
    basePath = config.OutPath + $"\\{config.AppName}";
else
    basePath = Environment.CurrentDirectory + $"\\{config.AppName}";

var srcPath = basePath + "\\Src";
var commonPath = srcPath + "\\Common";
var corePath = srcPath + "\\Core";
var infrastructurePath = srcPath + "\\Infrastructure";
var presentationPath = srcPath + "\\Presentation";

Console.WriteLine("Creating Base Folders...");
if (config.UseGit)
{
    Directory.CreateDirectory(basePath);
    Configuration.CreateProcess(basePath, $"git init");
}
Directory.CreateDirectory(srcPath);
Directory.CreateDirectory(commonPath);
Directory.CreateDirectory(corePath);
Directory.CreateDirectory(infrastructurePath);
Directory.CreateDirectory(presentationPath);
#endregion

/*
dotnet new classlib -n MyClassLibrary
cd MyClassLibrary
dotnet restore
dotnet build


cmd /k cd /d C:\Path\To\Your\Directory && your_command



# Navigate to your project directory
cd path/to/your/classlibrary

# Install the Newtonsoft.Json package
dotnet add package Newtonsoft.Json



cd path\to\your\directory
dotnet new mvc -n YourProjectName
cd YourProjectName
dotnet run



dotnet add reference ../MyClassLibrary/MyClassLibrary.csproj
*/

///////////////////////////
#region Creating Base Projects
Configuration.CreateProcess(srcPath, $"dotnet new sln -n {config.AppName}");

Configuration.CreateClassLibrary("Common", commonPath, srcPath, config);
Configuration.CreateClassLibrary("Application", corePath, srcPath, config);
Configuration.CreateClassLibrary("Domain", corePath, srcPath, config);
Configuration.CreateClassLibrary("Persistence", infrastructurePath, srcPath, config);

Console.WriteLine("Creating EndPoint...");
var p = Configuration.CreateProcess(presentationPath, $"dotnet new mvc -n {config.AppName}.EndPoint");
p.WaitForExit();
Console.WriteLine($"Adding EndPoint To sln file...");
p = Configuration.CreateProcess(srcPath, $"dotnet sln {config.AppName}.sln add Presentation\\{config.AppName}.EndPoint\\{config.AppName}.EndPoint.csproj");
#endregion

#region Creating Necessary Folders
Console.WriteLine("Create necessary folders in Persistence...");
Directory.CreateDirectory(infrastructurePath + $"\\{config.AppName}.Persistence\\Context");

Console.WriteLine("Create necessary folders in Application...");
Directory.CreateDirectory(corePath + $"\\{config.AppName}.Application\\Context");
Directory.CreateDirectory(corePath + $"\\{config.AppName}.Application\\Services");
Directory.CreateDirectory(corePath + $"\\{config.AppName}.Application\\Common");
#endregion

#region Adding References
Console.WriteLine("Add References To Application...");
Configuration.CreateProcess($"{corePath}\\{config.AppName}.Application", $"dotnet add reference ..\\{config.AppName}.Domain\\{config.AppName}.Domain.csproj");
Configuration.CreateProcess($"{corePath}\\{config.AppName}.Application", $"dotnet add reference ..\\..\\Common\\{config.AppName}.Common\\{config.AppName}.Common.csproj");

Console.WriteLine("Add References To Persistence...");
Configuration.CreateProcess($"{infrastructurePath}\\{config.AppName}.Persistence ", $"dotnet add reference ..\\..\\Core\\{config.AppName}.Domain\\{config.AppName}.Domain.csproj");
Configuration.CreateProcess($"{infrastructurePath}\\{config.AppName}.Persistence ", $"dotnet add reference ..\\..\\Common\\{config.AppName}.Common\\{config.AppName}.Common.csproj");
Configuration.CreateProcess($"{infrastructurePath}\\{config.AppName}.Persistence ", $"dotnet add reference ..\\..\\Core\\{config.AppName}.Application\\{config.AppName}.Application.csproj");

Console.WriteLine("Add References To EndPoint...");
Configuration.CreateProcess($"{presentationPath}\\{config.AppName}.EndPoint ", $"dotnet add reference ..\\..\\Infrastructure\\{config.AppName}.Persistence\\{config.AppName}.Persistence.csproj");
#endregion

//////////////////////////

if (config.UseEFCore)
{
    #region EndPoint
    Console.WriteLine($"Add EfCore To {config.AppName}.EndPoint Library...");
    var p2 = Configuration.CreateProcess(presentationPath + $"\\{config.AppName}.EndPoint", "dotnet add package Microsoft.EntityFrameworkCore");
    p2.WaitForExit();

    Console.WriteLine($"Add EfCore.Design To {config.AppName}.EndPoint Library...");
    p2 = Configuration.CreateProcess(presentationPath + $"\\{config.AppName}.EndPoint", "dotnet add package Microsoft.EntityFrameworkCore.Design");
    p2.WaitForExit();

    Console.WriteLine($"Add EfCore.Sqlserver To {config.AppName}.EndPoint Library...");
    p2 = Configuration.CreateProcess(presentationPath + $"\\{config.AppName}.EndPoint", "dotnet add package Microsoft.EntityFrameworkCore.SqlServer");
    p2.WaitForExit();
    #endregion

    #region Application
    Console.WriteLine($"Add EfCore To {config.AppName}.Application Library...");
    p2 = Configuration.CreateProcess(corePath + $"\\{config.AppName}.Application", "dotnet add package Microsoft.EntityFrameworkCore");
    p2.WaitForExit();

    Console.WriteLine($"Add EfCore.Design To {config.AppName}.Application Library...");
    p2 = Configuration.CreateProcess(corePath + $"\\{config.AppName}.Application", "dotnet add package Microsoft.EntityFrameworkCore.Design");
    p2.WaitForExit();
    #endregion

    #region Persistence
    Console.WriteLine($"Add EfCore To {config.AppName}.Persistence Library...");
    p2 = Configuration.CreateProcess(infrastructurePath + $"\\{config.AppName}.Persistence", "dotnet add package Microsoft.EntityFrameworkCore");
    p2.WaitForExit();

    Console.WriteLine($"Add EfCore.Design To {config.AppName}.Persistence Library...");
    p2 = Configuration.CreateProcess(infrastructurePath + $"\\{config.AppName}.Persistence", "dotnet add package Microsoft.EntityFrameworkCore.Design");
    p2.WaitForExit();

    Console.WriteLine($"Add EfCore.Sqlserver To {config.AppName}.Persistence Library...");
    p2 = Configuration.CreateProcess(infrastructurePath + $"\\{config.AppName}.Persistence", "dotnet add package Microsoft.EntityFrameworkCore.SqlServer");
    p2.WaitForExit();

    Console.WriteLine($"Add EfCore.Tools To {config.AppName}.Persistence Library...");
    p2 = Configuration.CreateProcess(infrastructurePath + $"\\{config.AppName}.Persistence", "dotnet add package Microsoft.EntityFrameworkCore.Tools");
    p2.WaitForExit();

    Console.WriteLine($"Add EfCore.Relational To {config.AppName}.Persistence Library...");
    p2 = Configuration.CreateProcess(infrastructurePath + $"\\{config.AppName}.Persistence", "dotnet add package Microsoft.EntityFrameworkCore.Relational");
    p2.WaitForExit();
    #endregion

    //////////////////
    Console.WriteLine("Editing Program.cs To Add EfCore...");
    string text = string.Empty;
    using (var sr = new StreamReader(presentationPath + $"\\{config.AppName}.EndPoint\\Program.cs"))
    {
        string textToReplace = """
                // Add services to the container.
                string ConnectionString = builder.Configuration.GetConnectionString("***") ?? throw new ArgumentNullException("ConnectionString");
                builder.Services.AddEntityFrameworkSqlServer().AddDbContext<***>
                (ops => {
                    ops.UseSqlServer
                    (
                        ConnectionString,
                        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                    );
                    ops.EnableThreadSafetyChecks();
                }
                );
                """.Replace("***", config.AppName + "Context");
        text = sr.ReadToEnd().Replace("// Add services to the container.", textToReplace).Replace("var builder = WebApplication.CreateBuilder(args);", """
            using Microsoft.EntityFrameworkCore;
            using ***.Persistence.Context;

            var builder = WebApplication.CreateBuilder(args);
            """.Replace("***", config.AppName));
    }
    using (var sw = new StreamWriter(presentationPath + $"\\{config.AppName}.EndPoint\\Program.cs"))
    {
        sw.Flush();
        sw.Write(text);
    }

    Console.WriteLine("Create Context Files...");
    using (var sw = new StreamWriter(infrastructurePath + $"\\{config.AppName}.Persistence\\Context\\{config.AppName}Context.cs"))
    {
        text = """
            using Microsoft.EntityFrameworkCore;
            using ***.Application.Context;

            namespace ***.Persistence.Context
            {
                public class ***Context : DbContext, I***Context
                {
                    public ***Context(DbContextOptions options) : base(options)
                    {

                    }
                    //public DbSet<sample> sample { get; set; }

                    protected override void OnModelCreating(ModelBuilder modelBuilder)
                    {           
                        base.OnModelCreating(modelBuilder);
                    }

                }
            }
            """.Replace("***", config.AppName);
        sw.Write(text);
    }
    using (var sw = new StreamWriter(corePath + $"\\{config.AppName}.Application\\Context\\I{config.AppName}Context.cs"))
    {
        text = """
            namespace ***.Application.Context
            {
                public interface I***Context
                {
                    //public DbSet<sample> sample { get; set; }

                    int SaveChanges(bool accepAllChangesOnSuccess);
                    int SaveChanges();
                    Task<int> SaveChangesAsync(bool accepAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken());
                    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
                }
            }
            """.Replace("***", config.AppName);
        sw.Write(text);
    }
}

if (config.DbContextName != string.Empty)
{
    var appSetting = """
                {
          "Logging": {
            "LogLevel": {
              "Default": "Information",
              "Microsoft.AspNetCore": "Warning"
            }
          },
          "AllowedHosts": "*",
          "ConnectionStrings": {
            "***": "Server=.;Initial Catalog=***;Integrated Security=True;TrustServerCertificate=True;"
          }
        }
        
        """.Replace("***", config.DbContextName);
    Console.WriteLine("Editing appsettings.json...");
    using (var sw = new StreamWriter(presentationPath + $"\\{config.AppName}.EndPoint\\appsettings.json"))
    {
        await sw.WriteAsync(appSetting);
    }
    using (var sw = new StreamWriter(presentationPath + $"\\{config.AppName}.EndPoint\\appsettings.Development.json"))
    {
        await sw.WriteAsync(appSetting);
    }
    if (config.UseGit)
    {
        Configuration.CreateProcess(basePath, $"git add .").WaitForExit();
        Configuration.CreateProcess(basePath, $"git commit -m \"Initail Git (Auto Commit from CleanAspCore Application)\"");
    }


    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Done!!");
    Console.ResetColor();
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}
