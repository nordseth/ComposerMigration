using EPiServer.ComposerMigration;
using EPiServer.ComposerMigration.DataAbstraction;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.DataAbstraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.ComposerMigration.Test
{
    [TestClass]
    public class PersonalizationContainerCollectorTests
    {
        private const string PersonalizationDataMockValue = "Smash!";

        [TestMethod]
        public void Collect_WhenContentIsPersonalizationContainer_ShouldAddContainerToMap()
        {
            // Arrange
            var contentMap = new Mock<IContentMap>();
            var subject = CreateSubject(contentMap.Object, null);

            var containerGuid = Guid.NewGuid();

            var master = new RawContent
            {
                Property = new[] 
                { 
                    new RawProperty { Name = ComposerProperties.PersonalizationData, Value = PersonalizationDataMockValue },
                    new RawProperty { Name = MetaDataProperties.PageGUID, Value = containerGuid.ToString() } 
                }
            };

            // Act
            subject.Collect(CreateTransferData(master));

            // Assert
            contentMap.Verify(x => x.AddPersonalisationContainer(containerGuid), Times.Once());
        }

        [TestMethod]
        public void Collect_ShouldAddCorrectVisitorGroupsForEachContainer()
        {
            // Arrange
            var contentMap = new Mock<IContentMap>();
            var guids = new [] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var containers = new[] { 
                new PersonalizationContainer { ContainerID = guids[0], IsDefaultContent = true }, 
                new PersonalizationContainer { ContainerID = guids[1], VisitorGroupList = new [] 
                    { 
                        new VisitorGroup { VisitorGroupID = guids[2] },
                        new VisitorGroup { VisitorGroupID = guids[3] } 
                    }
                } 
            };
            var subject = CreateSubject(contentMap.Object, containers);

            var master = new RawContent { Property = new[] { new RawProperty { Name = ComposerProperties.PersonalizationData, Value = PersonalizationDataMockValue } } };

            // Act
            subject.Collect(CreateTransferData(master));

            // Assert
            contentMap.Verify(x => x.AddVisitorGroupMap(guids[0]), Times.Once());
            contentMap.Verify(x => x.AddVisitorGroupMap(guids[1], guids[2], guids[3]), Times.Once());
        }

        private static PersonalizationContainerCollector CreateSubject(IContentMap contentMap, params PersonalizationContainer[] containerData)
        {
            contentMap = contentMap ?? new Mock<IContentMap>().Object;
            var serializer = new Mock<PersonalizationContainerSerializer>();
            serializer.Setup(x => x.Deserialize(PersonalizationDataMockValue)).Returns(containerData ?? Enumerable.Empty<PersonalizationContainer>());
            return new PersonalizationContainerCollector(contentMap, serializer.Object);
        }

        private static ITransferContentData CreateTransferData(RawContent masterContentData)
        {
            var transferData = new Mock<ITransferContentData>();
            transferData.SetupGet(x => x.RawContentData).Returns(masterContentData);
            return transferData.Object;
        }
    }
}
