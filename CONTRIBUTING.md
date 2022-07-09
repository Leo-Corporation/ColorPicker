# Contribution
## Summary
- [Knowledge](#knowledge)
- [Tools](#tools)
- [Writing code](#writing-code)
## Knowledge
To contribute to this project, you will need to have some prerequisites:

- A basic knowledge of C# (this project is written in C# 9)
- A basic knowledge of XAML
- A basic knowledge of Visual Studio

## Tools
You will also need to have the following tools:

- Microsoft Visual Studio 2019
  - .NET Desktop Development
  - Microsoft Blend
- Git
- (*optional*) Microsoft Visual Studio Code

## Writing code
Make you follow the following guidelines:

1) Use Tabs: To format your code, use tabs instead of spaces:
~~~ cs
class Car
{
    /// <summary>
    /// The maximum speed of the car.
    /// </summary>
    public int MaxSpeed { get; set; }
    
    /// <summary>
    /// This method does stuff.
    /// </summary>
    public void DoStuff()
    {
        Console.WriteLine("DoStuff"); // Print text
    }
}
~~~
2) Put your code between `{ }`:
~~~ cs
// Do this
int x = 12; // Define a number
int y = 45; // Define another number

if (x < y) // If y is bigger than x
{
    Console.WriteLine("y is bigger than x"); // Print text
}

// Don't do this
if (x < y) // If y is bigger than x
    Console.WriteLine("y is bigger than x"); // Print text
~~~
3) Comment your code:
~~~ cs
int a = 10; // Define a number
int b = 15; // Define another number

if (a > b) // If a is bigger than b
{
    //TODO
}
else
{
    //TODO
}
~~~
4) Use XML Documentation for ``public`` and ``internal`` methods, fields and properties:
~~~ cs
/// <summary>
/// This method does stuff.
/// </summary>
internal void DoStuff()
{
    Thread.Sleep(2000); // Do nothing for 2 seconds
}
~~~
5) Do not forget to put the [license](https://github.com/Leo-Corporation/Gavilya/blob/master/LICENSE) header on each `.cs` files:
~~~ cs

/*
MIT License

Copyright (c) Léo Corporation

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
*/
~~~
6) Use camel case for variables and pascal case for properties and method:
~~~ cs
int myInt = 0; // Declare an int
private int MyInt { get; set; } // Declare an int property

private void DoStuff()
{
    myInt = 2; // Set myInt to 2
}
~~~
That's pretty much all you need right now. Keep in mind this document can be updated at any time, so make sure to keep checking these guidelines.
