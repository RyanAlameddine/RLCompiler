
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using RLParser;
using RLTypeChecker;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace rLangLSP
{
    class BufferManager
    {
        private readonly ConcurrentDictionary<string, string> code = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, string[]> lines = new ConcurrentDictionary<string, string[]>();
        private readonly ConcurrentDictionary<string, SymbolTable> tables = new ConcurrentDictionary<string, SymbolTable>();
        private readonly ConcurrentDictionary<string, Context> trees = new ConcurrentDictionary<string, Context>();

        private ConcurrentDictionary<string, List<(CompileException, Context)>> errors = new ConcurrentDictionary<string, List<(CompileException, Context)>>();


        long typechecker = 0;
        public void UpdateText(string documentPath, string buffer, ILanguageServer router)
        {
            string[] l = buffer.Split('\n');
            lines.AddOrUpdate(documentPath, l, (k, v) => l);
            code.AddOrUpdate(documentPath, buffer, (k, v) => buffer);

            typechecker++;
            CheckErrors(documentPath, buffer, typechecker, router);

            router.Window.LogInfo("updated tree");
        }

        public void CheckErrors(string documentPath, string buffer, long typechecker, ILanguageServer router)
        {
            int erc = 0;
            Action<CompileException, Context> onError = (e, l) =>
            {
                router.Window.LogInfo($"{typechecker}, {this.typechecker}, ID");
                if(typechecker == this.typechecker)
                errors.AddOrUpdate(documentPath, new List<(CompileException, Context)>() { (e, l) }, (k, v) =>
                {
                    v.Add((e, l));
                    erc++;
                    return v;
                });
            };
            try
            {
                Context tree = RLParser.RLParser.Parse(buffer, onError);
                if (!trees.ContainsKey(documentPath))
                {
                    trees.AddOrUpdate(documentPath, tree, (k, v) => tree);
                    tables.AddOrUpdate(documentPath, new SymbolTable(null), (k, v) => new SymbolTable(null));
                }

                ASTNodeMessage[] strings = NodeMessages.GetMessages(tree);
                var s = Newtonsoft.Json.JsonConvert.SerializeObject(strings);
                router.SendNotification("rlang/loadAST", s);


                if (erc == 0)
                {
                    Task.Run(() =>
                    {
                        var t = RlTypeChecker.TypeCheck(tree, onError);
                        if (typechecker == this.typechecker)
                        {
                            router.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams()
                            {
                                Uri = new Uri(documentPath),
                                Diagnostics = Diagnostics.GetDiagnostics (router, code[documentPath], errors[documentPath])
                            });

                            trees.AddOrUpdate(documentPath, tree, (k, v) => tree);
                            tables.AddOrUpdate(documentPath, t, (k, v) => t);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                router.Window.LogWarning(e.ToString());
            }
        }

        public SymbolTable GetTable(string documentPath)
        {
            return tables.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }

        public string[] GetLines(string documentPath)
        {
            return lines.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }

        public string GetCode(string documentPath)
        {
            return code.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }

        public Context GetTree(string documentPath)
        {
            return trees.TryGetValue(documentPath, out var buffer) ? buffer : null;
        }

        public (string code, string[] lines, Context tree) Get(string documentPath)
        {
            return (GetCode(documentPath), GetLines(documentPath), GetTree(documentPath));
        }

        public List<(CompileException, Context)> GetErrors(string documentPath)
        {
            return errors.TryGetValue(documentPath, out var buffer) ? buffer : default;
        }

        public void ClearErrors(string documentPath)
        {
            errors.TryRemove(documentPath, out _);
        }
    }
}
