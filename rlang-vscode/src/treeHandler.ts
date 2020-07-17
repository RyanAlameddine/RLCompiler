import * as vscode from 'vscode';
import * as fs from 'fs';
import * as path from 'path';

export class RLangASTProvider implements vscode.TreeDataProvider<Node> {
    constructor() {}

    root: Node;


    private _onDidChangeTreeData: vscode.EventEmitter<Node | undefined> = new vscode.EventEmitter<Node | undefined>();
    readonly onDidChangeTreeData: vscode.Event<Node | undefined> = this._onDidChangeTreeData.event;
    change(): void
    {
        this._onDidChangeTreeData.fire(undefined);
    }

    getTreeItem(element: Node): vscode.TreeItem {
        return element;
    }

    getChildren(element?: Node): Thenable<Node[]> {
        if(element == undefined) return Promise.resolve([this.root]);

        if(element.children.length == 0) return Promise.resolve([]);

        return Promise.resolve(element.children);

        // if (!this.workspaceRoot) {
        //   vscode.window.showInformationMessage('No dependency in empty workspace');
        //   return Promise.resolve([]);
        // }

        // if (element) {
        //   return Promise.resolve(
        //     this.getDepsInPackageJson(
        //       path.join(this.workspaceRoot, 'node_modules', element.label, 'package.json')
        //     )
        //   );
        // } else {
        //   const packageJsonPath = path.join(this.workspaceRoot, 'package.json');
        //   if (this.pathExists(packageJsonPath)) {
        //     return Promise.resolve(this.getDepsInPackageJson(packageJsonPath));
        //   } else {
        //     vscode.window.showInformationMessage('Workspace has no package.json');
        //     return Promise.resolve([]);
        //   }
        // }
    }
}

export class Node extends vscode.TreeItem {

    children: Node[]

    constructor(
        public readonly label: string,
        public readonly childrenCount: number,
        public readonly start: number,
        public readonly end: number,
    ) {
        super(label, childrenCount != 0 ? vscode.TreeItemCollapsibleState.Expanded : vscode.TreeItemCollapsibleState.None);
        this.children = []
    }

    get tooltip(): string {
        return `${this.label}`;
    }
    
    //   iconPath = {
    //     light: path.join(__filename, '..', '..', 'resources', 'light', 'dependency.svg'),
    //     dark: path.join(__filename, '..', '..', 'resources', 'dark', 'dependency.svg')
    //   };
}