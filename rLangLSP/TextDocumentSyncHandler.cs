using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rLangLSP
{
    class TextDocumentSyncHandler : ITextDocumentSyncHandler
    {
        private readonly ILanguageServer _router;
        private readonly BufferManager _bufferManager;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.rl"
            }
        );

        private SynchronizationCapability _capability;

        public TextDocumentSyncHandler(ILanguageServer router, BufferManager bufferManager)
        {
            _router = router;
            _bufferManager = bufferManager;
        }

        public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full;

        public TextDocumentChangeRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentChangeRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                SyncKind = Change
            };
        }

        public TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            return new TextDocumentAttributes(uri, "rlang");
        }

        public async Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            //_router.Window.LogInfo(request.ContentChanges.FirstOrDefault()?.Text);
            return await TextChanged(request.TextDocument.Uri, request.ContentChanges.FirstOrDefault()?.Text);
        }

        public async Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            return await TextChanged(request.TextDocument.Uri, request.TextDocument.Text);
        }

        private async Task<Unit> TextChanged(DocumentUri uri, string text)
        {
            var documentPath = uri.ToString();
            _bufferManager.ClearErrors(documentPath);
            await Task.Run(() => _bufferManager.UpdateText(documentPath, text, _router));

            _router.Window.LogInfo($"hmm + ");
            _router.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams()
            {
                Uri = uri,
                Diagnostics = Diagnostics.GetDiagnostics
                (documentPath, _router, _bufferManager)
            });
            // _router.Window.LogInfo($"Updated buffer for document: {documentPath}\n{text}");
            return Unit.Value;
        }

        public Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public void SetCapability(SynchronizationCapability capability)
        {
            this._capability = capability;
            //throw new NotImplementedException();
        }

        TextDocumentRegistrationOptions IRegistration<TextDocumentRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions();
            //throw new NotImplementedException();
        }

        TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions>.GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions();
            //throw new NotImplementedException();
        }
    }
}
