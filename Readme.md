This is a sample template for a peachpie site where the php code resides in ```src\Website\wwwroot```

The benefit is you don't need any boilerplate code in the ```wwwroot``` folder.

A few things of note:

- At the moment a redirect is done on .php files to get the modified base path working.
- Having trouble resolving MimeTypes so have a custom definition setup (need to fix). ```src\Server\MimeTypes.cs```
- If your site needs to scan for php files then you need to copy the php files to the output folder (already configured in Website.msbuildproj) i.e. wordpress in detecting plugins/themes
- Any files which depend on files which may not exist i.e. themechecks wordpress plugin will fail to compile due to missing inheritance, to overcome this exclude the files which depend on the missing files. There are some default Exclusions setup for any files inside folders called ```test``` or ```/themecheck/```
- This project will download the latest development version of peachpie and ImageSharp from MyGet. If you don't want this remove the ```NuGet.Config``` file or modify it to your preferred source.
- The peachpie version is configured in Common.props

If you want to add files uniquely for peachpie project you can create a folder outside wwwroot and include it inside Website.msbuildproj. This is useful for patching missing functions or fixing bugs without altering the original source.

An example is for wordpress, you can create a folder called ```Patches``` inside ```src\Website``` and create a file called ```dummy-class-PEAR_Error.php```
with the following:
```
<?php

/**
 * Dummy class created in order to compile WordPress by Peachpie.
 */
class PEAR_Error {}
```

This will add the PEAR_Error class which isn't implemented allowing classes which derive from it to still compile.

In this case I just needed to create the class, but you may need to implement functions as required.

Speaking of wordpress you'll also want to create a 2nd patch file called: ```dummy-class-wp-cli-command.php```
with the following:
```
<?php

/**
 * Base class for WP-CLI commands
 *
 * Downloaded from https://github.com/wp-cli/wp-cli/blob/master/php/class-wp-cli-command.php to make
 * WordPress able to be compiled in Peachpie.
 *
 * @package wp-cli
 */
abstract class WP_CLI_Command {

	public function __construct() {}
}
```

With those two changes you should be able to compile wordpress.