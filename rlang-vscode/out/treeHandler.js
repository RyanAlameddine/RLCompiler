"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const vscode = require("vscode");
class RLangASTProvider {
    constructor() {
        this._onDidChangeTreeData = new vscode.EventEmitter();
        this.onDidChangeTreeData = this._onDidChangeTreeData.event;
    }
    change() {
        this._onDidChangeTreeData.fire(undefined);
    }
    getTreeItem(element) {
        return element;
    }
    getChildren(element) {
        if (element == undefined)
            return Promise.resolve([this.root]);
        if (element.children.length == 0)
            return Promise.resolve([]);
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
exports.RLangASTProvider = RLangASTProvider;
class Node extends vscode.TreeItem {
    constructor(label, childrenCount, start, end) {
        super(label, childrenCount != 0 ? vscode.TreeItemCollapsibleState.Expanded : vscode.TreeItemCollapsibleState.None);
        this.label = label;
        this.childrenCount = childrenCount;
        this.start = start;
        this.end = end;
        this.desc = "";
        this.children = [];
        if (label.includes('(')) {
            const index = label.indexOf('(');
            this.desc = label.substring(index, label.length - 1);
            this.label = label.substring(0, index - 1);
        }
    }
    get tooltip() {
        return `${this.label}`;
    }
    get description() {
        return `${this.desc}`;
    }
}
exports.Node = Node;
//# sourceMappingURL=treeHandler.js.map