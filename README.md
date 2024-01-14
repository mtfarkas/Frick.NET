# Frick.NET

## What is this?
This is a simple [Brainfuck](https://github.com/sunjay/brainfuck/blob/master/brainfuck.md) interpreter written in .NET. It's not the first, certainly not the best, but it's mine.

## Why?
I was bored.

## How do I use it?
You can use Frick.NET in two ways: consume it as a class library or use the CLI.

### Using the class library:
```csharp
using Frick.NET; 

string source = "<your Brainfuck code goes here>";
FrickInterpreter interpreter = new();
interpreter.Run(source);
```

### Using the CLI:
1. Build the CLI from source or download it from the Releases tab.
2. You can pass Brainfuck code in the following ways:
   * Pass a file name as an argument: `frick-cli -i <FILENAME>`
   * Pass a file's content piped from STDIN: `'<Brainfuck goes here>' > frick-cli`
   * Run `frick-cli` without any arguments and enter the code using the terminal
