// fsharplint:disable TypeNames
namespace TrelloConnect

open FSharp.Data
open System
open Pipes
open Utils

[<AutoOpen>]
module Types =
    type private _Boards = JsonProvider<"Samples/boards.sample.json", RootName="Boards">
    type private _Labels = JsonProvider<"Samples/labels.sample.json", RootName="Labels">
    type private _Lists = JsonProvider<"Samples/lists.sample.json", RootName="Lists">
    type private _Cards = JsonProvider<"Samples/cards.sample.json", RootName="Cards">
    type private _Attachments = JsonProvider<"Samples/attachments.sample.json", RootName="Attachments">
    type private _CustomFields = JsonProvider<"Samples/customfields.sample.json", RootName="CustomFields">

    type private _CardCustomFields =
        JsonProvider<"Samples/customfields.oncard.sample.json", RootName="CardCustomFields">

    type private _CardSearchResults =
        JsonProvider<"Samples/cardsearchresults.sample.json", RootName="CardSearchResults">

    type private BoardFields =
        | Name
        | Desc
        | Pinned
        static member Stringify(props: BoardFields list) : string =
            props
            |> Seq.map (fun x -> x.ToString())
            |> String.concat ","

        static member DefaultFields =
            [ BoardFields.Name; BoardFields.Desc ]
            |> BoardFields.Stringify

    type CustomField = 
        {
            Id: string
            IdValue: string
            IdCustomField: string
            IdModel: string
            ModelType: string
            Value: string
        }

    type Attachment =
        { Id: string
          Date: DateTime
          MimeType: string
          Name: string
          Url: string
          Pos: int
          FileName: string }

    type Label =
        { Id: string
          Name: string
          BoardId: string
          Color: string }

    type Card =
        { Id: string
          Name: string
          Desc: string
          Labels: Label List
          CreatedDate: DateTime }

    type List =
        { Id: string
          Name: string
          Closed: bool
          BoardId: string
          Position: int }

    type Board =
        { Id: string
          Name: string
          Desc: string
          Closed: bool
          Pinned: bool
          Url: string
          ShortUrl: string }

    type CardSearchResults = { Cards: Card [] }

    let private buildBoard (b: _Boards.Board) =
        { Id = b.Id
          Name = b.Name
          Desc = b.Desc
          Closed = b.Closed
          Pinned = b.Pinned |> boolOptionToDefault false
          Url = b.Url
          ShortUrl = b.ShortUrl }

    let private buildList (l: _Lists.List) =
        { Id = l.Id
          Name = l.Name
          Closed = l.Closed
          BoardId = l.IdBoard
          Position = l.Pos }

    let private buildLabel (l: _Cards.Label) =
        { Id = l.Id
          BoardId = l.IdBoard
          Name = l.Name
          Color = l.Color }

    let private buildCard (c: _Cards.Card) =
        { Id = c.Id
          Name = c.Name
          Desc = c.Desc
          Labels = c.Labels |> Seq.map buildLabel |> Seq.toList
          CreatedDate = Pipes.CardPipe.CreatedDate c.Id }

    let private buildLabelFromLabels (l: _Labels.Label) =
        { Id = l.Id
          BoardId = l.IdBoard
          Name = l.Name
          Color = l.Color }
    let private buildLabelFromCardSearchResults (l: _CardSearchResults.Label) = 
        { Id = l.Id
          BoardId = l.IdBoard
          Name = l.Name
          Color = l.Color }
    let private buildAttachment (a: _Attachments.Attachment) =
        { Id = a.Id
          Date = a.Date.Date
          MimeType = a.MimeType
          Name = a.Name
          Url = a.Url
          Pos = a.Pos
          FileName = a.FileName }

    let optionStringToDefault s =
        s
        |> function
        | Some x -> x
        | None -> ""

    let private buildCustomField (c: _CardCustomFields.CardCustomField) =
        {
            Id = c.Id
            IdValue = 
                c.IdValue
                |> function
                | Some x -> x
                | None -> ""
            Value = 
                c.Value
                |> function
                | Some x -> x.Number.Value.ToString()
                | None -> ""
            IdCustomField = c.IdCustomField
            IdModel = c.IdModel
            ModelType = c.ModelType
        }

    let ParseBoards x = _Boards.Parse x |> Seq.map buildBoard

    let ParseBoard x =
        _Boards.Parse $"[{x}]" |> Seq.item 0 |> buildBoard

    let ParseLabels x = _Labels.Parse x |> Seq.map buildLabelFromLabels

    let ParseLabel x =
        _Labels.Parse $"[{x}]" |> Seq.item 0 |> buildLabelFromLabels

    let ParseLists x = _Lists.Parse x |> Seq.map buildList

    let ParseList x =
        _Lists.Parse $"[{x}]" |> Seq.item 0 |> buildList

    let ParseCards x = _Cards.Parse x |> Seq.map buildCard

    let ParseCard x =
        _Cards.Parse $"[{x}]" |> Seq.item 0 |> buildCard

    let ParseAttachments x =
        _Attachments.Parse x |> Seq.map buildAttachment

    //let ParseCustomFields x = _CustomFields.Parse x
    let ParseCardCustomFields x = 
        _CardCustomFields.Parse x |> Seq.map buildCustomField

    let ParseCardSearchResults x =
        let csr = _CardSearchResults.Parse x
        csr.Cards
        |> Seq.map
            (fun c ->
                { Id = c.Id
                  Name = c.Name
                  Desc = c.Desc
                  CreatedDate = Pipes.CardPipe.CreatedDate c.Id
                  Labels = c.Labels |> Seq.map buildLabelFromCardSearchResults |> Seq.toList })
