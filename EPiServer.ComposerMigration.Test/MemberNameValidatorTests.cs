using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EPiServer.ComposerMigration.Test
{
    [TestClass]
    public class MemberNameValidatorTests
    {
        [TestMethod]
        public void CreateIdentifier_ShouldRemoveWhitespace()
        {
            var name = "My Name";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("MyName", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameStartWithADigit_ShouldReplaceNumbersWithItsName()
        {
            var name = "3 Column page";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("ThreeColumnPage", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameStartWithANumber_ShouldReplaceNumbersWithItsName()
        {
            var name = "3500 Column page";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("ThreeThousandFiveHundredColumnPage", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameHasANumberInItsName_ShouldLeaveAsItIs()
        {
            var name = "Column 5";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("Column5", result);
        }

        [TestMethod]
        public void CreateIdentifier_ShouldRemoveSymbols()
        {
            var name = "Oh my, what a #$%~// name!";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("OhMyWhatAName", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameHasWhitespace_ShouldDoCamelCasing()
        {
            var name = "My name";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("MyName", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameHasUnderscore_ShouldKeepUnderscore()
        {
            var name = "My_name";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("My_name", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameHasScandinavianCharacters_ShouldNormalizeOutput()
        {
            var name = "ÅÄÖÆØ åäöæø";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("AAOAEOAaoaeo", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameHasDiaeresisCharacters_ShouldNormalizeOutput()
        {
            var name = "ÄËÏÖÜŸ äëïöüÿ";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("AEIOUYAeiouy", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameHasCircumflexCharacters_ShouldNormalizeOutput()
        {
            var name = "ÂÊÎÔÛ âêîôû";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("AEIOUAeiou", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameHasTildeCharacters_ShouldNormalizeOutput()
        {
            var name = "ÃÑÕ ãñõ";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("ANOAno", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameHasGraveOrAcuteCharacters_ShouldNormalizeOutput()
        {
            var name = "ÀÈÌÒÙ àèìòù ÁÉÍÓÚÝŹ áéíóúý";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("AEIOUAeiouAEIOUYZAeiouy", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameHasOtherLatinCharacters_ShouldNormalizeOutput()
        {
            var name = "Ç çç ŠČŇŽĎŤ ščňžďť ĄĘŢŞ ąęţş ŮŻĂ ůżă Ľ ľľ";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("CCcSCNZDTScnzdtAETSAetsUZAUzaLLl", result);
        }

        [TestMethod]
        public void CreateIdentifier_WhenNameHasOtherInternationalCharacters_ShouldNormalizeOutput()
        {
            var name = "Œ œœ Ð ðð Ł łł ßß Ø øø";

            var subject = CreateSubject();

            var result = subject.CreateIdentifier(name);

            Assert.AreEqual("OEOeoeDDdLLlSzszOOo", result);
        }

        private MemberNameValidator CreateSubject()
        {
            return new MemberNameValidator();
        }
    }
}
