
# Developer Console for Godot 4.0+ .NET version 1.0b

This is an implementation of a developer console in Godot 4.0+ .NET using C#. This tool can be used to execute logic in a build that modifies the properties or mechanics of your game without having to change them in the Godot editor and rebuild the project.

This system uses *reflection* to gather all methods decorated with the [ConsoleCommand] attribute.


## License
#### MIT License
Copyright © 2023 hamsterbyte

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)


## Features

- Useful Command Shortcuts
- Several Predefined Console Commands
- Fully Customizable Color Theme
- Works In Any Godot 4.0+ .NET Project
- Fold Output Messages
- Quick Copy Any Output Line To The Console Command Prompt
- Extensible & Well Documented
- Built-in Calculator and Math functions
- Autocomplete and Command History; Press the *up arrow* on the keyboard to copy the last command string to the command prompt
- Crosshair Mode, a GUI based system built on top of the terminal
- Console commands can accept parameters
- Supports async methods and tasks
- Supports prefixes to organize related methods into method groups
- Supports method descriptions to inform the user of their functionality
- Works in build; the system is not confined to the Godot editor

## Limitations

- The system is specifically designed for use in 2D games, but the terminal can easily be extended to support 3D. Extending the crosshair mode will be more difficult and require substantial modifications to the core mechanics of the crosshair system.
- Command parameters must be strings or primitive types (Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single)
- The system technically supports any type that has a Parse() method that allows the type to be parsed from a string. You can include other types by adding an extension method to allow this functionality
   but some characters are restricted as they are used by the command intepreter to decompose the command string: **( ) ,** 


## Installation

Download the zip file included with the main branch and extract it to your project folder. This addon requires an existing project, if you prefer to test things before importing into your project, simply make a new project and import the files there.
Once extracted to the project folder, add res://addons/Developer Console/scene_DeveloperConsole.tscn as an autoload in your project settings to establish the developer console singleton. Make sure that you name the node **DeveloperConsole** to prevent unexpected behaviour.

