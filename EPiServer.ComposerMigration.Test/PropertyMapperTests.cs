using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EPiServer.ComposerMigration.DataAbstraction;

namespace EPiServer.ComposerMigration.Test
{
    [TestClass]
    public class PropertyMapperTests
    {
        [TestMethod]
        public void Map_WhenTypeIsContentArea_ShouldSetDisplayEditUIToTrue()
        {
            var subject = CreateSubject();
            var pageDefinition = new PageDefinition 
            {
                Name = "MyContentArea",
                DisplayEditUI = false,
                Type = new PageDefinitionType { 
                    TypeName = ComposerPropertyTypes.ContentArea, 
                    DataType = "String" }
            };

            var propertyDefinition = subject.Map(pageDefinition);

            Assert.IsTrue(propertyDefinition.DisplayEditUI);
        }

        [TestMethod]
        public void Map_WhenTypeIsContentArea_ShouldSetSearchableToFalse()
        {
            var subject = CreateSubject();
            var pageDefinition = new PageDefinition
            {
                Name = "MyContentArea",
                Searchable = true,
                Type = new PageDefinitionType
                {
                    TypeName = ComposerPropertyTypes.ContentArea,
                    DataType = "String"
                }
            };

            var propertyDefinition = subject.Map(pageDefinition);

            Assert.IsFalse(propertyDefinition.Searchable);
        }

        [TestMethod]
        public void MapType_WhenTypeNameIsEmpty_ShouldReturnMappedDataType()
        {
            var subject = CreateSubject();
            var definitionType = new PageDefinitionType { DataType = "String" };

            var propertyType = subject.MapType(definitionType);

            Assert.AreEqual(typeof(string), propertyType.Type);
        }

        [TestMethod]
        public void MapType_WhenTypeNameHasMapping_ShouldReturnMappedTypeName()
        {
            var subject = CreateSubject();
            var definitionType = new PageDefinitionType { DataType = "String", TypeName = "EPiServer.SpecializedProperties.PropertyImageUrl" };

            var propertyType = subject.MapType(definitionType);

            Assert.AreEqual("EPiServer.Url", propertyType.TypeName);
        }

        [TestMethod]
        public void MapType_WhenTypeNameHasNoMapping_ShouldReturnMappedDataType()
        {
            var subject = CreateSubject();
            var definitionType = new PageDefinitionType { DataType = "String", TypeName = "MyCustom.PropertyImageUrl" };

            var propertyType = subject.MapType(definitionType);

            Assert.AreEqual(typeof(string), propertyType.Type);
        }

        [TestMethod]
        public void MapType_WhenTypeNameHasNoMapping_ShouldReturnTypeAsBackingType()
        {
            var subject = CreateSubject();
            var definitionType = new PageDefinitionType { DataType = "String", TypeName = "MyCustom.PropertyImageUrl" };

            var propertyType = subject.MapType(definitionType);

            Assert.AreEqual(definitionType.TypeName, propertyType.BackingTypeName);
        }

        [TestMethod]
        public void MapType_WhenTypeNameHasNoMappingAnOptionIsSet_ShouldSetUIHintToLegacyWrapper()
        {
            var subject = CreateSubject(new OptionsFake { ForceLegacyPropertyWrapper = true });
            var definitionType = new PageDefinitionType { DataType = "String", TypeName = "MyCustom.PropertyImageUrl" };

            var propertyType = subject.MapType(definitionType);

            Assert.AreEqual("LegacyWrapper", propertyType.UIHint);
        }

        [TestMethod]
        public void MapType_WhenTypeNameHasNoMappingAnOptionIsNotSet_ShouldSetUIHintToNull()
        {
            var subject = CreateSubject(new OptionsFake { ForceLegacyPropertyWrapper = false });
            var definitionType = new PageDefinitionType { DataType = "String", TypeName = "MyCustom.PropertyImageUrl" };

            var propertyType = subject.MapType(definitionType);

            Assert.IsNull(propertyType.UIHint);
        }

        [TestMethod]
        public void MapType_WhenDataTypeIsNumberAndUseNullableValueTypesIsSet_ShouldReturnNullableInteger()
        {
            var subject = CreateSubject(new OptionsFake { UseNullableValueTypes = true });
            var definitionType = new PageDefinitionType { DataType = "Number" };

            var propertyType = subject.MapType(definitionType);

            Assert.AreEqual(typeof(int?), propertyType.Type);
        }

        [TestMethod]
        public void MapType_WhenDataTypeIsFloatAndUseNullableValueTypesIsSet_ShouldReturnNullableDouble()
        {
            var subject = CreateSubject(new OptionsFake { UseNullableValueTypes = true });
            var definitionType = new PageDefinitionType { DataType = "FloatNumber" };

            var propertyType = subject.MapType(definitionType);

            Assert.AreEqual(typeof(double?), propertyType.Type);
        }

        [TestMethod]
        public void MapType_WhenDataTypeIsDateAndUseNullableValueTypesIsSet_ShouldReturnNullableDateTime()
        {
            var subject = CreateSubject(new OptionsFake { UseNullableValueTypes = true });
            var definitionType = new PageDefinitionType { DataType = "Date" };

            var propertyType = subject.MapType(definitionType);

            Assert.AreEqual(typeof(DateTime?), propertyType.Type);
        }

        [TestMethod]
        public void MapType_WhenDataTypeIsTimeSpanAndUseNullableValueTypesIsSet_ShouldReturnNullableTimeSpan()
        {
            var subject = CreateSubject(new OptionsFake { UseNullableValueTypes = true });
            var definitionType = new PageDefinitionType { TypeName = "EPiServer.SpecializedProperties.PropertyTimeSpan" };

            var propertyType = subject.MapType(definitionType);

            Assert.AreEqual(typeof(TimeSpan?), propertyType.Type);
        }

        [TestMethod]
        public void MapType_WhenDataTypeIsBooleanAndUseNullableValueTypesIsSet_ShouldReturnBoolean()
        {
            var subject = CreateSubject(new OptionsFake { UseNullableValueTypes = true });
            var definitionType = new PageDefinitionType { DataType = "Boolean" };

            var propertyType = subject.MapType(definitionType);

            Assert.AreEqual(typeof(bool), propertyType.Type);
        }

        private static PropertyMapper CreateSubject(IComposerTranformationOptions options = null)
        {
            return new PropertyMapper(new MemberNameValidator(), options);
        }

    }
}
