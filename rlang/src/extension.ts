'use strict';

import * as vscode from 'vscode';
import * as fs from 'fs';
import * as path from 'path';

import {ExtensionContext,  } from 'vscode';

export function promisifyReadFile(filename: string): Promise<string> {
    return new Promise<string>((resolve, reject) => {
        fs.readFile(filename, 'utf8', (err, data) => {
        if (err) {
            reject(err);
        } else {
            resolve(data);
        }
        });
    });
}
  
interface Parameter {
    name: string;
    type: string;
}

interface OpCode {
    name: string;
    description: string;
    category: string;
    params: Parameter[];
    
}

class RlangProvider implements vscode.HoverProvider, vscode.CompletionItemProvider {
    opCodeCompletionItems : vscode.CompletionList = new vscode.CompletionList();

    private opCodes: { [name: string]: OpCode };

    public constructor() {
        // this.opCodes = opCodes;
        // for(let key in opCodes){
        //     this.opCodeCompletionItems.items.push(this.createSnippetItem(key, opCodes[key]));
        // }

        // let progSpace = new vscode.CompletionItem("progspace", vscode.CompletionItemKind.Snippet);
        // progSpace.insertText = new vscode.SnippetString("$---$\n");
        // progSpace.documentation = new vscode.MarkdownString(`## $---$\n\n *** \n\nMarks the beginning of program space`);
        // this.opCodeCompletionItems.items.push(progSpace);
    }

    lineOf(text: string, count: number) : number{
        var line = 0;
      
        for (var i = 0; i < text.length; i++) {
            if (i == count){
                return line + 1;
            }
            if (text[i] === '\n'){
                line++;
            }
        }
      
        return  -1;
    }

    getLabelChar(document: vscode.TextDocument, name : string) : number{
        return document.getText().search(new RegExp('\\b' + name + ':'));
    }

    getDescription(label : string, opCode : OpCode) : string {
        let paramString : string = '';
        for(let param of opCode.params){
                paramString += `*@params* \`${param.name}\`: ${param.type}\n\n`;
        }
        return `OpCode **${label}**: ${opCode.name}\n\n *** \n\n${opCode.description}\n\n${paramString}`;
            
    }

    provideHover(document: vscode.TextDocument, position: vscode.Position, token: vscode.CancellationToken): vscode.ProviderResult<vscode.Hover> {
        const range = document.getWordRangeAtPosition(position);
        if(!range) return;
        const text = document.getText(range);

        //registers
        if(/r[0-9A-F]/.test(text)){
            let hexString = parseInt(text[1], 16);
            return new vscode.Hover('Register ' + hexString);
        }

        const opCode: OpCode = this.opCodes[text];

        //opCodes
        if(opCode){
            return new vscode.Hover(this.getDescription(text.toLowerCase(), opCode));
        }

        //Hex
        if(/[0-9A-F]+(_)([0-9A-F])/.test(text)){
            let hex = text.replace('_', '');
            let hexValue = parseInt(hex, 16);

            return new vscode.Hover(`Hex value: ${hexValue.toString(16)}\n\nDec value: ${hexValue}`);
        }

        //label
        const line = this.lineOf(document.getText(), this.getLabelChar(document, text));
        if(line != -1){
            return new vscode.Hover(`Label: **${text}**\n\nLinks to line ` + line);
        }

        return new vscode.Hover('Not found');
    }

    provideCompletionItems(document: vscode.TextDocument, position: vscode.Position, token: vscode.CancellationToken, context: vscode.CompletionContext): vscode.ProviderResult<vscode.CompletionItem[] | vscode.CompletionList> {

        return this.opCodeCompletionItems;
    }

    createSnippetItem(label: string, opCode: OpCode): vscode.CompletionItem {
        let item = new vscode.CompletionItem(label, vscode.CompletionItemKind.Snippet);
        
        let paramString = ''

        let counter = 0;
        for(let param of opCode.params){
            counter++;
            if(param.type == "Register"){
                paramString += "r${" + counter + ":0} ";
            }else if(param.type == "Short"){
                counter++;
                paramString += "${" + counter  + ":00_00} ";
            }
        }
        paramString = paramString.slice(0, paramString.length - 1);
        
        counter = 3 - counter;
        for(var i = 0; i < counter; i++){
            paramString += "   ";
        }
        if(counter == 3){
            paramString = paramString.substring(0, paramString.length - 1);
        }

        if(label.length == 2) label += "  ";
        else if(label.length == 3) label += " ";

		item.insertText = new vscode.SnippetString(label + `[${paramString}] `);
		item.documentation = new vscode.MarkdownString(this.getDescription(label, opCode));

		return item;
	}

    dispose() {

    }
}

export async function activate(context: ExtensionContext) {
    console.log('Congratulations, your extension "rlang" is now active!');

    const prov: RlangProvider = new RlangProvider();

    vscode.languages.registerHoverProvider('rlang', prov);

    vscode.languages.registerCompletionItemProvider('rlang', prov);
}