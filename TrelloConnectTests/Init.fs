[<AutoOpen>]
module Init

open FSharp.Data
open System.IO
open TrelloConnect

[<Literal>]
let CfgString = 
    """
    {
        "ApiKey": "string",
        "ApiToken": "string",
        "TestBoardId": "string",
        "TestListId": "string",
        "TestCardId": "string"
    }
    """
type Config = JsonProvider<CfgString, RootName="Config">
let trello () =
    let cfg = File.ReadAllText(@"c:\dev\config\trelloconnect.json") |> Config.Parse
    new TrelloWorker(cfg.ApiKey,cfg.ApiToken)

