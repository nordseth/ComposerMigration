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

namespace EPiServer.ComposerMigration.DataAbstraction
{
    [Serializable]
    [Flags]
    public enum AccessLevel
    {
        /// <summary>
        /// No access to an item
        /// </summary>
        NoAccess = 0,
        /// <summary>
        /// Read access to an item
        /// </summary>
        Read = 0x0001,
        /// <summary>
        /// Create access for an item, i e create new items below this item
        /// </summary>
        Create = 0x0002,
        /// <summary>
        /// Change / create new versions of this item
        /// </summary>
        Edit = 0x0004,
        /// <summary>
        /// Delete this item
        /// </summary>
        Delete = 0x0008,
        /// <summary>
        /// Publish/unpublish items and versions of an item
        /// </summary>
        Publish = 0x0010,
        /// <summary>
        /// Set access rights for an item
        /// </summary>
        Administer = 0x0020,
        /// <summary>
        /// Full access for an item
        /// </summary>
        FullAccess = 0x003f,
        /// <summary>
        /// Access level not defined.
        /// </summary>
        /// <remarks>
        /// This enum value is reserved for internal use.
        /// </remarks>
        Undefined = 0x40000000
    }
}
