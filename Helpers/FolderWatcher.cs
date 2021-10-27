using System.IO;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace LogAnalyzer.Helpers
{
    public interface IFolderWatcher
    {
        void WatchFolder();
    }
    public class FolderWatcher : BackgroundService, IFolderWatcher
    {
        public IServiceProvider Services { get; }

        readonly string path = ".\\Logs";

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (Directory.Exists(path))
            {
                CheckFolder();
                WatchFolder();
            }
            else
                throw new DirectoryNotFoundException(String.Format("Directory {0} not found", path));
            return Task.CompletedTask;
        }

        public FolderWatcher(IServiceProvider services)
        {
            Services = services;
        }
        //check existing files in directory
        async public void CheckFolder()
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
                await CheckLogFile(Path.GetFileName(file));
        }

        //check for new files dropped in directory.
        public void WatchFolder()
        {
            if (Directory.Exists(path))
            {
                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher
                {
                    Path = path
                };
                var handler = new FileSystemEventHandler((s, e) => FileSystemWatcher_Created(s, e));
                fileSystemWatcher.Created += handler;
                fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        async void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            await CheckLogFile(e.Name);
        }

        async Task CheckLogFile(string file)
        {
            using var scope = Services.CreateScope();
            var logParser = scope.ServiceProvider.GetRequiredService<ILogParser>();
            await logParser.ParseLog(path, file);
        }
    }
}
