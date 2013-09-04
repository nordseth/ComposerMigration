using EPiServer.Core;
using EPiServer.Core.Transfer;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.ComposerMigration.Test
{
    internal static class TestUtil
    {
        public static ITransferContentData TransferData(RawContent masterContentData, params RawContent[] languageContent)
        {
            var transferData = new Mock<ITransferContentData>();
            transferData.SetupGet(x => x.RawContentData).Returns(masterContentData);
            transferData.SetupGet(x => x.RawLanguageData).Returns(new List<RawContent>(languageContent));
            return transferData.Object;
        }
    }
}
