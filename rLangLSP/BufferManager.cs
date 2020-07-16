
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Text;

//namespace rLangLSP
//{
//    class BufferManager
//    {
//        private ConcurrentDictionary<string, Buffer> _buffers = new ConcurrentDictionary<string, Buffer>();

//        public void UpdateBuffer(string documentPath, Buffer buffer)
//        {
//            _buffers.AddOrUpdate(documentPath, buffer, (k, v) => buffer);
//        }

//        public Buffer GetBuffer(string documentPath)
//        {
//            return _buffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
//        }
//    }
//}
