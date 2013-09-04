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
using System.Collections.Generic;

namespace EPiServer.ComposerMigration.DataAbstraction
{
    public class ContentType : IEquatable<ContentType>
    {
        private List<PropertyDefinition> _propertyDefinitions;
        public int ID { get; set; }
        public Guid GUID { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string DisplayName { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public int SortOrder { get; set; }
        public AccessControlList ACL { get; set; }
        public ContentTypeCategory ContentTypeCategory { get; set; }
        public List<PropertyDefinition> PropertyDefinitions
        {
            get { return _propertyDefinitions ?? (_propertyDefinitions = new List<PropertyDefinition>()); }
            set { _propertyDefinitions = value; }
        }
        public PageTypeDefault Defaults { get; set; }

        bool IEquatable<ContentType>.Equals(ContentType other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            // For our purposes, this check is good enough.
            return GUID == other.GUID;
        }
    }
}
