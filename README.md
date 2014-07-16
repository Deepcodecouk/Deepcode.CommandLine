# Deepcode.CommandLine

Writing a console app with .net and need to parse out the command line arguments in a simple way? Look no further...

CommandLine is able to parse command lines into a structured format, understanding verbs, switches, positional parameters and more. It has five different levels of functionality, depending on what your requirements are and what you want to achieve. From basic parsing, through to a framework for building command line apps and automatically dispatching control to the appropriate command class.

You can use as much or as little as you like.

## Parsing

At the lowest level, the parser is able to understand and structure your command line arguments into a list of verbs, switches and parameters, bu calling the Parse method on the CommandLineParser class. This returns a class (CommandLineArguments) which contains the structure of your command line in terms of verbs, switches and switch parameters.

```csharp
// Initialise the parser and parse the string[] args array
var parser = new CommandLineParser();
var arguments = parser.Parse(args);

// Inspect the command line
Console.WriteLine("---Verbs---");
foreach( var verb in arguments.Verbs)
    Console.WriteLine(verb);

Console.WriteLine("---Switches---");
foreach( var s in arguments.Switches)
{
    Console.WriteLine(s);
    foreach( var parameter in arguments.Switch(s) )
    {
        Console.WriteLine("\t{0}", parameter);
    }
}
```

Given the above code, if I invoke my app with the following command line:

```sh
myapp.exe add . -r --m:"Some message" /m "Another message" /d 100 200
```

The output would be

```
---Verbs---
add
.

---Switches---
r
m
    Some message
    Another message
d
    100
    200
```

### What's a verb, a switch and a parameter
A verb is a command line argument that hasn't been prefixed by a switch. At the moment, verbs can only exist prior to a switch.

A switch is denoted by a switch prefix, the default switch prefixes are `-` and `/`, but these can be changed.

A parameter is anything following a switch that isn't a switch itself.

