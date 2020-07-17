using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rLangLSP
{
    class CompletionResolveHandler : ICompletionResolveHandler
    {
        ILanguageServer router;
        BufferManager bufferManager;

        public CompletionResolveHandler(ILanguageServer router, BufferManager bufferManager)
        {
            this.router = router;
            this.bufferManager = bufferManager;
        }

        public bool CanResolve(CompletionItem value)
        {
            router.Window.LogInfo("yeetyeet");
            return true;
        }

        public async Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken)
        {
            return request;
        }
    }
}
