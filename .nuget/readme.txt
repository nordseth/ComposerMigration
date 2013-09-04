== EPiServer Composer Migration ==
This package adds the binaries needed to Import an .episerverdata package exported from an Composer site into a CMS 7 site.

Please refer to documentation on http://world.episerver.com/composermigration/ for instructions on the migration process.

=== Workflow overview ===
1. Export data from the Composer site
	1.1 Export Page types
	1.2 Export settings (Frames, Tabs,  Categories and Visitor Groups)
	1.3 Export content (Pages and Dynamic Properties)
2. Create a new empty EPiServer 7 CMS solution
	2.1 Import settings package
	2.2 Configure site
	2.3 Install the EPiServer.ComposerMigration package (You should be here right now!)
	2.4 Migrate users and groups (optional)
3. Migrate the solution
	3.1 Copy the old solution files and configuration.
	3.2 Generate Content Types classes and add to solution
	3.3 Refactor Composer templates
4. Migrate the content 
	4.1 Import the content package 
5. Remove the EPiServer.ComposerMigration package from your site.