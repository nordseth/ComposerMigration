EPiServer Composer Migration
======
The Composer Migration project contains Tools to allow content of EPiServer Composer sites to be migrated to EPiServer 7 CMS.

EPiServer 7 was release in October 2012 with new functionality in the CMS that allowed developers and content editors to divide their pages into smaller parts, a feature that was called Blocks. This feature matched in many ways one that was previously available through the use of a separate module called EPiServer Composer. But with version 7 it was integrated as a core CMS feature, making blocks a first class content type, something that didn't allow it to be fully backwards compatible with Composer.

This project contains of two separate tools. One command line code generator user to generate content type classes as descibed by an export package from a Composer site. 
The second part is a module that will transform an export package with content from a Composer site to a CMS compatible format at the time of the import.

This project does not contain any functionality for migrating your existing solution templates to CMS 7 or for migrating Page Type Builder page types to their CMS equivalent.
