'use strict';
Object.defineProperty(exports, "__esModule", { value: true });
const vscode_1 = require("vscode");
const vscode_languageclient_1 = require("vscode-languageclient");
const vscode_jsonrpc_1 = require("vscode-jsonrpc");
const treeHandler = require("./treeHandler");
function activate(context) {
    let tree = new treeHandler.RLangASTProvider();
    vscode_1.window.registerTreeDataProvider('rLangAST', tree);
    // The server is implemented in node
    let serverExe = 'dotnet';
    // If the extension is launched in debug mode then the debug server options are used
    // Otherwise the run options are used
    let serverOptions = {
        run: { command: serverExe, args: ['/src/rLangLSP.dll'] },
        debug: { command: serverExe, args: ['C:/Users/rhala/Code/RLCompiler/rLangLSP/bin/Debug/netcoreapp3.0/rLangLSP.dll'] }
    };
    // Options to control the language client
    let clientOptions = {
        documentSelector: [
            {
                pattern: '**/*.rl',
            }
        ],
        synchronize: {
            configurationSection: 'rLangLanguageServer',
            fileEvents: vscode_1.workspace.createFileSystemWatcher('**/*.rl')
        },
    };
    // Create the language client and start the client.
    const client = new vscode_languageclient_1.LanguageClient('rLangLanguageServer', 'rLang Language Server', serverOptions, clientOptions);
    client.trace = vscode_jsonrpc_1.Trace.Verbose;
    client.onReady().then(() => client.onNotification("rlang/loadAST", (x) => {
        handleAST(JSON.parse(x), tree);
        tree.change();
    }));
    let disposable = client.start();
    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);
}
exports.activate = activate;
function handleAST(x, tree) {
    tree.root = new treeHandler.Node(x[0]["message"], x[0]["children"]);
    addChildren(x, 0, tree.root);
}
function addChildren(x, index, node) {
    for (let i = 1; i <= x[index]["children"]; i++) {
        let newIndex = index + i;
        let newNode = new treeHandler.Node(x[newIndex]["message"], x[index]["children"]);
        node.children.push(newNode);
        addChildren(x, newIndex, newNode);
    }
}
//# sourceMappingURL=extension.js.map