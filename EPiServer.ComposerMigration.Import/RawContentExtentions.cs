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
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.DataAbstraction;
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.ComposerMigration
{
	public static class RawContentExtentions
	{
		public static RawProperty GetProperty(this RawContent content, string propertyName)
		{
			if (content == null || content.Property == null)
			{
				return null;
			}
			return content.Property.FirstOrDefault(p => p.Name == propertyName);
		}

		public static string GetPropertyValue(this RawContent content, string propertyName)
		{
			var property = content.GetProperty(propertyName);
			if (property != null)
			{
				return property.Value;
			}
			return null;
		}

		public static void SetPropertyValue(this RawContent content, string propertyName, string value)
		{
			var property = content.GetProperty(propertyName);
			if (property != null)
			{
				property.Value = value;
			}
			else
			{
				content.Property = content.Property.Concat(new []
				{
					new RawProperty { Name = propertyName, Value = value, }
				}).ToArray();
			}
		}

		public static Guid PageGuid(this RawContent content)
		{
			var property = content.GetProperty(MetaDataProperties.PageGUID);
			return property != null ? Guid.Parse(property.Value) : Guid.Empty;
		}

		public static string PageName(this RawContent content)
		{
			return content.GetPropertyValue(MetaDataProperties.PageName);
		}

		public static void PageName(this RawContent content, string value)
		{
			content.SetPropertyValue(MetaDataProperties.PageName, value);
		}

		public static string PageType(this RawContent content)
		{
			return content.GetPropertyValue(MetaDataProperties.PageTypeName);
		}

		public static void PageType(this RawContent content, string value)
		{
			content.SetPropertyValue(MetaDataProperties.PageTypeName, value);
		}

		public static string Language(this RawContent content)
		{
			return content.GetPropertyValue(MetaDataProperties.PageLanguageBranch);
		}

		public static void Language(this RawContent content, string value)
		{
			content.SetPropertyValue(MetaDataProperties.PageLanguageBranch, value);
		}

		public static string MasterLanguage(this RawContent content)
		{
			return content.GetPropertyValue(MetaDataProperties.PageMasterLanguageBranch);
		}

		public static void MasterLanguage(this RawContent content, string value)
		{
			content.SetPropertyValue(MetaDataProperties.PageMasterLanguageBranch, value);
		}

		/// <summary>
		/// Checks if the current content is either a Composer Page or Composer Function
		/// </summary>
		public static bool IsComposerContent(this RawContent content)
		{
			return content != null &&
				content.Property != null &&
				content.Property.Any(p => p.Name == ComposerProperties.Page || p.Name == ComposerProperties.ContentFunction);
		}

		/// <summary>
		/// Checks if the current content is a composer block.
		/// </summary>
		public static bool IsComposerFunction(this RawContent content)
		{
			return content.GetProperty(ComposerProperties.ContentFunction) != null;
		}

		/// <summary>
		/// Checks if the current content is a page that could contain areas and/or blocks.
		/// </summary>
		public static bool IsComposerPage(this RawContent content)
		{
			return content.GetProperty(ComposerProperties.Page) != null;
		}

		/// <summary>
		/// Checks if the current content is a Composer Container page
		/// </summary>
		public static bool IsComposerContainer(this RawContent content)
		{
			return content.GetProperty(ComposerProperties.ContainerPage) != null;
		}

	}
}
