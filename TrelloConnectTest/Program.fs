namespace TrelloConnect

open System
open TrelloConnect
open FSharp.Data
open System.IO

module Main = 
    type Config = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trelloconnect/config.sample.json", RootName="Config">
    let cfg = File.ReadAllText(@"c:\dev\config\trelloconnect.json") |> Config.Parse
    let wait() = Console.ReadLine() |> ignore
    let outTitle t meta = printfn "%s (%s)" t meta
    let outData (label: 'a) data = 
        printfn "\t%s: %s" (label.ToString()) data

    [<EntryPoint>]
    let main argv =
        let trello = new TrelloWorker(cfg.ApiKey, cfg.ApiToken)
        let boardId = cfg.TestBoardId
        let listId = cfg.TestListId
        let cardId = cfg.TestCardId

        outTitle "Get all boards" ""
        trello.GetMyBoards
        |> Seq.iter (fun b -> outData b.Id b.Name)
        
        outTitle "Get a board" boardId 
        let b = trello.GetBoard boardId
        outData "board" b.Name

        outTitle "Get lists on a board" boardId
        trello.GetLists boardId
        |> Seq.iter (fun l -> outData l.Id l.Name)

        outTitle "Get a list" listId
        let l = trello.GetList listId
        outData l.Id l.Name
        
        outTitle "Get cards on a list" listId
        trello.GetCards listId
        |> Seq.iter (fun c -> outData c.Id c.Name)

        outTitle "Get a card" cardId
        let c = trello.GetCard cardId
        outData c.Id c.Name

        outTitle "Get card created date" cardId
        outData "Date created: " ((Pipes.CardPipe.CreatedDate c.Id).ToString("yyyy-M-d"))

        outTitle "Get attachements on a card" cardId
        let attachments = trello.GetCardAttachments cardId
        outData "attachments" (attachments.Length.ToString())
        attachments |> Seq.iteri (fun i a -> outData i a.Name)
        wait()
        0
