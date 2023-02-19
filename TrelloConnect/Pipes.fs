namespace TrelloConnect

open System
open Microsoft.FSharp.Reflection
open System.IO

[<AutoOpen>]
module Pipes = 
    [<Obsolete("Is this needed?", true)>]
    type Board = 
        { Id: string
          Name: string }
        static member GetBoardById id = ()

    type CardPipe = 
        static member CreatedDate (cardId: string) = 
            let firstEight = cardId.Substring(0, 8)
            let converted = System.Convert.ToInt64(firstEight, 16)
            let dt = DateTimeOffset.FromUnixTimeSeconds(converted).DateTime
            dt

    and DateTimePipe = 
        static member private pad2 i =
            if i <= 9 then
                sprintf "0%i" i
            else
                i.ToString()

        static member private pad3 (i: int) : string =
            if i <= 9 then
                sprintf "00%i" i
            else if i <= 99 then
                sprintf "0%i" i
            else
                i.ToString()
        static member ToCompressedStamp (d: DateTime) : string =
            sprintf 
                "%i%i%i%s%s%s%s" 
                d.Year 
                d.Month 
                d.Day 
                (DateTimePipe.pad2 d.Hour) 
                (DateTimePipe.pad2 d.Minute) 
                (DateTimePipe.pad2 d.Second) 
                (DateTimePipe.pad3 d.Millisecond)

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

    and FilePipe =
        static member WriteTextRandomNamePrefix (prefix: string) (dir: string) (extNoDot: string) (text: string) =
            let guid = (Guid.NewGuid()).ToString()
            //let name = DateTime.Now |> DateTimePipe.ToCompressedStamp
            let nwp = $"{prefix}_{guid}"
            let di = dir |> DirectoryInfo
            di.Create()
            let fi = Path.Combine(di.FullName, $"{nwp}.{extNoDot}") |> FileInfo
            File.WriteAllText(fi.FullName, text)
            fi

    and UnionPipe = 
        static member FromStringFull<'a>(s: string) (comparison: string -> string -> bool): 'a option =
            match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun x -> x.Name |> comparison s) with
            | [| case |] -> Some(FSharpValue.MakeUnion(case, [||]) :?> 'a)
            | _ -> None

        static member FromString<'a>(s: string) =
            UnionPipe.FromStringFull<'a> s (=)
            
        static member FromStringCI<'a>(s: string) =
            let s = s.ToLower()
            UnionPipe.FromStringFull<'a> s (fun (a: string) s -> a.ToLower() = s.ToLower())

