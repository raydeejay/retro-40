XClasses is a simple IDL written in Python.
You declare your classes, methods, and members as Python objects,
and XClasses will generate the .c, .h, and .f files for you.
Not only do the Forth classes line up with their C counterparts
exactly, but all non-static methods (virtual and non-virtual)
are *automatically* thunked.  In other words, any method
declared in XClasses and implemented in C can be called from
the matching Ficl class, and the C method will be automatically
called with the correct arguments.  XClasses handles floating-point
arguments too!

Known limitations:
	* All arguments must be one cell wide.  (That means
	  only single-precision floats, too.)



To use:
	* Declare all your classes in a .py script
	* Include "xclasses.h" everywhere you need your classes
	* Include xclasses.cpp in your project somewhere
	* Call
		"xclasses.f" included
	  from your Ficl initialization script

And you're mostly done!

Simply including xclasses.f is not enough, though.  You must
explicitly instantiate your classes.  This is to allow you a
chance to add your own methods to the class.  For a class
named "myCallback", it would look like this:

	declare-myCallback
		end-myCallback

You also have to define the "init" function for the class.
Most of the time this will work fine:

	declare-myCallback
		use-default-init
		end-myCallback


The "default" init function calls the super class's init
function, then sets all data members to 0.  If this isn't
what you want, roll your own.  The init function takes
the usual 2-cell "this" pointer as its argument:

	declare-myCallback
		: init ( 2:this ) ...
			;
		end-myCallback

For a do-nothing init function, you'll want this:

	declare-myCallback
		: init 2drop ;
		end-myCallback


Here's a random example from the simple game I'm working on:

-----------------------------------------------------------------
skinStream = xMethod("stream", "void").setVirtual(1)
gjeSkin.addMethod(skinStream)

##
## gjeTexture
##
##
gjeTexture = xClass("gjeTexture")
gjeTexture.setParent(gjeSkin)
gjeTexture.addMethod(skinStream)
gjeTexture.addMethod(xMethod("clear", "void"))
gjeTexture.addMember(xVariable("texture", "LPDIRECT3DTEXTURE8"))
gjeTexture.addMember(xVariable("name", "char *"))
gjeTexture.addMember(xVariable("variation", "int"))
gjeTexture.addMember(xVariable("material", "D3DMATERIAL8 *"))

-----------------------------------------------------------------

I sure hope that's enough to get you started.



Random notes:
* XClasses doesn't deal well with struct-packing issues.  It assumes
  pretty much everything will be 4-byte aligned.  This can bite you
  if you add a 64-bit int... the C compiler may align it for you,
  and XClasses won't know about it.  (This could be fixed in a future
  release... are you volunteering?)  For now, it's best to declare
  your classes such that 64-bit ints are naturally 8-byte aligned.

* If you don't want to have to declare each class in turn,
  you can add something like this to the end of your Python script:
-----
def declare(name):
	xAddFiclFooter("\t\"" + name + ".constant\" \" sfind swap drop 0= [if] declare-" + name + " use-default-init end-" + name + " [endif] \" evaluate")


xAddFiclFooter(": xclassesDeclare")
for c in classes:
	declare(c.name)
xAddFiclFooter("\t;")
-----
  and then call "xclassesDeclare" from your Ficl script just after
  including "xclasses.f".


--lch
larry@hastings.org
