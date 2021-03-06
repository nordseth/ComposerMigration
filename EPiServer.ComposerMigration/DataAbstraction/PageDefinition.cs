﻿#region Copyright (C) 2013 EPiServer AB
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EPiServer.ComposerMigration.DataAbstraction
{
    [Serializable]
    public class PageDefinition
    {
        private PageDefinitionType _type;
        private TabDefinition _tab;

        public string Name { get; set; }
        public PageDefinitionType Type
        {
            get { return _type ?? (_type = new PageDefinitionType()); }
            set { _type = value; }
        }
        public bool Required { get; set; }
        public TabDefinition Tab
        {
            get { return _tab ?? (_tab = new TabDefinition()); }
            set { _tab = value; }
        }

        public bool Searchable { get; set; }
        public string EditCaption { get; set; }
        public string HelpText { get; set; }
        public int FieldOrder { get; set; }
        public bool DisplayEditUI { get; set; }
        public bool LanguageSpecific { get; set; }
    }
}
