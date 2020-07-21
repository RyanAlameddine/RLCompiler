'use strict';
Object.defineProperty(exports, "__esModule", { value: true });
const vscode_1 = require("vscode");
const vscode_languageclient_1 = require("vscode-languageclient");
const vscode_jsonrpc_1 = require("vscode-jsonrpc");
const treeHandler = require("./treeHandler");
function activate(context) {
    const tree = new treeHandler.RLangASTProvider();
    vscode_1.window.registerTreeDataProvider('rLangAST', tree);
    vscode_1.commands.registerCommand('rLangAST.jumpTo', (x) => {
        const code = vscode_1.window.activeTextEditor.document.getText();
        const start = getPosition(x.start, code);
        const end = getPosition(x.end, code);
        vscode_1.window.activeTextEditor.selection = new vscode_1.Selection(start, end);
        vscode_1.window.activeTextEditor.revealRange(new vscode_1.Range(start, end));
    });
    // The server is implemented in node
    const serverExe = 'dotnet';
    // If the extension is launched in debug mode then the debug server options are used
    // Otherwise the run options are used
    const serverOptions = {
        run: { command: serverExe, args: [__dirname + '/LSP/rLangLSP.dll'] },
        debug: { command: serverExe, args: ['C:/Users/rhala/Code/RLCompiler/rLangLSP/bin/Debug/netcoreapp3.0/rLangLSP.dll'] }
    };
    // Options to control the language client
    const clientOptions = {
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
    const disposable = client.start();
    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);
}
exports.activate = activate;
function handleAST(x, tree) {
    tree.root = new treeHandler.Node(x[0]["message"], x[0]["children"], x[0]["start"], x[0]["end"]);
    addChildren(x, 0, tree.root);
}
function addChildren(x, index, node) {
    let newIndex = index + 1;
    for (let i = 1; i <= x[index]["children"]; i++) {
        const newNode = new treeHandler.Node(x[newIndex]["message"], x[newIndex]["children"], x[newIndex]["start"], x[newIndex]["end"]);
        node.children.push(newNode);
        newIndex = addChildren(x, newIndex, newNode);
        newIndex++;
    }
    return newIndex - 1;
    // for(let i = 1; i <= x[index]["children"]; i++){
    //     let newIndex = index + i;
    //     let newNode = new treeHandler.Node(x[newIndex]["message"], x[index]["children"], x[index]["start"], x[index]["end"]);
    //     node.children.push(newNode);
    //     addChildren(x, newIndex, newNode);
    // }
}
function getPosition(c, code) {
    let line = 0;
    for (let i = 0; i < c; i++) {
        if (code[i] == '\n')
            line++;
    }
    return new vscode_1.Position(line, c);
}
//# sourceMappingURL=extension.js.map