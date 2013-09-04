using EPiServer.ComposerMigration.DataAbstraction;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.DataAbstraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace EPiServer.ComposerMigration.Test
{
    [TestClass]
    public class DynamicFunctionTransformTests
    {
        [TestMethod]
        public void TransformFunctionPickerProperty_ShouldChangeTheTypeToContentReference()
        {
            // Arrange
            var subject = CreateSubject();
            var property = new RawProperty { Type = PropertyDataType.String, TypeName = ComposerPropertyTypes.FunctionPicker, AssemblyName = "Dropit.Extension" };

            // Act
            subject.TransformFunctionPickerProperty(property);

            // Assert
            Assert.AreEqual(PropertyDataType.ContentReference, property.Type);
            Assert.IsNull(property.TypeName);
            Assert.IsNull(property.AssemblyName);
        }

        private static ComposerSerializer CreateSerializer(FunctionPickerData functionPickerData)
        {
            var serializer = new Mock<ComposerSerializer>();
            serializer.Setup(x => x.Deserialize<FunctionPickerData>(It.IsAny<string>())).Returns(functionPickerData);
            return serializer.Object;
        }

        private static DynamicFunctionTransform CreateSubject(FunctionPickerData functionPickerData = null, ExportLinkResolver exportLinkResolver = null)
        {
            exportLinkResolver = exportLinkResolver ?? new Mock<ExportLinkResolver>(null).Object;
            return new DynamicFunctionTransform(CreateSerializer(functionPickerData), exportLinkResolver);
        }
    }
}
