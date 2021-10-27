using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IISLogParser;
using System.Net;
using System.Net.Sockets;
using LogAnalyzer.Entities;
using LogAnalyzer.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace LogAnalyzer.Helpers
{
    public interface ILogParser
    {
        abstract Task ParseLog(string path, string file);
    }

    public class LogParser : ILogParser
    {
        private static int _iPIndex;
        private static DataContext _context;

        public LogParser(DataContext context)
        {
            _context = context;
        }
       
        //Initialize parsing process
        public async Task ParseLog(string path, string file)
        {
            if (!CheckForExistingLogFile(file))
            { 
                AddLogFile(file);
                await ReadLogFile(path + "\\" + file);
            }
        }

        //check if present file has already been scanned
        private static bool CheckForExistingLogFile(string file)
        {
            return _context.LogFiles.Any(l => l.FileName == file);
        }

        //add logfile to db
        private static void AddLogFile(string file)
        {
            LogFile logfile = new LogFile()
            {
                FileName = file
            };
            _context.LogFiles.Add(logfile);
            _context.SaveChanges();
        }

        //parse 
        private static async Task ReadLogFile(string file)
        {
            //Temporary list containing all ips found in file
            List<String> ipList = new List<String>();
            String line;
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            using (StreamReader reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream)
                {
                    string ipAddress;
                    string[] splitLine;
                    line = await reader.ReadLineAsync();
                    if (line != null)
                    {
                        if (line.StartsWith('#'))
                            //Check for position of client ip in line
                            CheckHeaderLine(line);
                        else
                        {
                            //extract ip out of line
                            splitLine = line.Split(' ');
                            ipAddress = splitLine[_iPIndex];
                            //probably redundant, add ip to list if present in log
                            if (ipAddress != "-")
                                ipList.Add(ipAddress);
                        }
                    }
                    else
                        break;
                }
                fs.Close();
                fs.Dispose();
            }
            List<String> uniqueIPs = GetUniqueIPs(ipList);
            List<LogEntry> logEntries = GetVisits(uniqueIPs, ipList);
            foreach (LogEntry logEntry in logEntries)
                logEntry.ClientFQDN = LookupFQDN(logEntry.ClientIP);

            UpdateLogEntriesInDataContext(logEntries);
        }


        private static List<String> GetUniqueIPs(List<String> ipList)
        {
            //flatten list to unique values
            return ipList.Select(i => i).Distinct().ToList();
        }

        private static List<LogEntry> GetVisits(List<String> uniqueIPs, List<String> ipList)
        {

            List<LogEntry> logEntries = new List<LogEntry>();
            foreach (string ip in uniqueIPs)
            {
                LogEntry logEntry = new LogEntry()
                {
                    ClientIP = ip,
                    Visits = ipList.Count(i => i == ip)
                };
                logEntries.Add(logEntry);
            }
            return logEntries;
        }

        private static void CheckHeaderLine(string line)
        {
            //if correct header line, extract position of client ip
            string[] splitLine;
            if(line.StartsWith("#Fields"))
            {
                splitLine = line.Split(' ');
                _iPIndex = Array.FindIndex(splitLine, s => s == "c-ip") - 1;
            }
        }

        public static void UpdateLogEntriesInDataContext(List<LogEntry> logEntries)
        {
            foreach(LogEntry logEntry in logEntries)
            {
                //check if record for combination of ip and fqdn exists in db, combination required for dynamic ips
                bool exists = _context.LogEntries.Any(l => l.ClientIP == logEntry.ClientIP &&
                l.ClientFQDN == logEntry.ClientFQDN);
                if (exists)
                {
                    LogEntry dbEntry = _context.LogEntries.Single(l => l.ClientIP == logEntry.ClientIP &&
                    l.ClientFQDN == logEntry.ClientFQDN);
                    //add visits from logfile to visits already in db
                    dbEntry.Visits = dbEntry.Visits + logEntry.Visits;
                    _context.LogEntries.Update(dbEntry);
                    _context.SaveChanges();
                }
                else
                {
                    //if no record exists, create one
                    _context.LogEntries.Add(logEntry);
                    _context.SaveChanges();
                }
            }
        }

        public static string LookupFQDN(string ip)
        {
            //default value
            string fullyQualifiedDomainName = "N/A";
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ip);
                if(entry != null)
                {
                    fullyQualifiedDomainName = entry.HostName;
                }
            }
            catch (SocketException)
            {
                //TODO
            }
            return fullyQualifiedDomainName;
        }
    }
}
