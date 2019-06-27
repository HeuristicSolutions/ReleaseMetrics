README for HS Library

1. What is the HS Library?
2. Organization
3. Dependencies
4. Installing
5. Hacking
6. Testing
7. Building & Releasing


==============================================================================
1. What is the HS Library?

The HS Library (hereafter referred to as HSLib) is a dll for the 
Microsoft .NET Framework platform. It includes classes and 
graphical controls that are included in every HS project and are 
intended to speed development, standardize how we do things, 
remove duplication, among other things.


===========================================================================
2. Organization

./src

  ./src/guis
    GUIs for using/testing various helper classes.

  ./src/Heuristics.Library.Cs
    Library features written in C#. All new additions to the library should
	go here.

    ./src/Heuristics.Library.Cs/Biz
       Classes for the LINQ and gateway-based DAL.

    ./src/Heuristics.Library.Cs/DesignByContract
       Classes for enforcing code contracts.

    ./src/Heuristics.Library.Cs/Extensions
       Static classes that expose extension methods. Generally speaking, 
	   methods should be organized according to the type of object they extend.
	   For instance, string extensions go in StringExtensions.cs.  An accepted
	   exception to this rule is when there is a large number of extensions that 
	   do essentially the same thing and operate on a large number of types. These
	   can be consolidated into a single class if it seems appropriate.

    ./src/Heuristics.Library.Cs/Interfaces
       Interface declarations for common abstractions.

    ./src/Heuristics.Library.Cs/Sys
       Infrastructure pieces, such as attribute classes or default implementations
	   of interfaces. These classes should focus on framework level concerns, there
	   shouldn't be any real "business" logic implemented here.

    ./src/Heuristics.Library.Cs/Utility
       General purpose utility classes.

	   
  ./src/Heuristics.Library.Cs.Tests
    Tests for the code in the C# library.

	
  ./src/Heuristics.Library.Mvc
    Library code that relates to the Microsoft ASP.NET MVC framework. This exists as
	its own project so that the core library does not carry a dependency on a specific
	version of the MVC framework, which is released out-of-band relative to ASP.NET.

    ./src/Heuristics.Library.Mvc/Extensions
       MVC-specific extension methods.

    ./src/Heuristics.Library.Mvc/ModelBinding
       Base classes and helpers for writing custom model binders.

	   
  ./src/Heuristics.Library.Mvc.Tests
    Tests for the MVC library.

	
  ./src/Heuristics.Library.Testing
    Library features that support test projects. These features are in their own
	project because they have dependencies (like NUnit and RhinoMocks) that we don't 
	want to attach to the core library. 

    ./src/Heuristics.Library.Testing/Data
       Helper classes and features for writing data tests that interact with
	   existing database instances.

    ./src/Heuristics.Library.Testing/Validation
       Classes for testing validation rules.

	   
  ./src/Heuristics.Library.Testing.Mvc
    Library features that support the testing of ASP.NET MVC projects.

    ./src/Heuristics.Library.Testing.Mvc/ControllerTesting
       Classes that support testing Controllers and ActionResults.

    ./src/Heuristics.Library.Testing.Mvc/ModelBinding
       Classes for testing ModelBinder classes.

    ./src/Heuristics.Library.Testing.Mvc/Routing
       Classes for testing routes.


  ./src/Heuristics.Library.Vb
    Library features written in VB. These are legacy features that are still
	supported but are not under active development.

    ./src/Heuristics.Library.Vb/guicontrols
		HS standard GUI controls.

    ./src/Heuristics.Library.Vb/utility
	  General library features.

		./src/Heuristics.Library.Vb/utility/dataaccess
			Legacy DAL.

		./src/Heuristics.Library.Vb/utility/masterone
			Master page implementation for .NET 1.1. Obsolete.

		./src/Heuristics.Library.Vb/utility/payment
			An implementation of Authorize.NET for working with credit card payments.

		./src/Heuristics.Library.Vb/utility/replacemore
		The ReplaceMore templating and token replacement utility.

		./src/Heuristics.Library.Vb/utility/validator
			Helper class for validating data.

			
  ./src/Heuristics.Library.Vb.Tests
    Tests for the code in the VB library.



==============================================================================
3. Dependencies

- Microsoft .NET 4.0
- hs_default_template.zeus >= rel-0.8.4
- Microsoft ASP.NET MVC 3  	(used by the *.Mvc projects. Currently references MVC 2 RC)
- NUnit		   				(used by the Testing library and test projects)
- RhinoMocks				(used by the Testing library and test projects)
- MVC Contrib				(used by the Testing library)


==============================================================================
4. Installing

To use HSLib in your application you should setup the following 
external:

Your Application             External URL
-----------------------------------------------
/src/lib/HSLib           ->  /heuristics/library/latest-v4.0/lib


==============================================================================
5. Hacking

To start hacking on HSLib you need to do the following:

{To be written}


==============================================================================
6. Testing

There are separate test projects for the C# and VB library code. Each test project
contains some integration tests that require environment-specific configuration. To
run the tests you must:

	1) Create an empty database to use for the data access tests.
	
	2) In the root of the solution, copy Heuristics.Library.Tests.custom.config.tmpl
	   to Heuristics.Library.Tests.custom.config.
	   
	3) Edit this custom config file and set the connection string to the database
	   you created in step 1.
	   
	4) Edit this custom config file and set the email addresses to use when testing
	   MailPlus.
	   
	5) To run the tests, use the nunit_test_runner.bat file to load NUnit. This will
       use the NUnit console .exe that ships with this project and will load the 
       NUnit project file.



==============================================================================
7. Building & Releasing

The build script is ./src/rakefile.rb. You must have Ruby and Rake available 
to build this project. The release target will create the SVN tag, build the project, 
and update the "latest" folder with a binary release.

- Output help on how to use it:
    rake usage

- Output list of available tasks:
    rake -T

- Clean up the dev environment:
    rake clean

- Do an SVN update and rebuild the working copy
    rake fast_update

- Execute the tests against the current working copy:
    rake test

- Execute the release task:
    rake release version=x.x.x

	Assembly Names
	------------------------
	When the library is released, the C# and VB library assemblies are merged together
	into a single Heuristics.Library.dll. This happens during release, not on every build,
	so that it's easier to test and manage the library in its native state.

	Versioning
	------------------------
	This is the .NET 3.5 branch of the library. The rakefile will automatically prepend
	"v3.5-" to the version that you specify. So if you do "rake release version=1.0.0",
	rake will create tags called "rel-v3.5-1.0.0" and "rel-v3.5-1.1.1-bin". 
	
	In addition, releasing this library will automatically update the "latest-v3.5" branch
	in SVN. At this time, the "latest" branch is reserved for the .NET 1.1 version of the
	library so that older projects may continue to reference it.
