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
	/// <summary>
	/// Predefined sort orders.
	/// </summary>
	public enum FilterSortOrder : int
	{
        /// <summary>
        /// Undefined sort order.
        /// </summary>
		None					= 0,
		/// <summary>
		/// Most recently created page will be first in list
		/// </summary>
		CreatedDescending		= 1,
		/// <summary>
		/// Oldest created page will be first in list
		/// </summary>
		CreatedAscending		= 2,
		/// <summary>
		/// Sorted alphabetical on name
		/// </summary>
		Alphabetical			= 3,
		/// <summary>
		/// Sorted on page index
		/// </summary>
		Index					= 4,
		/// <summary>
		/// Most recently changed page will be first in list
		/// </summary>
		ChangedDescending		= 5,
		/// <summary>
		/// Sort on ranking, only supported by special controls
		/// </summary>
		Rank					= 6,
		/// <summary>
		/// Oldest published page will be first in list
		/// </summary>
		PublishedAscending		= 7,
		/// <summary>
		/// Most recently published page will be first in list
		/// </summary>
		PublishedDescending		= 8
	}
}