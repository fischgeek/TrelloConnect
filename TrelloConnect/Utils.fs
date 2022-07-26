namespace TrelloConnect

module Utils = 
    let out (s: string) = System.Console.WriteLine(s)
    let boolOptionToDefault (defaultBool: bool) (optionBool: bool option) = if optionBool.IsSome then optionBool.Value else defaultBool

