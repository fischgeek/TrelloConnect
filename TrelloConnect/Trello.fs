// fsharplint:disable NonPublicValuesNames
namespace TrelloConnect

open TrelloConnect.Pipes
open System
open System.Configuration

[<AutoOpen>]
module Trello =
    let private MakeCall verb url fn msg =
        HRB.Url url
        |> verb
        |> function
        | Success x -> fn x
        | _ -> failwith msg

    let private Get fn msg url = MakeCall HttpGet url fn msg
    let private Put fn msg url = MakeCall HttpGet url fn msg
    let private Del fn msg url = MakeCall HttpGet url fn msg
    let private Pos fn msg url = MakeCall HttpGet url fn msg

    let param paramKey paramVal =
        paramVal |> SP.PrependIfNotEmpty $"{paramKey}="

    type TrelloWorker(key: string, tok: string) =
        member this.FormatURL (url: string) (x: string list) =
            if String.IsNullOrEmpty key then
                failwith "An API key is required."

            if String.IsNullOrEmpty tok then
                failwith "An API token is required."

            [ param "key" key; param "token" tok ] @ x
            |> SP.Join "&" // name=newname&desc=newdesc&pos=2
            |> SP.PrependIfNotEmpty "?" // ?name=newname&desc=newdesc&pos=2
            |> SP.Prepend $"https://api.trello.com/1{url}" // https://api.trello.com/1?name=newname&desc=newdesc&pos=2

        member this.GetBoards ?includeClosed =
            let filter = 
                if includeClosed.IsSome then
                    if includeClosed.Value then "all" else "open"
                else "open"
            this.FormatURL $"/members/me/boards" [param "filter" filter]
            |> Get Types.ParseBoards "Could not get boards."

        member this.GetBoard id =
            this.FormatURL 
                $"/boards/{id}" 
                [
                    param "fields" "name,desc,closed,pinned,shortLink,starred,url,shortUrl,dateLastActivity,dateLastView"
                ]
                |> Get Types.ParseBoard "Could not get board."
        
        //member this.GetBoard (id, props: BoardFields list) =
        //    this.FormatURL 
        //        $"/boards/{id}" 
        //        [
        //            param "fields" BoardFields.DefaultFields
        //        ]
        //        |> Get Types.ParseBoard "Could not get board."

        member this.GetLabelsOnBoard boardId =
            this.FormatURL $"/boards/{boardId}/labels" []
            |> Get ParseLabels "Could not get labels."

        member this.GetLists boardId =
            this.FormatURL $"/boards/{boardId}/lists" []
            |> Get Types.ParseLists "Could not get lists."

        member this.GetList id =
            this.FormatURL $"/lists/{id}" []
            |> Get Types.ParseList "Could not get list."

        member this.GetCards listId =
            this.FormatURL $"/lists/{listId}/cards" []
            |> Get Types.ParseCards "Could not get cards."

        member this.GetCard id =
            this.FormatURL $"/cards/{id}" []
            |> Get Types.ParseCard "Could not get card."

        member this.CreateCard(listId, name, ?desc) =
            this.FormatURL
                $"/cards"
                [ param "idList" listId
                  param "name" name
                  param "desc" (desc |> SP.NoneToBlank) ]
            |> Pos ignore "Failed to create card."

        member this.UpdateCard(cardId, ?newName, ?newDesc, ?pos) =
            this.FormatURL
                $"cards/{cardId}"
                [ param "name" (newName |> SP.NoneToBlank)
                  param "desc" (newDesc |> SP.NoneToBlank)
                  param "pos" (pos |> SP.NoneToBlank) ]
            |> Put ignore "Failed to update card."

        member this.GetCardAttachments cardId =
            this.FormatURL $"/cards/{cardId}/attachments" []
            |> Get Types.ParseAttachments "Could not get attachments."

        member this.ArchiveCard id =
            this.FormatURL $"/cards/{id}" [ param "closed" "true" ]
            |> Put ignore "Failed to archive card."

        member this.AttachUrlToCard url id =
            this.FormatURL $"/cards/{id}/attachments" [ param "url" url ]
            |> Pos ignore "Could not attach url to card."

        member this.AddLabelToCard id labelId =
            this.FormatURL $"/cards/{id}/idLabels" [ param "value" labelId ]
            |> Pos ignore "Could not add label to card."

        member this.MoveCard(id, newListId, ?pos) =
            this.FormatURL
                $"/cards/{id}"
                [ param "idList" newListId
                  param "pos" (pos |> SP.NoneToBlank) ]
            |> Put ignore "Failed to move card."

        //member this.GetCustomFieldsOnBoard id =
        //    this.FormatURL $"/boards/{id}/customFields" []
        //    |> Get Types.ParseCustomFields "Failed to get custom fields on board."

        //member this.GetCustomFieldsOnCard id =
        //    this.FormatURL $"/cards/{id}/customFieldItems" []
        //    |> Get Types.ParseCardCustomFields "Failed to get custom fields on card."

        member this.SetDueDate id dueDate =
            this.FormatURL $"/cards/{id}" [ param "due" dueDate ]
            |> Put ignore "Could not set due date on card."

        member this.SearchCards query =
            this.FormatURL
                $"/search"
                [ param "query" query
                  param "modelTypes" "cards" ]
            |> Get Types.ParseCardSearchResults "Failed to search."
