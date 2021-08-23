module BoardTests

open System
open Xunit
open FsUnit

[<Fact>]
let ``GetBoards - Greater than 0`` () =
    trello().GetBoards() |> Seq.length |> should greaterThan 0