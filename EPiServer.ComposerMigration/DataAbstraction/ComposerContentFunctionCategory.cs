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
namespace EPiServer.ComposerMigration.DataAbstraction
{
    /// <summary>
    /// This enum exists to support deserializing of the ContentFunctionInstanceCategory enum in Composer
    /// </summary>
    public enum ComposerContentFunctionCategory
    {
        /// <summary>
        /// Unknown category for content functions
        /// </summary>
        Unknown = 0x0,
        /// <summary>
        /// Content category which includes content functions with content, and extension properties only.
        /// </summary>
        Content = 0x1,
        /// <summary>
        /// Layout category which includes content function with available content areas for hosting other content functions.
        /// </summary>
        Layout = 0x2,
        /// <summary>
        /// Combined category which includes content functions with HTML content, extension properties, and available content areas.
        /// </summary>
        Combined = 0x4,
        /// <summary>
        /// Global category which includes global functions.
        /// </summary>
        Global = 0x8,
        /// <summary>
        /// Personalization category which includes personalization functions.
        /// </summary>
        Personalization = 0xF

    }
}
