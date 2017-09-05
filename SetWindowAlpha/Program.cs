using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace SetWindowAlpha
{
   
    class Program
    {
        
        private static void Usage(string fileName)
        {
            var spaces = new string(' ', 4);
            if (fileName != string.Empty)
            {
                if (fileName.EndsWith("_"))
                {
                    fileName = fileName.Substring(0, fileName.Length - 1);
                }
            }
            Console.WriteLine("{0}设置窗体程序透明度。{1}", Environment.NewLine, Environment.NewLine);
            Console.WriteLine("{0} Alpha", fileName);
            Console.WriteLine("{0} /n Alpha{1}", fileName, Environment.NewLine);
            Console.WriteLine("{0}/n         第n个可视化窗体程序（缺省默认为主控制台窗口程序）。", spaces);
            Console.WriteLine("{0}Alpha      透明度有效值(0-255)。", spaces);
            Console.WriteLine("{0}{1}注：当参数为空时，默认显示可视化窗体程序列表。", Environment.NewLine, spaces);
        }

        static int Main(string[] args)
        {
            try
            {
                if (args.Length < 3)
                {
                    if (args.Length == 0)
                    {
                        var num = 0;
                        Console.WriteLine("{0} {1} {2}","Index".PadRight(8), "ProcessName".PadRight(25), "MainWindowTitle");
                        Console.WriteLine("{0} {1} {2}",string.Empty.PadRight(8,'='), string.Empty.PadRight(25, '='), string.Empty.PadRight(25, '='));
                        foreach (var item in Process.GetProcesses().Where(m => m.MainWindowHandle != IntPtr.Zero).OrderBy(m => m.ProcessName))
                        {
                            
                            num++;
                            var str = num < 10 ? $"[0{num}]" : $"[{num}]";
                            var name = item.ProcessName;
                            if (name.Length > 25)
                            {
                                name = name.Substring(0, 25);
                            }
                            Console.WriteLine("{0} {1} {2}", str.PadRight(8), name.PadRight(25) ,item.MainWindowTitle);
                        }
                    }
                    else if (args.Any(m => m.Contains("/?")))
                    {
                        var fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;
                        fileName = fileName.Substring(0, fileName.LastIndexOf('.')).ToUpper();
                        Usage(fileName);
                    }
                    else
                    {
                        var alphaVal = args.Length == 1 ? args[0] : args[1];
                        byte alpha;
                        if (byte.TryParse(alphaVal, out alpha))
                        {
                            var hwnd = IntPtr.Zero;
                            if (args.Length == 1)
                            {
                                hwnd = Extentions.GetConsoleWindow();
                            }
                            else
                            {
                                var procs = Process.GetProcesses().Where(m => m.MainWindowHandle != IntPtr.Zero).OrderBy(m => m.ProcessName).ToArray();//获取可视化窗口程序句柄
                                if (Regex.IsMatch(args[0], "^/[1-9][0-9]*$"))
                                {
                                    int index;
                                    if (int.TryParse(args[0].Substring(1), out index))
                                    {
                                        index--;
                                        if (index >= 0 && index < procs.Length)
                                        {
                                            hwnd = procs[index].MainWindowHandle;
                                        }
                                        else
                                        {
                                            Console.WriteLine("索引超出范围。");
                                            return 0;
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("命令语法不正确。");
                                    return 1;
                                }
                            }
                            if (hwnd != IntPtr.Zero)
                            {
                                if (!hwnd.SetWindowAlpha(alpha))
                                {
                                    Console.WriteLine("无法设置指定程序透明度!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("不存在指定程序或无法获取程序的句柄。");
                            }
                        }
                        else
                        {
                            Console.WriteLine("不是有效的Alpha值:{0}", alphaVal);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("命令语法不正确。");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 1;
            }
            return 0;
        }
    }
}
