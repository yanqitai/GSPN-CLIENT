using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.Helpers
{
    public static class MyExtensions
    {
        public static IEnumerable<TResult> ZipThree<T1, T2, T3, TResult>(
            this IEnumerable<T1> source,
            IEnumerable<T2> second,
            IEnumerable<T3> third,
            Func<T1, T2, T3, TResult> func)
        {
            using (var e1 = source.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            using (var e3 = third.GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
                    yield return func(e1.Current, e2.Current, e3.Current);
            }
        }

        public static async Task<Task> WaitForAnyNonFaultedTaskAsync(IEnumerable<Task> tasks)
        {
            IList<Task> customTasks = tasks.ToList();
            Task completedTask;
            do
            {
                completedTask = await Task.WhenAny(customTasks);
                customTasks.Remove(completedTask);
            } while (completedTask.IsFaulted && customTasks.Count > 0);

            return completedTask.IsFaulted ? null : completedTask;
        }
    }

    public static class ExpandoObjectExtension
    {
        public static bool IsPropertyExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }
    }

    public static class ForEachAsyncExtension
    {
        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            foreach (var value in list)
            {
                await func(value);
            }
        }
    }

    public static class SerialPortExtensions
    {
        public static async Task<string> SerialReadLineAsync(this SerialPort serialPort)
        {
            try
            {
                int quedanPorLeer = serialPort.BytesToRead;
                byte[] buffer = new byte[quedanPorLeer];
                string result = string.Empty;
                //Debug.WriteLine("Let's start reading.");

                CancellationTokenSource cts = new CancellationTokenSource(new TimeSpan(0, 0, 0, 0, 333));

                while (quedanPorLeer > 0)
                {
                    int leidos = await serialPort.BaseStream.ReadAsync(buffer, 0, quedanPorLeer, cts.Token).ConfigureAwait(false);
                    result += serialPort.Encoding.GetString(buffer);

                    if (result.EndsWith(serialPort.NewLine))
                    {
                        result = result.Substring(0, result.Length - serialPort.NewLine.Length);
                        result.TrimEnd('\r', '\n');
                        //Debug.Write(string.Format("Data: {0}", result));
                        result += "\r\n";
                        return result;
                    }
                    quedanPorLeer -= leidos;
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
