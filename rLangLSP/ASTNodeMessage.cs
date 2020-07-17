using Newtonsoft.Json;
using RLParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace rLangLSP
{
    static class NodeMessages
    {
        public static ASTNodeMessage[] GetMessages(Context root)
        {
            var list = new List<ASTNodeMessage>();
            getMessages(list, root);
            return list.ToArray();
        }

        private static void getMessages(List<ASTNodeMessage> messages, Context root)
        {
            messages.Add(new ASTNodeMessage(root.ToString(), root.Children.Count, root.Characters.Start.Value, root.Characters.End.Value));
            foreach(var child in root.Children)
            {
                getMessages(messages, child);
            }
        }
    }

    [Serializable]
    struct ASTNodeMessage
    {
        [JsonProperty]
        string message { get; set; }
        [JsonProperty]
        int children { get; set; }

        [JsonProperty]
        int start { get; set; }
        [JsonProperty]
        int end { get; set; }

        public ASTNodeMessage(string message, int children, int start, int end)
        {
            this.message = message;
            this.children = children;
            this.start = start;
            this.end = end;
        }
    }
}
