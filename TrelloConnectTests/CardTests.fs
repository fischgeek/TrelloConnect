module CardTests

open System
open Xunit
open FsUnit

[<Fact>]
let ``CreateCard - Card Name - Creates Card`` () = 
    trello().CreateCard("", "")
    ()
