
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using RLParser;
using RLTypeChecker;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace rLangLSP
{
    class BufferManager
    {
        private ConcurrentDictionary<string, string> code = new ConcurrentDictionary<string, string>();
        private ConcurrentDictionary<string, string[]> lines = new ConcurrentDictionary<string, string[]>();
        private ConcurrentDictionary<string, Context> trees = new ConcurrentDictionary<string, Context>();

        private ConcurrentDictionary<string, List<(CompileException, Context)>> errors = new ConcurrentDictionary<string, List<(CompileException, Context)>>();

        public void UpdateText(string documentPath, string buffer, ILanguageServer router)
        {
            string[] l = buffer.Split('\n');
            lines.AddOrUpdate(documentPath, l, (k, v) => l);
            code.AddOrUpdate(documentPath, buffer, (k, v) => buffer);

            int erc = 0;
            Action<CompileException, Context> onError = (e, l) =>
            {
                errors.AddOrUpdate(documentPath, new List<(CompileException, Context)>() { (e, l) }, (k, v) =>
                {
                    v.Add((e, l));
                    erc++;
                    return v;
                });
            };
            Context tree = RLParser.RLParser.Parse(buffer, onError);
            if (erc == 0) RlTypeChecker.TypeCheck(tree, onError);
            trees.AddOrUpdate(documentPath, tree, (k, v) => tree);
            try
            {
                ASTNodeMessage[] strings = NodeMessages.GetMessages(tree);
                var s = Newtonsoft.Json.JsonConvert.SerializeObject(strings);
                router.SendNotification("rlang/loadAST", s);
            }
            catch(Exception e)
            {
                router.Window.LogError(e.ToString());
            }

            router.Window.LogInfo("updated tree");
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
