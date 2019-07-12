We're managing SQL views by manually modifying the migrations to create them, but removing a migration deletes the file;
so if the only place that the view is defined is within the migration itself, reverting a migration will discard the view
schema as well.

So, we're tracking the manual SQL that we want to run in this folder, one file per migration.

These files are tracked as embedded resources so that they don't actually get deployed anywhere and are easy to access.

=================================
TO ADD A NEW CUSTOM SQL SCRIPT
=================================
To use manual sql in a migration:

	1) Create the migration, so we know its name

	2) Create a "<Migration>_Up.sql" and "<Migration>_Down.sql" script that add and revert your changes

	3) Make your files EMBEDDED RESOURCES!

	4) Add this line to the BOTTOM of the "Up" method in the migration class:
		MigrationHelper.ExecuteCustomSql(migrationBuilder, "<MigrationName>_Up.sql");

	5) Add this line to the TOP of the "Down" method in the migration class:
		MigrationHelper.ExecuteCustomSql(migrationBuilder, "<MigrationName>_Down.sql");
