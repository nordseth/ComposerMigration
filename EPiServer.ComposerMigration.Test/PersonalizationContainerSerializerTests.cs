using EPiServer.ComposerMigration;
using EPiServer.ComposerMigration.DataAbstraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.ComposerMigration.Test
{
    [TestClass]
    public class PersonalizationContainerSerializerTests
    {
        private string serializedData = @"[
            {""ContainerID"":""c7cc5de0-6729-4877-9037-668f4408e38f"",""VisitorGroupList"":[],""IsDefaultContent"":true},
            {""ContainerID"":""bc4de16a-6123-4f46-9bb4-defdc709a696"",""VisitorGroupList"":[
                    {""VisitorGroupID"":""2613bf54-512c-4dfb-85a1-a1355be19a2f"",""VisitorGroupName"":""Visitor from UK""},
                    {""VisitorGroupID"":""6234d2a9-2f85-4168-baa0-dd3698861ba1"",""VisitorGroupName"":""Visitor from Sweden""}
                ],""IsDefaultContent"":false}
        ]";

        [TestMethod]
        public void Deserialize_WithProperData_ShouldDeserializeToList()
        {
            var subject = new PersonalizationContainerSerializer();

            var list = subject.Deserialize(serializedData);

            Assert.AreEqual(2, list.Count());
            Assert.AreEqual(new Guid("c7cc5de0-6729-4877-9037-668f4408e38f"), list.First().ContainerID);
        }

    }
}
