// fsharplint:disable TypeNames
namespace TrelloConnect

open FSharp.Data
open System
open Pipes
open Utils
open Pipes

[<AutoOpen>]
module Types =
    type private _Boards = JsonProvider<"../TrelloConnect/Samples/boards.sample.json", RootName="Boards">
    type private _Labels = JsonProvider<"../TrelloConnect/Samples/labels.sample.json", RootName="Labels">    
    type private _Lists = JsonProvider<"../TrelloConnect/Samples/lists.sample.json", RootName="Lists">
    type private _Cards = JsonProvider<"../TrelloConnect/Samples/cards.sample.json", RootName="Cards">
    type private _Attachments = JsonProvider<"../TrelloConnect/Samples/attachments.sample.json", RootName="Attachments">
    type private _CustomFields = JsonProvider<"../TrelloConnect/Samples/customfields.sample.json", RootName="CustomFields">
    type private _CustomField = JsonProvider<"../TrelloConnect/Samples/customfield.sample.json", RootName="CustomField">
    type private _CardCustomFields = JsonProvider<"../TrelloConnect/Samples/customfields.oncard.sample.json", RootName="CardCustomFields">
    type private _CardSearchResults = JsonProvider<"../TrelloConnect/Samples/cardsearchresults.sample.json", RootName="CardSearchResults">
    type private _SearchResults = JsonProvider<"../TrelloConnect/Samples/searchresults.sample.json", RootName="SearchResults">
    type private _IdName = JsonProvider<"../TrelloConnect/Samples/idname.sample.json", RootName="IdName">

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

    type AttachmentMimeType = 
        | Image
        | PDF
        | OctetStream
        | NotYetHandled
        | Empty

    type CardCoverColor = 
        | Pink
        | Yellow
        | Lime
        | Blue
        | Black
        | Orange
        | Red
        | Purple
        | Sky
        | Green
        static member StringValue cc = 
            match cc with 
                | Pink -> "pink"
                | Yellow -> "yellow"
                | Lime -> "lime"
                | Blue -> "blue"
                | Black -> "black"
                | Orange -> "orange"
                | Red -> "red"
                | Purple -> "purple"
                | Sky -> "sky"
                | Green -> "green"

    type CardCoverSize =
        | Normal
        | Full
        static member StringValue cc = 
            match cc with
            | Normal -> "normal"
            | Full -> "full"

    type CardCoverBrightness = 
        | Light
        | Dark
        static member StringValue cc =
            match cc with
            | Light -> "light"
            | Dark -> "dark"

    type CardCover =
        {
            Color: CardCoverColor
            Brightness: CardCoverBrightness
            Size: CardCoverSize
        }

    type CustomFieldOnCard = 
        {
            Id: string
            IdValue: string
            IdCustomField: string
            IdModel: string
            ModelType: string
            Value: string
        }

    type CustomField = 
        {
            Id: string
            IdModel: string
            ModelType: string
            Name: string
            Pos: int
            Type: string
        }

    type CustomFieldFull =
        {
            Id: string
            IdModel: string
            ModelType: string
            Name: string
            Pos: int
            Type: string
            IdValue: string
            IdCustomField: string
            Value: string
        }

    type Attachment =
        { Id: string
          Date: DateTime
          MimeType: string
          MyMimeType: AttachmentMimeType
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
          ListId: string
          BoardId: string
          CreatedDate: DateTime 
          Closed: bool }

    type List =
        { Id: string
          Name: string
          Closed: bool
          BoardId: string
          Position: decimal }

    type Board =
        { Id: string
          Name: string
          Desc: string
          Closed: bool
          Pinned: bool
          Url: string
          ShortUrl: string }

    type IdName = 
        { Id: string
          Name: string }

    type CardSearchResults = { Cards: Card [] }

    let private buildIdName (idn: _IdName.IdName) : IdName = 
        { Id = idn.Id
          Name = idn.Name }

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
          CreatedDate = Pipes.CardPipe.CreatedDate c.Id 
          ListId = c.IdList
          BoardId = c.IdBoard
          Closed = c.Closed }

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
          MyMimeType = 
            a.MimeType
            |> function
            | "image/jpeg" -> Image
            | "application/pdf" -> PDF
            | "application/octet-stream" -> OctetStream
            | "" -> Empty
            | _ -> NotYetHandled
          Name = a.Name
          Url = a.Url
          Pos = a.Pos
          FileName = a.FileName }

    let optionStringToDefault s =
        s
        |> function
        | Some x -> x
        | None -> ""

    let private buildCustomField (c: _CustomField.CustomField) = 
        {
            Id = c.Id
            ModelType = c.ModelType
            IdModel = c.IdModel
            Name = c.Name
            Pos = c.Pos
            Type = c.Type
        }

    let private buildCustomFieldOnCard (c: _CardCustomFields.CardCustomField) =
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
                | Some x -> 
                    [x.Number; x.Text] 
                    |> Seq.find Option.isSome
                    |> Option.defaultValue ""

                    //(x.Number, x.Text)
                    //|> function
                    //| Some n, _ -> n
                    //| _, Some n -> n
                    //| _ -> ""
                | None -> ""
            IdCustomField = c.IdCustomField
            IdModel = c.IdModel
            ModelType = c.ModelType
        }

    let ParseIdName x = _IdName.Parse x |> buildIdName

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

    let ParseCard (x: string) =
        _Cards.Parse $"[{x}]" |> Seq.item 0 |> buildCard

    let ParseAttachments x =
        _Attachments.Parse x |> Seq.map buildAttachment

    
    //let ParseCustomFields x = _CustomFields.Parse x

    let ParseCustomField x = _CustomField.Parse x |> buildCustomField

    let ParseCardCustomFields x = 
        _CardCustomFields.Parse x |> Seq.map buildCustomFieldOnCard

    let ParseCardSearchResults x =
        let csr = _CardSearchResults.Parse x
        csr.Cards
        |> Seq.map
            (fun c ->
                { Id = c.Id
                  Name = c.Name
                  Desc = c.Desc
                  CreatedDate = Pipes.CardPipe.CreatedDate c.Id
                  Closed = c.Closed
                  ListId = c.IdList
                  BoardId = c.IdBoard
                  Labels = c.Labels |> Seq.map buildLabelFromCardSearchResults |> Seq.toList })