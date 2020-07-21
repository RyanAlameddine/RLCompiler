using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using RLParser;
using RLParser.Scopes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace rLangLSP
{
    class OnTypeFormattingHandler : IDocumentOnTypeFormattingHandler
    {
        ILanguageServer router;
        BufferManager bufferManager;

        public OnTypeFormattingHandler(ILanguageServer router, BufferManager bufferManager)
        {
            this.router = router;
            this.bufferManager = bufferManager;
        }

        public DocumentOnTypeFormattingRegistrationOptions GetRegistrationOptions()
        {
            return new DocumentOnTypeFormattingRegistrationOptions() { FirstTriggerCharacter = "\n" };
        }

        public async Task<TextEditContainer> Handle(DocumentOnTypeFormattingParams request, CancellationToken cancellationToken)
        {
            (string code, string[] lines, Context tree) = bufferManager.Get(request.TextDocument.Uri.ToString());
            List<TextEdit> edits = new List<TextEdit>();

            int ln = request.Position.Line - 1;
            string line = lines[ln];
            //if (new Regex("^.*{ *(\r\n|\r|\n)$").IsMatch(line))
            //{
            //    router.Window.LogInfo($"\"{line}\"");
            //    if (!new Regex("^ *{").IsMatch(line))
            //    {
            //        router.Window.LogInfo(request.Character + "," + request.Position.Line);
            //        edits.Add(new TextEdit() { NewText = "\n", Range = GetRangeAt(ln, line.LastIndexOf('{')) });
            //    }
            //}



            return edits;
        }

        private Range GetRangeAt(int l, int c)
        {
            var pos = new Position(l, c);
            return new Range(pos, pos);
        }

        public void SetCapability(DocumentOnTypeFormattingCapability capability)
        {

        }
    }
}
