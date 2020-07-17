using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using RLParser;
using RLParser.Extensions;
using RLParser.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace rLangLSP
{
    class CompletionHandler : ICompletionHandler
    {
        ILanguageServer router;
        BufferManager bufferManager;

        public CompletionHandler(ILanguageServer router, BufferManager bufferManager)
        {
            this.router = router;
            this.bufferManager = bufferManager;
        }

        public CompletionRegistrationOptions GetRegistrationOptions()
        {
            return new CompletionRegistrationOptions();
        }

        public async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            router.Window.LogInfo("completion request");
            (var code, var lines, var tree) = bufferManager.Get(request.TextDocument.Uri.ToString());
            int c = request.Position.Character;
            int l = request.Position.Line;

            if(lines == null) return new CompletionList();

            string token = lines[l][c].ToString();

            int i = GetChar(l, c, code);
            router.Window.LogInfo(i.ToString());

            var context = tree.GetFirstAtChar(i);
            router.Window.LogInfo(context.ToString());

            //creating items
            List<CompletionItem> items = new List<CompletionItem>();

            if (context is FileContext)
            {
                items.Add(CreateKeyword("namespace"));
                items.Add(CreateKeyword("class"));
            }

            if (context is ClassBodyContext)
            {
                items.Add(CreateKeyword("var"));
                items.Add(CreateKeyword("def"));
                items.Add(CreateKeyword("public:\n"));
                items.Add(CreateKeyword("private:\n"));
            }

            return items;
        }

        private CompletionItem CreateKeyword(string v)
        {
            return new CompletionItem()
            {
                Label = v,
                Kind = CompletionItemKind.Keyword,
            };
        }

        private CompletionItem CreateItem(string v, CompletionItemKind kind)
        {
            return new CompletionItem()
            {
                Label = v,
                Kind = kind,
            };
        }

        int GetChar(int l, int c, string code)
        {
            int line = 0;
            int currentC = 0;
            for(int i = 0; i < code.Length; i++)
            {
                if(code[i] == '\n')
                {
                    line++;
                    currentC = 0;
                }
                if (line == l && c == currentC) return i;
                currentC++;
            }
            return -1;
        }

        public void SetCapability(CompletionCapability capability)
        {
            
        }
    }
}
