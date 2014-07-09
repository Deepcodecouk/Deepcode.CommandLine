# Deepcode.CommandLine

Writing a console app with .net and need to parse out the command line arguments in a simple way? Look no further...

CommandLine is able to parse command lines into a structured format, understanding verbs, switches, positional parameters and more. It has five different levels of functionality, depending on what your requirements are and what you want to achieve. From basic parsing, through to a framework for building command line apps and automatically dispatching control to the appropriate command class. 

You can use as much or as little as you like.

## Parsing

At the lowest level, the parser is able to understand and structure your command line arguments into a list of verbs, switches and parameters, bu calling the Parse method on the CommandLineParser class. This returns a class (CommandLineArguments) which contains the structure of your command line in terms of verbs, switches and switch parameters.

``` C#
// Initialise the parser and parse the string[] args array
var parser = new CommandLineParser();
var arguments = parser.Parse(args);

// Inspect the command line
Console.WriteLine("---Verbs---");
foreach( var verb in arguments.Verbs)
    Console.WriteLine(verb);

Console.WriteLine("---Switches---");
foreach( var switch in arguments.Switches)
{
    Console.WriteLine(switch);
    foreach( var parameter in arguments.Switch(switch) )
    {
        Console.WriteLine("\t{0}", parameter);
    }
}
```

Given the above code, if I invoke my app with the following command line:

```
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
A verb is a command line argument that hasn't been prefixed by a switch. Verbs can only exist prior to a switch.

A switch is denoted by a switch prefix - the default switch prefixes are - and /, but these can be changed. 

A parameter is anything following a switch that isn't a switch itself.

So, the default behaviour is to look at the arguments in the command line - start by adding verbs to the command line until we encounter an argument that starts with the switch prefix (it doesn't matter how many times the switch prefix is specified, so -d, --d and ----d are all equivalent).

Once it encounters a switch, it stops adding arguments to the verbs list, and instead sets up a collection for the switch itself. Each non prefixed argument it then encounters is determined to be a parameter of the switch itself.

### Changing parsing behaviour
The default behaviour should be fine for most cases, but there are some customisation options on the parser that you can use to tweak the behaviour.

####bool CommandLineParser.ParseSwitches
This property defaults to true, setting it to false will ignore the switch parsing behaviour and just bind everything it finds into the verbs collection. Sounds like not much value to that, as it's basically just the equivalent of the string[] you get by default, but when you then consider the binding functionality later, this can be useful for binding.

#### char[] CommandLineParser.SwitchPrefixes
This property defaults to char[]{ '-', '/' } - you can set it to any other char array to override what the parser considers to be a switch prefix.

## Binding
WIP: Binding command line arguments to classes, with some basic type conversion

## Validating
WIP: Using annotations to validate argument classes after binding is complete

## Documenting
WIP: Ability to interrogate argument classes to document usage - for things like "myapp.exe help" and "myapp.exe help verb"...

## Command line based app framework
WIP: An app framework that determines what command class to load based on verbs, binds to the target command class, validates and then executes the command. Also provides automatic usage/help support.



