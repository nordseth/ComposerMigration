using EPiServer.ComposerMigration.DataAbstraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.ComposerMigration.Test
{
    [TestClass]
    public class ContentMapTests
    {
        private const string DefaultLanguage = "en";
        private const string DefaultAreaName = "TestArea";

        [TestMethod]
        public void AddPage_ShouldAddPageToMap()
        {
            // Arrange
            var subject = new ContentMap();
            var page = new ComposerPage { Guid = Guid.NewGuid(), Language = DefaultLanguage };

            // Act
            subject.AddPage(page);

            // Assert
            var result = subject.GetPage(page.Guid, page.Language);
            Assert.AreSame(page, result);
        }

        [TestMethod]
        public void GetParent_WhenGuidMatchesShadowGuidAndHasFunctions_ShouldReturnPage()
        {
            // Arrange
            var subject = new ContentMap();

            var page = new ComposerPage
            {
                Name = "Page name",
                Guid = Guid.NewGuid(),
                Language = DefaultLanguage,
                ShadowGuid = Guid.NewGuid(),
                ContentAreas =
                {
                    new ComposerContentArea 
                    { 
                        Name = DefaultAreaName, 
                        ContentFunctions = { new ComposerContentFunction { Guid = Guid.NewGuid() } } 
                    }
                }
            };

            subject.AddPage(page);

            // Act
            var result = subject.GetParentPage(page.ShadowGuid);

            // Assert
            Assert.AreSame(page, result);
        }

        [TestMethod]
        public void GetParent_WhenGuidMatchesShadowGuidButPageHasNoFunctions_ShouldReturnNull()
        {
            // Arrange
            var subject = new ContentMap();

            var page = new ComposerPage
            {
                Name = "Page name",
                Guid = Guid.NewGuid(),
                Language = DefaultLanguage,
                ShadowGuid = Guid.NewGuid()
            };

            subject.AddPage(page);

            // Act
            var result = subject.GetParentPage(page.ShadowGuid);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetParent_WhenGuidMatchPage_ShouldReturnNull()
        {
            // Arrange
            var subject = new ContentMap();
            var function = new ComposerContentFunction { Guid = Guid.NewGuid(), Language = DefaultLanguage };
            var page = new ComposerPage
            {
                Guid = Guid.NewGuid(),
                Language = DefaultLanguage,
                ContentAreas = { new ComposerContentArea { Name = DefaultAreaName, ContentFunctions = { function } } }
            };
            subject.AddPage(page);

            // Act
            var result = subject.GetParentPage(page.Guid);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetParent_WhenShadowGuidOfAddedPageIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var subject = new ContentMap();

            var page = new ComposerPage
            {
                Guid = Guid.NewGuid(),
                ShadowGuid = Guid.Empty,
                Language = DefaultLanguage,
            };

            subject.AddPage(page);

            // Act
            var result = subject.GetParentPage(page.ShadowGuid);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetParent_WhenGuidMatchesFunction_ShouldReturnPage()
        {
            // Arrange
            var subject = new ContentMap();

            var function = new ComposerContentFunction { Guid = Guid.NewGuid() };
            var page = new ComposerPage
            {
                Name = "Page name",
                Guid = Guid.NewGuid(),
                Language = DefaultLanguage,
                ContentAreas = { new ComposerContentArea { Name = DefaultAreaName, ContentFunctions = { function } } }
            };

            subject.AddPage(page);

            // Act
            var result = subject.GetParentPage(function.Guid);

            // Assert
            Assert.AreSame(page, result);
        }

        [TestMethod]
        public void GetContentFunctions_WhenContentDoesntExist_ShouldReturnEmptyLookup()
        {
            // Arrange
            var subject = new ContentMap();
            var testGuid = Guid.NewGuid();
            var composerPage = new ComposerPage { Guid = Guid.NewGuid(), Language = DefaultLanguage };
            subject.AddPage(composerPage);

            // Act
            var result = subject.GetContentFunctions(testGuid, DefaultLanguage);

            // Assert
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetContentFunctions_WhenContentDoesntHaveAnyFunctions_ShouldReturnEmptyLookup()
        {
            // Arrange
            var subject = new ContentMap();
            var composerPage = new ComposerPage { Guid = Guid.NewGuid(), Language = DefaultLanguage };
            subject.AddPage(composerPage);

            // Act
            var result = subject.GetContentFunctions(composerPage.Guid, composerPage.Language);

            // Assert
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetContentFunctions_WhenContentAreaDoesNotExist_ShouldReturnLookupWithoutFunctions()
        {
            // Arrange
            var subject = new ContentMap();
            var areaName = "Area54";
            var page = new ComposerPage
            {
                Guid = Guid.NewGuid(),
                Language = DefaultLanguage,
                ContentAreas = { new ComposerContentArea { Name = DefaultAreaName } }
            };
            subject.AddPage(page);

            // Act
            var result = subject.GetContentFunctions(page.Guid, page.Language)[areaName];

            // Assert
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetContentFunctions_WhenContentHasAreaWithoutFunctions_ShouldReturnLookupWithoutFunctions()
        {
            // Arrange
            var subject = new ContentMap();
            var page = new ComposerPage
            {
                Guid = Guid.NewGuid(),
                Language = DefaultLanguage,
                ContentAreas = { new ComposerContentArea { Name = DefaultAreaName } }
            };
            subject.AddPage(page);

            // Act
            var result = subject.GetContentFunctions(page.Guid, page.Language)[DefaultAreaName];

            // Assert
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GetContentFunctions_WhenContentHasFunction_ShouldReturnLookupWithAnyFunction()
        {
            // Arrange
            var subject = new ContentMap();
            var function = new ComposerContentFunction { Guid = Guid.NewGuid() };
            var page = new ComposerPage
            {
                Guid = Guid.NewGuid(),
                Language = DefaultLanguage,
                ContentAreas = { new ComposerContentArea { Name = DefaultAreaName, ContentFunctions = { function } } }
            };
            subject.AddPage(page);

            // Act
            var result = subject.GetContentFunctions(page.Guid, page.Language)[DefaultAreaName];

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(b => b.Guid == function.Guid));
        }

        [TestMethod]
        public void GetContentFunctions_WhenLanguageIsIncorrect_ShouldCheckParentLanguage()
        {
            // Arrange
            var subject = new ContentMap();
            var nestedFunction = new ComposerContentFunction { Guid = Guid.NewGuid(), Language = DefaultLanguage };

            var layoutFunction = new ComposerContentFunction
            {
                Guid = Guid.NewGuid(),
                Language = DefaultLanguage,
                ContentAreas = { new ComposerContentArea { Name = DefaultAreaName, ContentFunctions = { nestedFunction } } }
            };

            var page = new ComposerPage
            {
                Guid = Guid.NewGuid(),
                Language = "sv",
                ContentAreas = { new ComposerContentArea { Name = DefaultAreaName, ContentFunctions = { layoutFunction } } }
            };

            subject.AddPage(page);

            // Act
            var result = subject.GetContentFunctions(layoutFunction.Guid, DefaultLanguage)[DefaultAreaName];

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(b => b.Guid == nestedFunction.Guid));
        }

        [TestMethod]
        public void RestructurePersonalization_ShouldMovePersonalizedFunctionsToContainerParent()
        {
            // Arrange
            var subject = new ContentMap();
            var sourceFunctions = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var containerGuid = Guid.NewGuid();
            var page = CreatePersonalizedComposerPage(containerGuid, sourceFunctions);
            subject.AddPage(page);
            subject.AddPersonalisationContainer(containerGuid);

            // Act
            subject.RestructurePersonalization();

            // Assert
            var restructuredFunctions = subject.GetContentFunctions(page.Guid, DefaultLanguage).Single().ToList();
            Assert.AreEqual(2, restructuredFunctions.Count());
            Assert.AreEqual(sourceFunctions[0], restructuredFunctions[0].Guid);
            Assert.AreEqual(sourceFunctions[1], restructuredFunctions[1].Guid);
        }

        [TestMethod]
        public void RestructurePersonalization_ShouldSetTheSamePersonalizationGroupOnAllFunctions()
        {
            // Arrange
            var subject = new ContentMap();
            var sourceFunctions = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var containerGuid = Guid.NewGuid();
            var page = CreatePersonalizedComposerPage(containerGuid, sourceFunctions);
            subject.AddPage(page);
            subject.AddPersonalisationContainer(containerGuid);

            // Act
            subject.RestructurePersonalization();

            // Assert
            var restructuredFunctions = subject.GetContentFunctions(page.Guid, DefaultLanguage).Single().ToArray();
            Assert.IsTrue(restructuredFunctions.Skip(1).All(b => b.PersonalizationGroup == restructuredFunctions[0].PersonalizationGroup));
        }

        [TestMethod]
        public void ExpandVisitorGroupReference_WhenReferenceIsEmpty_ShouldNotSetVisitorGroups()
        {
            // Arrange
            var subject = new ContentMap();
            var function = new ComposerContentFunction { VisitorGroupContainerID = Guid.Empty };

            // Act
            subject.ExpandVisitorGroupReference(function);

            // Assert
            Assert.IsFalse(function.VisitorGroups.Any());
        }

        [TestMethod]
        public void ExpandVisitorGroupReference_WhenReferenceIsNotFound_ShouldNotSetVisitorGroups()
        {
            // Arrange
            var subject = new ContentMap();
            var function = new ComposerContentFunction { VisitorGroupContainerID = Guid.NewGuid() };

            // Act
            subject.ExpandVisitorGroupReference(function);

            // Assert
            Assert.IsFalse(function.VisitorGroups.Any());
        }

        [TestMethod]
        public void ExpandVisitorGroupReference_WhenReferenceHasBeenRegistered_ShouldSetVisitorGroups()
        {
            // Arrange
            var subject = new ContentMap();
            var reference = Guid.NewGuid();
            var visitorGroups = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var function = new ComposerContentFunction { VisitorGroupContainerID = reference };
            subject.AddVisitorGroupMap(reference, visitorGroups);

            // Act
            subject.ExpandVisitorGroupReference(function);

            // Assert
            Assert.AreEqual(3, function.VisitorGroups.Count());
            Assert.IsTrue(function.VisitorGroups.SequenceEqual(visitorGroups));
        }

        private static ComposerPage CreatePersonalizedComposerPage(Guid containerGuid, IEnumerable<Guid> functionGuids)
        {
            var functions = new List<ComposerContentFunction>();

            foreach (var guid in functionGuids)
            {
                functions.Add(new ComposerContentFunction { Guid = guid, Language = DefaultLanguage });
            }

            return CreatePersonalizedComposerPage(containerGuid, functions);
        }

        private static ComposerPage CreatePersonalizedComposerPage(Guid containerGuid, IEnumerable<ComposerContentFunction> functions)
        {
            var personalizedContainer = new ComposerContentFunction
            {
                Guid = containerGuid,
                Language = DefaultLanguage,
                ContentAreas = { 
                    new ComposerContentArea {
                        Name = "PersonalizationArea",
                        ContentFunctions = functions.ToList()
                    }
                }
            };

            return new ComposerPage
            {
                Guid = Guid.NewGuid(),
                Language = DefaultLanguage,
                ContentAreas = { 
                    new ComposerContentArea { 
                        Name = DefaultAreaName, 
                        ContentFunctions = { personalizedContainer } 
                    }
                }
            };
        }
    }
}
