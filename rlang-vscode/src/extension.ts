'use strict';

import { workspace, ExtensionContext, window, commands, Selection, Position, Range } from 'vscode';
import { LanguageClient, LanguageClientOptions, ServerOptions } from 'vscode-languageclient';
import { Trace } from 'vscode-jsonrpc';

import * as treeHandler from "./treeHandler"

export function activate(context: ExtensionContext) : void{

    const tree =  new treeHandler.RLangASTProvider();
    window.registerTreeDataProvider('rLangAST', tree);
    commands.registerCommand('rLangAST.jumpTo', (x : treeHandler.Node) => {
        const code = window.activeTextEditor.document.getText();
        const start = getPosition(x.start, code)
        const end = getPosition(x.end, code);
        
        window.activeTextEditor.selection = new Selection(start, end)
        window.activeTextEditor.revealRange(new Range(start, end))
    })


    // The server is implemented in node
    const serverExe = 'dotnet';

    // If the extension is launched in debug mode then the debug server options are used
    // Otherwise the run options are used
    const serverOptions: ServerOptions = {
        run: { command: serverExe, args: [__dirname + '/LSP/rLangLSP.dll'] },
        debug: { command: serverExe, args: ['C:/Users/rhala/Code/RLCompiler/rLangLSP/bin/Debug/netcoreapp3.0/rLangLSP.dll'] }
    }

    // Options to control the language client
    const clientOptions: LanguageClientOptions = {
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

    const disposable = client.start();

    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);
}

function handleAST(x : unknown, tree: treeHandler.RLangASTProvider){
    tree.root = new treeHandler.Node(x[0]["message"], x[0]["children"], x[0]["start"], x[0]["end"]);
    addChildren(x, 0, tree.root);
}

function addChildren(x : unknown, index : number, node : treeHandler.Node) : number{
    let newIndex = index + 1;
    for(let i = 1; i <= x[index]["children"]; i++){
        
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

function getPosition(c: number, code : string) : Position{
    let line = 0;
    for(let i = 0; i < c; i++)
    {
        if (code[i] == '\n') line++;
    }
    return new Position(line, c);
}