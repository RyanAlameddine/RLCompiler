'use strict';

import { workspace, Disposable, ExtensionContext, window } from 'vscode';
import { LanguageClient, LanguageClientOptions, SettingMonitor, ServerOptions, TransportKind, InitializeParams, VersionedTextDocumentIdentifier } from 'vscode-languageclient';
import { Trace } from 'vscode-jsonrpc';

import * as treeHandler from "./treeHandler"

export function activate(context: ExtensionContext) {

    let tree =  new treeHandler.RLangASTProvider();
    window.registerTreeDataProvider('rLangAST', tree)

    // The server is implemented in node
    let serverExe = 'dotnet';

    // If the extension is launched in debug mode then the debug server options are used
    // Otherwise the run options are used
    let serverOptions: ServerOptions = {
        run: { command: serverExe, args: ['/src/rLangLSP.dll'] },
        debug: { command: serverExe, args: ['C:/Users/rhala/Code/RLCompiler/rLangLSP/bin/Debug/netcoreapp3.0/rLangLSP.dll'] }
    }

    // Options to control the language client
    let clientOptions: LanguageClientOptions = {
        documentSelector: [
            {
                pattern: '**/*.rl',
            }
        ],
        synchronize: {
            configurationSection: 'rLangLanguageServer',
            fileEvents: workspace.createFileSystemWatcher('**/*.rl')
        },

    }

    // Create the language client and start the client.
    const client = new LanguageClient('rLangLanguageServer', 'rLang Language Server', serverOptions, clientOptions);
    client.trace = Trace.Verbose;

    client.onReady().then(() => client.onNotification("rlang/loadAST", (x:string) => {
        handleAST(JSON.parse(x), tree);
        tree.change();
    }));

    let disposable = client.start();

    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);
}

function handleAST(x : any, tree: treeHandler.RLangASTProvider){
    tree.root = new treeHandler.Node(x[0]["message"], x[0]["children"]);
    addChildren(x, 0, tree.root);
}

function addChildren(x, index, node){
    for(let i = 1; i <= x[index]["children"]; i++){
        let newIndex = index + i;
        let newNode = new treeHandler.Node(x[newIndex]["message"], x[index]["children"]);
        node.children.push(newNode)
        addChildren(x, newIndex, newNode);
    }
}