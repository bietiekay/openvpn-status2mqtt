﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace openvpnstatus2mqtt
{
    /// <summary>
    /// This Class stores a number of Console Output Lines into a ring buffer
    /// </summary>
    public class ConsoleOutputLogger
    {
        public String LogfileDirectory = "";
        public String LogfilePrefixName = "";
        private int Max_Number_Of_Entries = 500;
        private LinkedList<String> LoggerList = new LinkedList<String>();
        public bool verbose = false;
        public bool writeLogfile = false;
        private DateTime LastWrite = DateTime.MinValue;
        private StreamWriter Logfile = null;

        public ConsoleOutputLogger(String _LogDir, String _LogFilenamePrefix)
        {
            LogfileDirectory = _LogDir;
            LogfilePrefixName = _LogFilenamePrefix;
        }

        public void SetNumberOfMaxEntries(int Number)
        {
            // TODO: It would be nice to keep at least the Number of Lines we're setting
            lock (LoggerList)
            {
                LoggerList.Clear();
            }
            Max_Number_Of_Entries = Number;
        }

        public int GetMaxNumberOfEntries()
        {
            return Max_Number_Of_Entries;
        }

        private bool lastEntryYesterday()
        {
            if (DateTime.Now.Day != LastWrite.Day)
                return true;
            else
                return false;
        }

        private String GenerateLogFilename()
        {
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            // Change culture to en-US.
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            String LogFilename = DateTime.Now.ToShortDateString() + LogfilePrefixName + ".log";
            // Restore original culture.
            Thread.CurrentThread.CurrentCulture = originalCulture;

            return LogFilename;
        }

        public void LogToFile(String text)
        {
            lock (LoggerList)
            {
                if (Logfile == null)
                {
                    WriteLineToScreenOnly("Opening Logfile: " + LogfileDirectory + Path.DirectorySeparatorChar + GenerateLogFilename());
                    Logfile = new StreamWriter(LogfileDirectory + Path.DirectorySeparatorChar + GenerateLogFilename(), true);
                    Logfile.AutoFlush = true;
                }

                if (LastWrite == DateTime.MinValue)
                    LastWrite = DateTime.Now;

                // check if the day changed, if it did, we start a new log-file
                if (lastEntryYesterday())
                {
                    // when the logfile is open already, we close it
                    if (Logfile != null)
                        Logfile.Close();
                    // now we reopen/create the new logfile for this day...
                    Logfile = new StreamWriter(LogfileDirectory + Path.DirectorySeparatorChar + GenerateLogFilename(), true);
                    Logfile.AutoFlush = true;
                }

                lock (Logfile)
                {
                    Logfile.WriteLine(text);
                }
            }
        }

        public void WriteLine(String text)
        {
            DateTime TimeDate = DateTime.Now;
            text = TimeDate.ToShortDateString() + " - " + TimeDate.ToShortTimeString() + " " + text;

            // write it to the console
            if (verbose) Console.WriteLine(text);
            if (writeLogfile) LogToFile(text);

            LastWrite = TimeDate;
            lock (LoggerList)
            {
                if (LoggerList.Count == Max_Number_Of_Entries)
                {
                    LoggerList.RemoveFirst();
                }

                LoggerList.AddLast(text);
            }
        }

        public void WriteLineToScreenOnly(String text)
        {
            DateTime TimeDate = DateTime.Now;

            text = TimeDate.ToShortDateString() + " - " + TimeDate.ToShortTimeString() + " " + text;
            // write it to the console
            if (verbose) Console.WriteLine(text);
        }

        public String[] GetLoggedLines()
        {
            String[] Output = new String[Max_Number_Of_Entries];
            int Current_Position = 0;

            lock (LoggerList)
            {
                foreach (String line in LoggerList)
                {
                    Output[Current_Position] = line;
                    Current_Position++;
                }
            }
            return Output;
        }
    }
}

