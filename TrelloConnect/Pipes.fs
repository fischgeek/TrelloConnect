namespace TrelloConnect

open System

[<AutoOpen>]
module Pipes = 
    type Board = 
        { Id: string
          Name: string }
        static member GetBoardById id = ()

    and CardPipe = 
        static member CreatedDate (cardId: string) = 
            let firstEight = cardId.Substring(0, 8)
            let converted = System.Convert.ToInt64(firstEight, 16)
            let dt = DateTimeOffset.FromUnixTimeSeconds(converted).DateTime
            dt

    and StringPipe = 
        static member NoneToBlank (str: string option) = if str.IsSome then str.Value else ""
        static member PrependIfNotEmpty (whatToPrepend: string) (string: string) = if string |> String.IsNullOrEmpty then "" else whatToPrepend + string
        static member Join delimiter (text: string seq): string = String.Join(delimiter, text)
        static member Lines(text: string): string [] = System.Text.RegularExpressions.Regex.Split(text, @"\r\n|\n\r|\n|\r")
        static member Prepend (whatToPrepend: string) (string: string) = whatToPrepend + string
        static member Trim(text: string): string = if isNull text then "" else text.Trim()
        static member IsNotEmptyOrWhitespce = StringPipe.Trim >> String.IsNullOrEmpty >> not
        static member ExcludeBlankLines(lines: string seq): string seq = lines |> Seq.filter StringPipe.IsNotEmptyOrWhitespce
        static member Append (whatToAppend: string) (appendTo: string) = appendTo + whatToAppend
        static member IsEmpty x = String.IsNullOrEmpty x
        static member RemoveBlankLines(text: string): string =
            text
            |> StringPipe.Lines
            |> StringPipe.ExcludeBlankLines
            |> String.concat "\r\n"

    and SP = StringPipe


