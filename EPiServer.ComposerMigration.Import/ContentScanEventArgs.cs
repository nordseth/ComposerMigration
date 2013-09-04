using EPiServer.Core.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration
{
    public class ContentScanEventArgs : EventArgs
    {
        private ITransferContentData _transferContentData;
        private IContentTransferContext _transferContext;

        public ContentScanEventArgs(ITransferContentData transferContentData, IContentTransferContext transferContext)
        {
            _transferContentData = transferContentData;
            _transferContext = transferContext;
        }

        public ITransferContentData TransferContentData
        {
            get { return _transferContentData; }
        }

        public IContentTransferContext TransferContext
        {
            get { return _transferContext; }
        }
    }
}
