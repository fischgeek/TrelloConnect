namespace TrelloConnect

module Utils = 
    let boolOptionToDefault (defaultBool: bool) (optionBool: bool option) = if optionBool.IsSome then optionBool.Value else defaultBool

