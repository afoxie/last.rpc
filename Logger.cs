using System;
using System.Collections.Generic;
using System.Text;

public class Logger
{
    private static ConsoleColor def = ConsoleColor.Gray;
    private static ConsoleColor disc = ConsoleColor.Magenta;
    private static ConsoleColor last = ConsoleColor.Red;
    private static ConsoleColor txt = ConsoleColor.White;
    private static void _clr(ConsoleColor clr)
    {
        Console.ForegroundColor = clr;
    }
    public static void LogDiscord(string text)
    {
        _clr(def);
        Console.Write("[");
        _clr(disc);
        Console.Write("Discord");
        _clr(def);
        Console.Write("] ");
        _clr(txt);
        Console.WriteLine(text.ToString());
        _clr(ConsoleColor.Red);
    }
    public static void LogLast(string text)
    {
        _clr(def);
        Console.Write("[");
        _clr(last);
        Console.Write("last.fm");
        _clr(def);
        Console.Write("] ");
        _clr(txt);
        Console.WriteLine(text.ToString());
        _clr(ConsoleColor.Red);
    }
}