![image](https://github.com/hamsterbyte/Godot-4.x-.NET-Developer-Console/assets/34613447/dc761cc2-b7a7-46a5-ad51-9a1ad663bf98)


## Enabling The Console
The console is activated by pressing the **`** (left quote) key. The default access level of the console is *WithArg*, more about this below. If you wish to modify the acces level of the developer console this can be done by changing the access level setting found on the root node of the developer console scene. 

![image](https://github.com/hamsterbyte/Godot-4.x-.NET-Developer-Console/assets/34613447/49148d99-ff5d-42bc-a9f1-f553c39a0c7f)

There are 4 different access levels:

| Access Level        | Description                    |
| :------------------ | :---------------------        |
| `None`              | Completely disable the console |
| `Debug`             | Enabled for Editor and Debug builds |
| `WithArg`           | Enabled for Editor and Debug builds. Can be enabled for Release by adding -developer as a command line arg |
| `All`               | Always enabled |

When using the WithArg access level you will be required to execute the application with the **-developer** command line argument if you wish to enable the console. This can be easily done in Windows by creating a shortcut you your executable and appending the argument to the target

![image](https://github.com/hamsterbyte/Godot-4.x-.NET-Developer-Console/assets/34613447/887562fe-61bf-4547-bc96-9ceba442ed90)


    
# Terminal Shortcuts
#### ?

```
  ?{searchString}
```

| Parameter           | Type     | Description                                                 |
| :------------------ | :------- | :---------------------------------------------------------- |
| `searchString`      | `string` | **Optional**. Return only commands that contain this string |

Print a list of all available console commands that contain the search string. If search string is not provided, print a list of all console commands that are valid in the current context, this includes *static* commands which can be called from any context.

---
#### ??

```
  ??
```
Print a list of console commands that are *unique* to the current context. This excludes *static* commands that can be called from any context.

---
#### /

```
  /{path}
```

| Parameter   | Type     | Description                                 |
| :--------   | :------- | :------------------------------------------ |
| `path`      | `string` | **Required**. Path to the desired context   |

Set context to the provided path. If path does not exist, will return an error to the console.

---
#### /.

```
  /.{...}
```

| Parameter   | Type     | Description                                 |
| :--------   | :------- | :------------------------------------------ |
| `...`       | `string` | **Optional**. Additional levels to step up  |

Will set context to parent of the current context. Providing additional .'s will perform this method recursively. Recursion will terminate at the root level.

---
#### \#

```cs
  #{lineNumber}
```

| Parameter          | Type     | Description                                 |
| :--------          | :------- | :------------------------------------------ |
| `lineNumber`       | `int`    | **Required**. Line number to copy           |

Copy a line from the output of the console to the console command prompt. Useful for efficiently executing commands as it eliminates the need for copy/paste


# Adding Console Commands
Console commands can be either *instanced* or *static*. Instanced commands are only available when the context is set to an instance that contains them. Static commands are available from any context. Console commands can be made async, but should return a *Task* object. As the system uses reflection with some fairly generous binding flags, the *scope* of the methods is mostly irrelevant. Be aware of this when you are adding console commands as you may not want to make a method that is not publicly scoped available in the console.

```cs
//Example Instanced Command
[ConsoleCommand]
private void MyConsoleCommand() {
  //Some logic
}

//Example Static Command
[ConsoleCommand]
private static void MyConsoleCommand() {
  //Some logic
}

//Example Instanced async Command
[ConsoleCommand]
private async Task MyConsoleCommand() {
  await Task.Delay(1000)
}

//Example Static async Command
[ConsoleCommand]
private static async Task MyConsoleCommand() {
  await Task.Delay(1000)
}
```
The methods above all return either void or Task, but command interpretation should support *most* return types. If a value is returned from the method the result will be printed automatically to the console.
Below you will see a few more advanced examples, one returns a Task<bool> and one returns a Task and runs asychronously. These are not typical use cases, but they nicely illustrate how extensible this system is.

```cs
 //Check if the provided Int64 is a prime number
 [ConsoleCommand(Prefix = "Math", Description = "Check if a number is prime")]
    private static Task<bool> IsPrime(long number){
        return Task.Run(() => {
                if (number == 2){
                    return true;
                }

                if (number < 2 || number % 2 == 0){
                    return false;
                }

                int squareRoot = (int)Math.Sqrt(number);
                for (long i = 3; i <= squareRoot; i += 2){
                    if (number % i == 0){
                        return false;
                    }
                }

                return true;
            }
        );
    }

    //Print a list of all prime numbers that exist within the range of start to end
    [ConsoleCommand(Prefix = "Math",
      Description = "Print a list of all prime numbers from start to end. Start is always greater than 2.")]
    private static async Task ListPrimes(long start, long end){
        start = start < 3 ? 3 : start;
        for (long i = start; i <= end; i++){
            Task<bool> primeCheckTask = IsPrime(i);
            await primeCheckTask;
            if (primeCheckTask.Result) DC.Print(i);
        }
    }

```

## Prefixes & Descriptions
This system allows you to group console commands and add descriptions of what the command does directly to the console output. Adding a prefix is similar to declaring that the method exists in a specific namespace. Descriptions will be output to the terminal when a help command is executed: ?, ??, ?{searchString}. Descriptions are also visible when mousing over a command in the command tree in crosshair mode.

```cs
//Example Prefix/Description
[ConsoleCommand(Prefix = "Engine", Description = "My Method Description")]
private static void MyConsoleCommand() {
  //Some logic
}
```
Adding a prefix and description to your console commands is optional, but recommended for organization, ease of use, and to prevent naming conflicts.

Instanced commands will **always** have a prefix even if a prefix is not provided, The prefix will be the type of the class from which the method is being executed. For example:

```cs
//Example Instanced Prefix
public partial class MyClass : Node2D {
    [ConsoleCommand(Description = "My Method Description")]
    private void MyConsoleCommand() {
    //Some logic
    }
}
```
The above method would automatically become **MyClass.MyConsoleCommand()** in the console. If a prefix is added to an instanced command it will be inserted at the beginning. A prefix may be **required** on a console command if there is a naming conflict with another existing command as all console commands must have a unique indentifier. Multiple prefixes can be added to a console command similar how you would access inheriting types in C#.

```cs
//Example Nested Prefix
public partial class MyClass : Node2D {
    [ConsoleCommand(Prefix = "MyConsoleCommand.MyOtherConsoleCommand"]
    private void MyConsoleCommand() {
    //Some logic
    }
}
```
This method would be keyed as **MyClass.MyConsoleCommand.MyOtherConsoleCommand.MyConsoleCommand()**. You probably don't want to be this verbose with your naming conventions, but the system does allow it.

Console commands are stored in a **Dictionary<string, MethodInfo>**. The key in this dictionary is the name of the method preceded by the methods prefixes. As dictionaries require unique keys, the console commands must also have a unique Prefix/Name combination. Be aware of this when adding new commands to the console as attempting to add duplicate keys will throw an error. This error is left unhandled to inform you of the naming conflict.

# Printing To The Console

The method for printing output to the developer console is nearly identical to printing output in Godot itself. The following methods can be used to print and format output in the console.

#### DC.Print()
```cs
private void MyMethod(){
    DC.Print({o});
}  
```

| Parameter          | Type        | Description                                 |
| :--------          | :-------    | :------------------------------------------ |
| `o`                | `object`    | **Required**. Object to print               |

#### DC.PrintError()
```cs
private void MyMethod(){
    DC.PrintError({o});
}  
```

| Parameter          | Type        | Description                                                |
| :--------          | :-------    | :------------------------------------------                |
| `o`                | `object`    | **Required**. Object to print formatted as an error message|

#### DC.PrintComment()
```cs
private void MyMethod(){
    DC.PrintComment({o});
}  
```

| Parameter          | Type        | Description                                                |
| :--------          | :-------    | :------------------------------------------                |
| `o`                | `object`    | **Required**. Object to print formatted as a comment       |


If the object is not a string, it must have a ToString() implementation. If you are printing custom objects to the console, this may require an override method to parse the object and create a human readable string that includes the information you want to print.



## Modifying Indent Level
You can also modify the indent level of what is printed in the console. This is useful for printing things in a tree-like structure or grouping output messages. This allows folding in the output console congruent with code folding in an IDE.

#### DC.IncreaseIndent() & DC.DecreaseIndent()
```cs
private void MyMethod(){
    DC.IncreaseIndent();
    DC.Print({o});
    DC.DecreaseIndent();
}  
```
Increasing the indent level is not automatically reverted to the previous level, so if you increase the indent level you must also decrease the indent level to retain proper output formatting. If this is not done it will result in improper output grouping.

---

#### DC.SetIndentLevel()
```cs
private void MyMethod(){
    DC.SetIndentLevel({i});
}  
```
| Parameter          | Type        | Description                                 |
| :--------          | :-------    | :------------------------------------------ |
| `i`                | `int`       | **Required**. Number of levels to indent    |

This method allows you to set the indent level of the console arbitrarily. The specified indent level will be clamped between 0 and **int.MaxValue**. You should favour using the increase/decrease methods unless you absolutely need this functionality as improper use can break output folding.

# Error Handling
The developer console automatically handles any exceptions thrown as a result of executing a console command provided the command itself was called from the console. These errors will be printed to the output of the console and execution will proceed outside the scope of the method. This behaviour is congruent with printing an error message to the console and returning void, or a faulted Task in the case of an async method. 

Exceptions thrown from methods that are not called from the console will behave normally, even if the method is decorated with the **[ConsoleCommand]** attribute.

## Exceptions
The console supports all exceptions, but there are *console specific* exceptions. Most of the console specific exceptions are not relevant to adding console commands as they were created to handle errors that would only be encountered by the command interperter. 

These exceptions include:

**DCInvalidCommandException, DCInvalidParemeterFormatException, DCParameterMismatchException, and DCParseFailureException.**

However, the previous exceptions all inherit from **DCException**, which is the only exception you need to consider when creating console commands.

Throwing exceptions in console commands is *my preferred* method for generating error messages in the console output with this system. It's easy to do, the errors are handled automatically, and execution is automatically returned outside the scope of the method.
Throwing exceptions can affect performance, but should not matter for logic that would normally be found inside of a console command. If you wish to print an error without throwing an exception, that functionality is also available.

```cs
// Method 1
[ConsoleCommand(Prefix = "Node")]
private static void Destroy(){
    if (DC.CurrentNode.GetPath() == "/root") throw new DCException("Cannot destroy root.");
    Node t = DC.CurrentNode;
    DC.ChangeContext(t.GetParent().GetPath());
    OnNodeDestroyed?.Invoke(t);
    t.QueueFree();
    DC.Print($"Node destroyed at '{t.GetPath()}'");
}

// Method 2
[ConsoleCommand(Prefix = "Node")]
private static void Destroy(){
    if (DC.CurrentNode.GetPath() == "/root"){
        DC.PrintError("Cannot destroy root.")
        return;
    }
    Node t = DC.CurrentNode;
    DC.ChangeContext(t.GetParent().GetPath());
    OnNodeDestroyed?.Invoke(t);
    t.QueueFree();
    DC.Print($"Node destroyed at '{t.GetPath()}'");
}
```
In the example above, you'll notice that the first line of logic in Method 1 is checking if the path of the Node is "/root". If it is, we simply throw a new DCException with our custom message. This will automatically print the error message to the console and break execution out of the scope of the method. The second method shows another way to achieve the same functionality without throwing an exception. **Method 1** simply prevents the need for nesting simple print and return logic. The use of DCException is not required as any exception thrown is automatically converted to a DCException to preserve output formatting. The original exception's message will be retained.

Considering this behaviour, it's important to make sure that breaking the execution of the method will not affect further execution out of scope. If it will, you will need to use a try/catch block inside your console command to handle your exception in scope as normal.

# Properties and Wrappers
The DC class has a few useful properties and wrappers that can be used when adding new console commands. They should be used wherever practicable to prevent creating unnecessary references.

### DC.Context
Returns a tuple containing **(Node, Dictionary<string, MethodInfo>)**. This returns the same as **CommandInterpreter.Context**

*Context* is the combination of the current node as well as all valid console commands that can be called on that node.

### DC.CurrentNode
Returns a reference to the current context's node

### DC.Instance
Returns a reference to the DeveloperConsoleUI singleton

### DC.Root
Returns a reference to the root node of the current scene

### DC.ChangeContext()
Change context to the node at the provided path.
```cs
private void MyMethod(){
    DC.ChangeContext({path})
}  
```
| Parameter          | Type        | Description                                 |
| :--------          | :-------    | :------------------------------------------ |
| `path`             | `string`    | **Required**. Path to node in new context   |

---
With the properties and wrappers listed above you should have no need to access the command interpreter class directly. Modifying the command interpreter class should only be done if you are experiencing errors that cannot be resolved without doing so. Before modifying the command interpreter class make sure you look through the code thoroughly and understand exactly how it works as this is where some of the more complex parts of the system are being handled.
