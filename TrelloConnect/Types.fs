// fsharplint:disable TypeNames
namespace TrelloConnect

open FSharp.Data
open System
open Pipes
open Utils
open Pipes
open System.IO

[<AutoOpen>]
module Types =
    type private _Boards = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/boards.sample.json", RootName="Boards">
    type private _Labels = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/labels.sample.json", RootName="Labels">    
    type private _Lists = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/lists.sample.json", RootName="Lists">
    type private _Cards = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/cards.sample.json", RootName="Cards">
    type private _Attachments = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/attachments.sample.json", RootName="Attachments">
    type private _CustomFields = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/customfields.sample.json", RootName="CustomFields">
    type private _CustomField = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/customfield.sample.json", RootName="CustomField">
    type private _CardCustomFields = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/customfields.oncard.sample.json", RootName="CardCustomFields">
    type private _CardSearchResults = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/cardsearchresults.sample.json", RootName="CardSearchResults">
    type private _SearchResults = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/searchresults.sample.json", RootName="SearchResults">
    type private _IdName = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/idname.sample.json", RootName="IdName">
    type WebHook = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/hook.sample.json", RootName="WebHook">

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

    type EmbeddedCustomFieldOnCard = 
        {
            Id: string
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
          Closed: bool
          HasLocation: bool 
          CustomFields: EmbeddedCustomFieldOnCard List }
        static member Empty = 
            {
                Id = ""
                Name = ""
                Desc = ""
                Labels = List.empty
                ListId = ""
                BoardId = ""
                CreatedDate = DateTime.Now
                Closed = false
                HasLocation = false
                CustomFields = List.Empty
            }

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

    type HookAction = 
        | AcceptEnterpriseJoinRequest
        | AddAttachmentToCard
        | AddChecklistToCard
        | AddLabelToCard
        | AddMemberToBoard
        | AddMemberToCard
        | AddMemberToOrganization
        | AddOrganizationToEnterprise
        | AddToEnterprisePluginWhitelist
        | AddToOrganizationBoard
        | CommentCard
        | ConvertToCardFromCheckItem
        | CopyBoard
        | CopyCard
        | CopyChecklist
        | CopyCommentCard
        | CreateBoard
        | CreateBoardInvitation
        | CreateBoardPreference
        | CreateCard
        | CreateCheckItem
        | CreateLabel
        | CreateList
        | CreateOrganization
        | CreateOrganizationInvitation
        | DeactivatedMemberInBoard
        | DeactivatedMemberInEnterprise
        | DeactivatedMemberInOrganization
        | DeleteAttachmentFromCard
        | DeleteBoardInvitation
        | DeleteCard
        | DeleteCheckItem
        | DeleteComment
        | DeleteLabel
        | DeleteOrganizationInvitation
        | DisableEnterprisePluginWhitelist
        | DisablePlugin
        | DisablePowerUp
        | EmailCard
        | EnableEnterprisePluginWhitelist
        | EnablePlugin
        | EnablePowerUp
        | MakeAdminOfBoard
        | MakeAdminOfOrganization
        | MakeNormalMemberOfBoard
        | MakeNormalMemberOfOrganization
        | MakeObserverOfBoard
        | MemberJoinedTrello
        | MoveCardFromBoard
        | MoveCardToBoard
        | MoveListFromBoard
        | MoveListToBoard
        | ReactivatedMemberInBoard
        | ReactivatedMemberInEnterprise
        | ReactivatedMemberInOrganization
        | RemoveChecklistFromCard
        | RemoveFromEnterprisePluginWhitelist
        | RemoveFromOrganizationBoard
        | RemoveLabelFromCard
        | RemoveMemberFromBoard
        | RemoveMemberFromCard
        | RemoveMemberFromOrganization
        | RemoveOrganizationFromEnterprise
        | UnconfirmedBoardInvitation
        | UnconfirmedOrganizationInvitation
        | UpdateBoard
        | UpdateCard
        | UpdateCheckItem
        | UpdateCheckItemStateOnCard
        | UpdateChecklist
        | UpdateComment
        | UpdateCustomFieldItem
        | UpdateLabel
        | UpdateList
        | UpdateMember
        | UpdateOrganization
        | VoteOnCard
        | Unknown
        static member FromString = UnionPipe.FromStringCI<HookAction>

    //let saveMsg prefix msg extra = 
    //    let fi = FilePipe.WriteTextRandomNamePrefix prefix @"c:\dev\temp\trello-calls\" "json" extra
    //    $"{msg} Output saved to {fi.FullName}"
    type ModelString = string
    type ActionString = string

    type WebhookActionResult = 
        | ActionNotHandled
        | CommandNotHandled of string
        | ModelNotHandled
        | ActionOrModelNotRecognized
        | SaveJsonToFile of string
        | Success of string
        | Fail of string
        static member HandleResult (prtMsg: string -> unit) (saveMsg: ModelString -> string -> string) (model: string) (ha: HookAction option) (war: WebhookActionResult) = 
            let h = 
                match ha with
                | Some x -> x.ToString()
                | None -> "No action Received"
            war
            |> function
            | ActionNotHandled -> $"{h}. Action not handled on Model {model}." |> prtMsg
            | CommandNotHandled c -> $"{h}. Command '{c}' not handled." |> prtMsg
            | ModelNotHandled -> $"Model '{model}' not handled." |> prtMsg
            | ActionOrModelNotRecognized -> $"Model '{model}' not recognized with action '{h}'" |> prtMsg
            | SaveJsonToFile js -> 
                let outputFile = saveMsg model js
                let z = outputFile
                $"Model '{model}' not recognized with action '{h}'. Saved to {z}" |> prtMsg
            | Success msg -> $"Success! {h}. {msg}." |> prtMsg
            | Fail msg -> $"Fail! {h}. {msg}." |> prtMsg
    
    type WAR = WebhookActionResult

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

    let private buildEmbeddedCustomFieldOnCard (c: _Cards.CustomFieldItem) =
        {
            Id = c.Id
            Value = 
                c.Value
                |> (fun x -> 
                    [x.Number; x.Text] 
                    |> Seq.tryFind Option.isSome
                    |> function
                    | Some x -> x.Value
                    | None -> ""
                    //|> Seq.find Option.isSome
                    //|> Option.defaultValue ""
                )
            IdCustomField = c.IdCustomField
            IdModel = c.IdModel
            ModelType = c.ModelType
        }
    
    let private buildCard (c: _Cards.Card) =
        { Id = c.Id
          Name = c.Name
          Desc = c.Desc
          Labels = c.Labels |> Seq.map buildLabel |> Seq.toList
          CreatedDate = Pipes.CardPipe.CreatedDate c.Id 
          ListId = c.IdList
          BoardId = c.IdBoard
          Closed = c.Closed
          HasLocation = c.Badges.Location
          CustomFields = c.CustomFieldItems |> Seq.map buildEmbeddedCustomFieldOnCard |> Seq.toList
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

    let ParseCardsX x = 
        let cards = _Cards.Parse x
        cards
        |> Array.map (fun c ->
            buildCard c
        )

    let ParseCard (x: string) =
        _Cards.Parse $"[{x}]" |> Seq.item 0 |> buildCard

    let ParseAttachments x =
        _Attachments.Parse x |> Seq.map buildAttachment
    
    let ParseCustomFields x = _CustomFields.Parse x

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
                  Labels = c.Labels |> Seq.map buildLabelFromCardSearchResults |> Seq.toList 
                  HasLocation = c.Badges.Location 
                  CustomFields = []
                  //CustomFields = c.CustomFieldItems |> Seq.map buildCustomFieldOnCard |> Seq.toList
                })