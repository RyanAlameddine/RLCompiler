using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using RLParser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace rLangLSP
{
    class DocumentHighlightHandler : IDocumentHighlightHandler
    {
        ILanguageServer router;
        BufferManager bufferManager;

        public DocumentHighlightHandler(ILanguageServer router, BufferManager bufferManager)
        {
            this.router = router;
            this.bufferManager = bufferManager;
        }

        public async Task<DocumentHighlightContainer> Handle(DocumentHighlightParams request, CancellationToken cancellationToken)
        {
            (var code, var lines, var tree) = bufferManager.Get(request.TextDocument.Uri.ToString());
            int c = request.Position.Character;
            int l = request.Position.Line;

            if(lines == null) return new DocumentHighlight[0];
            
            string token = lines[l][c].ToString();

            if (lines[l][c].ToString().IsIdentifier())
            {
                for(int i = c + 1; i < lines[l].Length && lines[l][i].ToString().IsIdentifier(); i++)
                {
                    token += lines[l][i];
                }
                for (int i = c - 1; i >= 0 && lines[l][i].ToString().IsIdentifier(); i--)
                {
                    token = lines[l][i] + token;
                }
            }
            else
            {
                return new DocumentHighlight[0];
            }
            if (token.IsKeyword()) return new DocumentHighlight[0];

            List<DocumentHighlight> highlights = new List<DocumentHighlight>();

            for (int lineNumber = 1; lineNumber < lines.Length; lineNumber++)
            {
                var matches = Regex.Matches(lines[lineNumber], token);
                foreach (Match match in matches)
                {
                    highlights.Add(new DocumentHighlight()
                    { Range = new Range(new Position(lineNumber, match.Index), new Position(lineNumber, match.Index + match.Length)) });
                }
            }
            return highlights;
        }

        public static Position GetPosition(string code, int c)
        {
            int line = 0;
            for(int i = 0; i < c; i++)
            {
                if (code[i] == '\n') line++;
            }
            return new Position(line, c);
        }

        public void SetCapability(DocumentHighlightCapability capability)
        {
            
        }

        DocumentHighlightRegistrationOptions IRegistration<DocumentHighlightRegistrationOptions>.GetRegistrationOptions()
        {
            return new DocumentHighlightRegistrationOptions();
        }
    }
}
