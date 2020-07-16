
using RLParser;
using System.Collections.Concurrent;

namespace rLangLSP
{
    class BufferManager
    {
        private ConcurrentDictionary<string, string> code = new ConcurrentDictionary<string, string>();
        private ConcurrentDictionary<string, string[]> lines = new ConcurrentDictionary<string, string[]>();
        private ConcurrentDictionary<string, Context> trees = new ConcurrentDictionary<string, Context>();

        public void UpdateText(string documentPath, string buffer)
        {
            string[] l = buffer.Split('\n');
            lines.AddOrUpdate(documentPath, l, (k, v) => l);
            code.AddOrUpdate(documentPath, buffer, (k, v) => buffer);

            Context tree = RLParser.RLParser.Parse(buffer);
            trees.AddOrUpdate(documentPath, tree, (k, v) => tree);
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
    }
}