So, the default behaviour is to look at the arguments in the command line - start by adding verbs to the command line until we encounter an argument that starts with the switch prefix (it doesn't matter how many times the switch prefix is specified, so `-d`, `--d` and `----d` are all equivalent).

Once it encounters a switch, it stops adding arguments to the verbs list, and instead sets up a collection for the switch itself. Each non prefixed argument it then encounters is determined to be a parameter of the switch itself.

### Changing parsing behaviour
The default behaviour should be fine for most cases, but there are some customisation options on the parser that you can use to tweak the behaviour.

####bool CommandLineParser.ParseSwitches
This property defaults to true, setting it to false will ignore the switch parsing behaviour and just bind everything it finds into the verbs collection. Sounds like not much value to that, as it's basically just the equivalent of the `string[]` you get by default, but when you then consider the binding functionality later, this can be useful for binding.

#### char[] CommandLineParser.SwitchPrefixes
This property defaults to `char[]{ '-', '/' }` - you can set it to any other char array to override what the parser considers to be a switch prefix.

## Binding
The binding namespace adds additional functionality to allow classes to be bound to the parsed command line verbs, switches and arguments, with some rudimentary type conversion.

Assuming we want to bind a class to a command line that might look like this:

```sh
myapp.exe delete -match *.txt temp_*.* -r -verbosity 3
  or
myapp.exe delete -m *.txt temp_*.* -recurse false -v 3

```

We can define a class that we want to bind to as follows;

```csharp
public class FileCommandParameters
{
    public string Action{ get; set; }
    public string[] MatchMasks{ get; set; }
    public bool Recurse{ get; set; }
    public int Verbosity{ get; set; }
}
```

We could now just go ahead and use the parser to interrogate the command line and create this object, or, we can bind to it by adding some binding attributes.

```csharp
public class FileCommandParameters
{
    [ParameterVerb]
    public string Action{ get; set; }

    [ParameterAlias("match")]
    [ParameterAlias("m")]
    public string[] MatchMasks{ get; set; }

    [ParameterAlias("recurse")]
    [ParameterAlias("r")]
    public bool Recurse{ get; set; }

    [ParameterAlias("verbosity")]
    [ParameterAlias("v")]
    public int Verbosity{ get; set; }
}
```

We can then bind the command line to it as follows;

```csharp
var parser = new CommandLineParser();
var arguments = parser.Parse(args);
var parameters = new FileCommandParameters();
var binder = new CommandLineBinder();

binder.BindTo(arguments, parameters);

// Parameters will now contain the values from the command line...
```

Ok that's all a bit long winded. You can short circuit some of that and have the binder create the instance of the object too;

```csharp
var parser = new CommandLineParser();
var arguments = parser.Parse(args);
var binder = new CommandLineBinder();

var parameters = binder.CreateAndBindTo<FileCommandParameters>(arguments);
```

Or, using the `string[]` extension methods, you can do it all in one hit;

```csharp
var parameters = args.ParseAndBindCommandLine<FileCommandParameters>();
```

Note - when using the extension method, you don't get the opportunity to override the defaults of the parser, so anything beyond the defaults is left as an exercise for the reader :)

### Multiple verbs and positional verb binding
The binding sample above assumes a single verb. If you want to allow multiple verbs to be captured, then the `string Action` could just be re-written as `string[] Action` - this would then capture all the verbs passed in.

More complex verb bindings are possible, including binding verbs to multiple properties in the target by using the additional parameters on the `ParameterVerbAttribute` including - `int StartPosition`, `int EndPosition`, and `bool Greedy`. Eg:

```csharp
public class TargetBindingTyped
{
	[ParameterVerb]
	public string[] GreedyVerbs { get; set; }

	[ParameterVerb(StartPosition=2)]
	public int SecondVerb { get; set; }

	[ParameterVerb(StartPosition=3)]
	public bool ThirdVerb { get; set; }

    [ParameterVerb(StartPosition=1, EndPosition=2)]
    public string[] FirstAndSecondVerbs{ get; set; }

    [ParameterVerb(StartPosition=2, Greedy=true)]
    public string [] SecondVerbOnwards{ get; set; }
}
```

#### A little explaination.

The extra parameters on the `ParameterVerbAttribute` control where in the array of parsed verbs the target property should get it's values from. The default values for these additional parameters are - `Greedy=false`, `StartPosition=1`, `EndPosition=Int32.MaxValue`.

This should be quite self explanatory, but, here's how the various properties above would be bound.

*GreedyVerbs* - as it's an array, with no additional parameters on the `ParameterVerb`, this will be populated with all the verbs passed on the command line.

*SecondVerb* - as the `StartPosition` is set, this will find and populate the second verb passed in. Even if this was an array, it would only get the second verb - specifying a start position will also default the end position to just get that one verb.

*ThirdVerb* - as with SecondVerb, this will just get the verb at position 3.

*FirstAndSecondVerbs* - This has a start and end specified, so this will retrieve the verbs at positions 1 to 2, inclusive.

*SecondVerbOnwards* - This has a start, but greedy is set true. This will grab all verbs from position 2 onwards. If we were to pass 15 parameters, this would get all parameters between positions 2 and 15.


### The behaviour of the binder
The binder, as of right now, can bind strings, ints, bools and arrays of same, both on verbs and on switch parameters. Initially parameters are read as strings, and converted based on the type of property it targets.

If a target property is a bool and no switch arguments were provided on the command line, then the presence of the switch/verb will default the target property to true. That means, the following command lines are all equivalent;

```sh
myapp.exe -v
myapp.exe -v true
myapp.exe -v:true
```

### Unbound errors in the binder
The binder will happily try to bind everything it finds and, hopefully, won't blow up on you. However, it will tell you about any issues it may have discovered, specifically with parameters passed on the command line that couldn't be bound to anything in the target.

The collection of these unbound errors can be accessed via the binders `UnboundErrors` collection. This is just a simple list of strings describing things that could not be bound for example;

```sh
myapp.exe verb1 verb2 -yes -no1 -no2 param1 param2 -no3
>>
    Unknown verb [verb2] as position 2
    Unknown switch option [no1]
    Unknown switch option [no1] with parameters [param1 param2]
    Unknown switch option [no3]
```

## Validating
WIP: Using annotations to validate argument classes after binding is complete

## Documenting
WIP: Ability to interrogate argument classes to document usage - for things like "myapp.exe help" and "myapp.exe help verb"...

## Command line based app framework
WIP: An app framework that determines what command class to load based on verbs, binds to the target command class, validates and then executes the command. Also provides automatic usage/help support.
