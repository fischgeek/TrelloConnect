// fsharplint:disable TypeNames
namespace TrelloConnect

open FSharp.Data
open System
open Pipes
open Utils

[<AutoOpen>]
module Types = 
    type private _Boards = JsonProvider<"Samples/boards.sample.json",RootName="Boards">
    //type private _Board = JsonProvider<"Samples/board.sample.json",RootName="Board">
    type private _Labels = JsonProvider<"Samples/labels.sample.json",RootName="Labels">
    //type private _Label = JsonProvider<"Samples/label.sample.json", RootName="Label">
    type private _Lists = JsonProvider<"Samples/lists.sample.json", RootName="Lists">
    //type private _List = JsonProvider<"Samples/list.sample.json", RootName="List">
    type private _Cards = JsonProvider<"Samples/cards.sample.json", RootName="Cards">
    type private _Attachments = JsonProvider<"Samples/attachments.sample.json", RootName="Attachments">
    type private _CustomFields = JsonProvider<"Samples/customfields.sample.json", RootName="CustomFields">
    type private _CardCustomFields = JsonProvider<"Samples/customfields.oncard.sample.json", RootName="CardCustomFields">
    type private _CardSearchResults = JsonProvider<"Samples/cardsearchresults.sample.json", RootName="CardSearchResults">
    
    type private BoardProperty = 
        | Name
        | Desc
        | Pinned
        static member Stringify (props: BoardProperty list) : string = 
            props 
            |> Seq.map (fun x -> x.ToString()) 
            |> String.concat ","
        static member DefaultFields = 
            [BoardProperty.Name; BoardProperty.Desc;] |> BoardProperty.Stringify

    type Attachment = 
        {
            Id: string
            Date: DateTime
            MimeType: string
            Name: string
            Url: string
            Pos: int
            FileName: string
        }
    type Label = 
        {
            Id: string
            Name: string
            BoardId: string
            Color: string
        }
    type Card = 
        {
            Id: string
            Name: string
            Desc: string
            CreatedDate: DateTime
        }
    type List = 
        {
            Id: string
            Name: string
            Closed: bool
            BoardId: string
            Position: int
        }
    type Board = 
        {
            Id: string
            Name: string
            Desc: string
            Closed: bool
            Pinned: bool
            Url: string
            ShortUrl: string
            //LabelNames: string []
        }
    type CardSearchResults = 
        {
            Cards: Card []
        }

    
    let private buildBoard (b: _Boards.Board) = 
        { 
            Id = b.Id
            Name = b.Name
            Desc = b.Desc
            Closed = b.Closed
            Pinned = b.Pinned |> boolOptionToDefault false
            Url = b.Url
            ShortUrl = b.ShortUrl 
        }
    let private buildList (l: _Lists.List) =
        {
            Id = l.Id
            Name = l.Name
            Closed = l.Closed
            BoardId = l.IdBoard
            Position = l.Pos 
        }    
    let private buildCard (c: _Cards.Card) = 
        { 
            Id = c.Id
            Name = c.Name
            Desc = c.Desc
            CreatedDate = Pipes.CardPipe.CreatedDate c.Id 
        }
    let private buildLabel (l: _Labels.Label) = 
        {
            Id = l.Id
            BoardId = l.IdBoard
            Name = l.Name
            Color = l.Color
        }
    let private buildAttachment (a: _Attachments.Attachment) = 
        {
            Id = a.Id
            Date = a.Date.Date
            MimeType = a.MimeType
            Name = a.Name
            Url = a.Url
            Pos = a.Pos
            FileName = a.FileName
        }

    let ParseBoards x = _Boards.Parse x |> Seq.map buildBoard
    let ParseBoard x = _Boards.Parse $"[{x}]" |> Seq.item 0 |> buildBoard
    
    let ParseLabels x = _Labels.Parse x |> Seq.map buildLabel
    let ParseLabel x = _Labels.Parse $"[{x}]" |> Seq.item 0 |> buildLabel
    
    let ParseLists x = _Lists.Parse x |> Seq.map buildList
    let ParseList x = _Lists.Parse $"[{x}]" |> Seq.item 0 |> buildList
    
    let ParseCards x = _Cards.Parse x |> Seq.map buildCard
    let ParseCard x = _Cards.Parse $"[{x}]" |> Seq.item 0 |> buildCard

    let ParseAttachments x = _Attachments.Parse x |> Seq.map buildAttachment
    let ParseCustomFields x = _CustomFields.Parse x
    let ParseCardCustomFields x = _CardCustomFields.Parse x
    let ParseCardSearchResults x = 
        let csr = _CardSearchResults.Parse x
        csr.Cards 
        |> Seq.map (fun c ->
            {
                Id = c.Id
                Name = c.Name
                Desc = c.Desc
                CreatedDate = Pipes.CardPipe.CreatedDate c.Id
            }
        )