using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace rLangLSP
{
    static class Diagnostics
    {
        public static Container<Diagnostic> GetDiagnostics(string URI, ILanguageServer router, BufferManager bufferManager)
        {
            var code = bufferManager.GetCode(URI);
            var errors = bufferManager.GetErrors(URI);
            if(errors == default) return new Container<Diagnostic>();

            router.Window.LogInfo($"diagnosing + {errors.Count}");
            int i = 0;
            return errors.Select((e) =>
            {
                i++;
                var start = DocumentHighlightHandler.GetPosition(code, e.Item1.Start);
                var end = DocumentHighlightHandler.GetPosition(code, e.Item1.End);
                return new Diagnostic()
                {
                    Message = e.Item1.Message,
                    Code = i,
                    Severity = DiagnosticSeverity.Error,
                    Range = new Range(start, end)
                };
            }).ToArray();
        }
    }
}
