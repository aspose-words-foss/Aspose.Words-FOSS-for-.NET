// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.

//
// NOTE This source file is included by link from
// X:/awnet/Aspose.Foundation/Tools/XpsToPng
// X:/awnet/Aspose.Foundation/Tools/WebToPng
//
// DO NOT add using directives for Aspose namespaces
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using Aspose.JavaAttributes;

namespace Aspose.TestFx
{
    /// <summary>
    /// Implements client logic for posting a job to server.
    /// This class is used by server as well since server can act as client.
    /// </summary>
    [JavaManual]
    [AndroidDelete]
    internal static class ClientUtil
    {
        /// <summary>
        /// Implements client logic for posting a job to server
        /// </summary>
        /// <param name="mutex">Server mutex name</param>
        /// <param name="pipe">Server pipe name</param>
        /// <param name="serverPath">Deafult path to server executable</param>
        /// <param name="envVariable">Name of environment variable used to get server path</param>
        /// <param name="parameters">Parameters passed to the server</param>
        /// <returns>Server command output</returns>
        internal static string PostServerJobGetResults(string mutex, string pipe,
            string serverPath, string envVariable, string[] parameters)
        {
            EnsureServerRunning(mutex, Environment.GetEnvironmentVariable(envVariable) ?? serverPath);

            // Send request and get unique pipe name in response where to get results later
            // Use short timeout since it takes little time for the server to schedule the job
            StringBuilder sb = new StringBuilder();
            foreach (var p in parameters)
            {
                if (string.IsNullOrEmpty(p))
                    continue;
                if (sb.Length > 0)
                    sb.Append(' ');
                sb.Append("\"" + FixSlashAtEnd(p) + "\"");
            }

            var result = PipeChat(pipe, sb.ToString(), 10);

            // If server already knows there is an error it will tell us straight away
            // This typically happens where passed arguments are invalid
            if (string.IsNullOrWhiteSpace(result))
                throw new Exception("server replied with empty response");  // we waited for pipe name, but nothing came back
            if (!char.IsLetter(result[0]))  // if there is an error the results will start with non-letter (by design)
                throw new Exception("server replied with error --->\r\n"+ result.Substring(1));

            // Now wait for a response over the dedicated pipe
            // Set longer timeout to allow time for the server to generate results
            result = PipeChat(result, null, 60);

            if (!string.IsNullOrEmpty(result) && result.StartsWith("$", StringComparison.OrdinalIgnoreCase))
                throw new Exception("server replied with error --->\r\n"+ result.Substring(1));

            return result;
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        internal static string[] PostServerJobGetResults(string mutex, string pipe,
            string serverPath, string envVariable, string[][] parameters)
        {
            if (parameters.Length <= 0)
                throw new ArgumentException("parameters");

            if (parameters.Length == 1)
                return new string[] { PostServerJobGetResults(mutex, pipe, serverPath, envVariable, parameters[0]) };

            using (ThreadPool pool = new ThreadPool())
            {
                Dictionary<int, object> dict = new Dictionary<int, object>();
                int workItemIndex = 0;
                foreach (var p in parameters)
                {
                    int wi = workItemIndex;
                    pool.Enqueue(delegate {
                    {
                        try
                        {
                            string result = PostServerJobGetResults(mutex, pipe, serverPath, envVariable, p);
                            Monitor.Enter(dict);
                            dict.Add(wi, result);
                            Monitor.Exit(dict);
                        }
                        catch (Exception e)
                        {
                            Monitor.Enter(dict);
                            dict.Add(wi, e);
                            Monitor.Exit(dict);
                        }
                    }});
                    workItemIndex++;
                }
                pool.WaitForIdle();

                string[] results = new string[dict.Count];
                List<Exception> errors = new List<Exception>();
                foreach (var kvp in dict)
                    if (kvp.Value is Exception)
                        errors.Add((Exception)kvp.Value);
                    else
                        results[kvp.Key] = (string)kvp.Value;

                if (errors.Count > 0)
                    throw new AggregateException(errors.ToArray());

                return results;
            }
        }

        /// <summary>
        /// If I want to pass \ at the end of the path with spaces inside then I need quotes around it
        /// but the windows command line parser will treat it as if I wanted to escape the quote at the end
        /// yet I wanted to include \ at the end and keep the quotes. ugly windows.
        /// </summary>
        private static string FixSlashAtEnd(string v)
        {
            return (v.EndsWith("\\", StringComparison.OrdinalIgnoreCase) == true) ? v + '\\' : v;
        }

        /// <summary>
        /// Opens pipe, optionally writes request to it, and reads response
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        private  static string PipeChat(string pipeName, string request = null, int timeoutSeconds = 10)
        {
            PipeDirection direction = (request == null ? PipeDirection.In : PipeDirection.InOut);
            using(var stream = new NamedPipeClientStream(".", pipeName, direction))
            {
                stream.Connect(timeoutSeconds*1000);

                if (request != null)
                {
                    var writer = new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true);
                    writer.AutoFlush = true;
                    writer.WriteLine(request);
                }

                var reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadLine() ?? string.Empty;
            }
        }

        private static void EnsureServerRunning(string mutexName, string serverPath)
        {
            // check server is running, if not start it
            // if server terminates after we check that it was running, we will not be able to connect to it below
            // if we start it but someone has started another copy just before us then we will connect to the existing process
            const int MAX_ATTEMPTS = 10;
            for (var i = 0; i < MAX_ATTEMPTS; i++)
            {
                bool createdNew;
                using (var mutex = new Mutex(true, mutexName))
                {
                    createdNew = mutex.WaitOne(0);
                    // createdNew is false and mutex not acquired where server is already running
                    if (!createdNew)
                        break;

                    mutex.ReleaseMutex();
                }

                var startInfo = new ProcessStartInfo();
                startInfo.FileName = serverPath;
                startInfo.Arguments = "-s";
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = true;   // we want detached process, we do not want to wait for it to finish
                Process.Start(startInfo);
            }
        }
    }
}
