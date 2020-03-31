using DinkToPdf.Contracts;
using Microsoft.DotNet.PlatformAbstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;


namespace DinkToPdf.TestThreadSafe
{
    public class Program
    {
        static SynchronizedConverter _converter;

        public static void Main(string[] args)
        {
            Console.WriteLine(
                $"RID: {RuntimeEnvironment.GetRuntimeIdentifier()}, " +
                $"OS: {RuntimeEnvironment.OperatingSystem}, " +
                $"OSPlatform: {RuntimeEnvironment.OperatingSystemPlatform}, " +
                $"OSVersion: {RuntimeEnvironment.OperatingSystemVersion}, " +
                $"Architecture: {RuntimeEnvironment.RuntimeArchitecture}, " +
                $"Framework: {Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName}"
            );

            if (!Directory.Exists("Files"))
            {
                Directory.CreateDirectory("Files");
            }

            _converter = new SynchronizedConverter(new PdfTools());

            _converter.PhaseChanged += Converter_PhaseChanged;
            _converter.ProgressChanged += Converter_ProgressChanged;
            _converter.Finished += Converter_Finished;
            _converter.Warning += Converter_Warning;
            _converter.Error += Converter_Error;

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Grayscale,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10 },
                },
                Objects = {
                    new ObjectSettings() {
                        Page = "https://www.color-hex.com/"
                    }
                }
            };

            var task = Task.Run(() => Action(doc));

            var doc2 = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4Small
                },

                Objects = {
                    new ObjectSettings()
                    {
                        Page = "https://google.com/"
                    }
                }
            };

            var task2 = Task.Run(() => Action(doc2));

            Task.WaitAll(task, task2);

            if (!Debugger.IsAttached)
            {
                Console.ReadKey();
            }
        }

        private static void Action(IDocument doc)
        {
            byte[] pdf = _converter.Convert(doc);

            using FileStream stream = new FileStream(Path.Combine("Files", DateTime.UtcNow.Ticks.ToString() + ".pdf"), FileMode.Create);
            stream.Write(pdf, 0, pdf.Length);
        }

        private static void Converter_Error(object sender, EventDefinitions.ErrorArgs e)
        {
            Console.WriteLine("[ERROR] {0}", e.Message);
        }

        private static void Converter_Warning(object sender, EventDefinitions.WarningArgs e)
        {
            Console.WriteLine("[WARN] {0}", e.Message);
        }

        private static void Converter_Finished(object sender, EventDefinitions.FinishedArgs e)
        {
            Console.WriteLine("Conversion {0} ", e.Success ? "successful" : "unsucessful");
        }

        private static void Converter_ProgressChanged(object sender, EventDefinitions.ProgressChangedArgs e)
        {
            Console.WriteLine("Progress changed {0}", e.Description);
        }

        private static void Converter_PhaseChanged(object sender, EventDefinitions.PhaseChangedArgs e)
        {
            Console.WriteLine("Phase changed {0} - {1}", e.CurrentPhase, e.Description);
        }
    }
}
