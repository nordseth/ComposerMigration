#region Copyright (C) 2013 EPiServer AB
/*
Permission is hereby granted, free of charge, to any person obtaining a copy of this 
software and associated documentation files (the "Software"), to deal in the Software 
without restriction, including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/
#endregion
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.ComposerMigration.CodeGeneration
{
    public static class CodeDomExtensions
    {
        public static readonly char SnippetIndicator = '$';

        public static void AddArgument(this CodeAttributeDeclaration attribute, string name, string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            CodeExpression expression;
            // Use convention that if string starts with $ its a const
            if (value.StartsWith(SnippetIndicator.ToString()))
            {
                expression = new CodeSnippetExpression(value.TrimStart(SnippetIndicator));
            }
            else
            {
                expression = new CodePrimitiveExpression(value);
            }
            attribute.AddArgument(name, expression);
        }

        public static void AddArgument(this CodeAttributeDeclaration attribute, string name, object value)
        {
            attribute.AddArgument(name, new CodePrimitiveExpression(value));
        }

        public static void AddArgument(this CodeAttributeDeclaration attribute, string name, CodeExpression valueExpression)
        {
            attribute.Arguments.Add(new CodeAttributeArgument(name, valueExpression));
        }

        public static CodeAttributeDeclaration AddCustomAttribute(this CodeTypeMember member, string name)
        {
            var attr = new CodeAttributeDeclaration(name);
            member.CustomAttributes.Add(attr);
            return attr;
        }

        public static CodeAttributeDeclaration AddCustomAttribute(this CodeTypeMember member, string name, Type value)
        {
            return member.AddCustomAttribute(name, new CodeTypeOfExpression(value));
        }

        public static CodeAttributeDeclaration AddCustomAttribute(this CodeTypeMember member, string name, string value)
        {
            CodeExpression expression;

            // Use convention that if string starts with $ its a const
            if (value.StartsWith(SnippetIndicator.ToString()))
            {
                expression = new CodeSnippetExpression(value.TrimStart(SnippetIndicator));
            }
            else
            {
                expression = new CodePrimitiveExpression(value);
            }

            return member.AddCustomAttribute(name, expression);
        }

        public static CodeAttributeDeclaration AddCustomAttribute(this CodeTypeMember member, string name, object value)
        {
            return member.AddCustomAttribute(name, new CodePrimitiveExpression(value));
        }

        public static CodeAttributeDeclaration AddCustomAttribute(this CodeTypeMember member, string name, CodeExpression valueExpression)
        {
            var attr = new CodeAttributeDeclaration(name, new CodeAttributeArgument(valueExpression));
            member.CustomAttributes.Add(attr);
            return attr;
        }


    }
}
