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
    class DefinitionProvider : IDefinitionHandler
    {
        ILanguageServer router;
        BufferManager bufferManager;

        public DefinitionProvider(ILanguageServer router, BufferManager bufferManager)
        {
            this.router = router;
            this.bufferManager = bufferManager;
        }

        public DefinitionRegistrationOptions GetRegistrationOptions()
        {
            return new DefinitionRegistrationOptions();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken)
        //#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {

            List<LocationOrLocationLink> items = new List<LocationOrLocationLink>();

            (var code, var lines, var tree) = bufferManager.Get(request.TextDocument.Uri.ToString());
            int c = request.Position.Character;
            int l = request.Position.Line;

            router.Window.LogInfo("definition requested");
            if (lines == null || tree == null) return items;

            string token = lines[l][c].ToString();

            if (lines[l][c].ToString().IsIdentifier())
            {
                for (int i = c + 1; i < lines[l].Length && lines[l][i].ToString().IsIdentifier(); i++)
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
                return items;
            }
            if (token.IsKeyword()) return items;


            //Context startOfSearch = FindContextAtLine(l, tree);
            //if (startOfSearch == null) return items;

            var def = FindDefinition(token, tree);
            if (def == null) return items;

            var start = DocumentHighlightHandler.GetPosition(code, def.Characters.Start.Value);
            var end = DocumentHighlightHandler.GetPosition(code, def.Characters.End.Value);

            router.Window.LogInfo("definition provided " + def);

            return new LocationOrLocationLinks(new Location() { Range = new Range(start, end), Uri = request.TextDocument.Uri});
        }

        public static Context FindDefinition(string token, Context root)
        {
            if(root is FunctionHeaderContext f)
            {
                if (f.Name == token) return root;
            }
            else if(root is VariableDefinitionContext v)
            {
                if (v.Name == token) return root;
            }
            else if (root is VariableOrIdentifierDefinitionContext va)
            {
                if (va.IsVariable && va.Name == token) return root;
            }
            else if(root is ClassHeaderContext c)
            {
                if (c.Name == token) return root;
            }

            foreach(var child in root.Children)
            {
                var d = FindDefinition(token, child);
                if (d != null) return d;
            }

            return null;
        }

        //public static Context FindContextAtLine(int line, Context root)
        //{
        //    if(root.Lines.Start.Value <= line && root.Lines.End.Value >= line)
        //    {
        //        return root;
        //    }
        //    foreach(var child in root.Children)
        //    {
        //        var c = FindContextAtLine(line, child);
        //        if (c != null) return c;
        //    }
        //    return null;
        ////}

        //public static Context FindDefinition(Context startOfSearch)
        //{
        //    if(startof)
            
        //    return FindDefinition(startOfSearch.Parent);
        //}

        public void SetCapability(DefinitionCapability capability)
        {

        }
    }
}
