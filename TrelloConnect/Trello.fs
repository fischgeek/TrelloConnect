// fsharplint:disable NonPublicValuesNames
namespace TrelloConnect

open TrelloConnect.Types
open TrelloConnect.Pipes
open System
open System.Configuration
open System.IO
open System.Drawing
open Newtonsoft.Json
open System.Web

[<AutoOpen>]
module Trello =
    let private MakeCallWithBody verb url (body:string) fn msg = 
        HRB.Url url
        |> HRB.Body body
        |> verb
        |> function
        | Success x -> fn x
        | _ -> failwith msg
    let private MakeCall verb url fn msg = MakeCallWithBody verb url "" fn msg
    let private MakeCallWithOAuth1 key tok verb url fn msg = MakeCall (HRB.OAuth1 key tok >> verb) url fn msg
    let private MakeCallBodyWithOAuth1 key tok body verb url fn msg = MakeCall (HRB.OAuth1 key tok >> HRB.Body body >> verb) url fn msg

    let private Get fn msg url = MakeCall HttpGet url fn msg
    let private GetWithOAuth1 key tok fn msg url = MakeCallWithOAuth1 key tok HttpGet url fn msg
    
    let private Put fn msg url = MakeCall HttpPut url fn msg
    let private PutJson fn msg body url = MakeCallWithBody HttpPutJson url body fn msg
    let private PutWithOAuth1 key tok fn msg url = MakeCallWithOAuth1 key tok HttpPut url fn msg
    let private PutJsonWithOAuth1 key tok fn msg body url = MakeCallBodyWithOAuth1 key tok body HttpPut url fn msg
    let private PutJsonObjWithOAuth1 key tok fn msg body url = MakeCallBodyWithOAuth1 key tok body HttpPutJson url fn msg
    
    let private Del fn msg url = MakeCall HttpDel url fn msg
    
    let private Pos fn msg url = MakeCall HttpPost url fn msg
    let private PosWithOAuth1 key tok fn msg url = MakeCallWithOAuth1 key tok HttpPost url fn msg
    let private DelWithOAuth1 key tok fn msg url = MakeCallWithOAuth1 key tok HttpDel url fn msg
    let private PosJsonWithOAuth1 key tok fn msg body url = MakeCallBodyWithOAuth1 key tok body HttpPost url fn msg

    let param paramKey paramVal =
        paramVal |> SP.PrependIfNotEmpty $"{paramKey}="

    let assumeString fn xx =
        match xx with
        | String x -> fn x
        | Bytes x -> failwith "Did not get a string return type."
    type TrelloWorker(key: string, tok: string) =
        member this.OAuth1GetReturnString fn msg url = GetWithOAuth1 key tok (assumeString fn) msg url
        member this.OAuth1GetReturnAny fn msg url = GetWithOAuth1 key tok fn msg url

        member this.OAuth1PutReturnString fn msg url = PutWithOAuth1 key tok (assumeString fn) msg url
        member this.OAuth1PutJsonReturnString fn msg body url = PutJsonWithOAuth1 key tok (assumeString fn) msg body url
        member this.OAuth1PutJsonObjReturnString fn msg body url = PutJsonObjWithOAuth1 key tok (assumeString fn) msg body url

        member this.OAuth1PostReturnString fn msg url = PosWithOAuth1 key tok (assumeString fn) msg url
        member this.OAuth1DelReturnString fn msg url = DelWithOAuth1 key tok (assumeString fn) msg url
        member this.OAuth1PostJsonReturnString fn msg body url = PosJsonWithOAuth1 key tok (assumeString fn) msg body url

        member this.FormatURL (url: string) (x: string list) =
            if String.IsNullOrEmpty key then
                failwith "An API key is required."

            if String.IsNullOrEmpty tok then
                failwith "An API token is required."

            x
            |> SP.Join "&" // name=newname&desc=newdesc&pos=2
            |> SP.PrependIfNotEmpty "?" // ?name=newname&desc=newdesc&pos=2
            |> SP.Prepend $"https://api.trello.com/1{url}" // https://api.trello.com/1?name=newname&desc=newdesc&pos=2

        member this.GetBoards ?includeClosed =
            let filter = 
                if includeClosed.IsSome then
                    if includeClosed.Value then "all" else "open"
                else "open"
            this.FormatURL $"/members/me/boards" [param "filter" filter]
            |> this.OAuth1GetReturnString Types.ParseBoards "Could not get boards."

        member this.GetBoard id =
            this.FormatURL 
                $"/boards/{id}" 
                [
                    param "fields" "name,desc,closed,pinned,shortLink,starred,url,shortUrl,dateLastActivity,dateLastView"
                ]
                |> this.OAuth1GetReturnString Types.ParseBoard "Could not get board."
    
        //member this.GetBoard (id, props: BoardFields list) =
        //    this.FormatURL 
        //        $"/boards/{id}" 
        //        [
        //            param "fields" BoardFields.DefaultFields
        //        ]
        //        |> this.GWO Types.ParseBoard "Could not get board."

        member this.GetLabelsOnBoard boardId =
            this.FormatURL $"/boards/{boardId}/labels" []
            |> this.OAuth1GetReturnString ParseLabels "Could not get labels."

        member this.CreateLabelOnBoard boardId (lbl: Label) = 
            this.FormatURL $"/boards/{boardId}/labels" 
                [
                    param "name" lbl.Name
                    param "color" lbl.Color
                ] |> this.OAuth1PostReturnString Types.ParseLabel "Failed to create label."

        member this.GetLists boardId =
            this.FormatURL $"/boards/{boardId}/lists" []
            |> this.OAuth1GetReturnString Types.ParseLists "Could not get lists."

        member this.GetList id =
            this.FormatURL $"/lists/{id}" []
            |> this.OAuth1GetReturnString Types.ParseList "Could not get list."

        member this.GetCards listId =
            this.FormatURL $"/lists/{listId}/cards" []
            |> this.OAuth1GetReturnString Types.ParseCards "Could not get cards."

        member this.GetCardsOnBoard boardId = 
            this.FormatURL $"/boards/{boardId}/cards" []
            |> this.OAuth1GetReturnString Types.ParseCards "Could not get cards."

        member this.GetCard id =
            this.FormatURL $"/cards/{id}" []
            |> this.OAuth1GetReturnString Types.ParseCard "Could not get card."

        member this.CreateCardIgnore(listId, name, ?desc, ?addr) =
            this.FormatURL
                $"/cards"
                [ param "idList" listId
                  param "name" name
                  param "desc" (desc |> SP.NoneToBlank)
                  param "address" (addr |> SP.NoneToBlank) ]
            |> this.OAuth1PostReturnString ignore "Failed to create card."

        member this.CreateCard(listId, name, ?desc, ?locName, ?addr) =
            this.FormatURL
                $"/cards"
                [ param "idList" listId
                  param "name" name
                  param "desc" (desc |> SP.NoneToBlank)
                  param "locationName" (locName |> SP.NoneToBlank)
                  param "address" (addr |> SP.NoneToBlank) |> HttpUtility.UrlEncode ] 
            |> this.OAuth1PostReturnString Types.ParseCard "Failed to create card."

        //member this.CreateCard2(listId, name, ?desc) =
        //    this.FormatURL
        //        $"/cards"
        //        [ param "idList" listId
        //          param "name" name
        //          param "desc" (desc |> SP.NoneToBlank) ]
        //    |> this.OAuth1PostReturnString ignore "Failed to create card."

        member this.UpdateCard(cardId, ?newName, ?newDesc, ?pos) =
            this.FormatURL
                $"/cards/{cardId}"
                [ param "name" (newName |> SP.NoneToBlank)
                  param "desc" (newDesc |> SP.NoneToBlank)
                  param "pos" (pos |> SP.NoneToBlank) ]
            |> this.OAuth1PutReturnString Types.ParseCard "Failed to update card."

        member this.AddCoordinatesToCard (cardId: string) (coords: decimal * decimal) = 
            let lat,lng = coords
            let coordString = $"{lat.ToString()},{lng.ToString()}"
            this.FormatURL $"/cards/{cardId}" [ param "coordinates" coordString ]
            |> this.OAuth1PutReturnString Types.ParseCard "Failed to update card's coords."

        member this.DeleteCard cardId = 
            this.FormatURL $"/cards/{cardId}" []
            |> this.OAuth1DelReturnString ignore "Failed to delete card."

        member this.GetCardAttachments cardId =
            this.FormatURL $"/cards/{cardId}/attachments" []
            |> this.OAuth1GetReturnString Types.ParseAttachments "Could not get attachments."

        member this.DownloadAttachment saveToPath cardId (attachment: Attachment) = 
            this.FormatURL $"/cards/{cardId}/attachments/{attachment.Id}/download/{attachment.FileName}" []
            |> this.OAuth1GetReturnAny
                (function
                | Bytes b -> File.WriteAllBytes(saveToPath, b |> Seq.toArray)
                | _ -> ())
                "Failed to download attachment."

        member this.ArchiveCard id =
            this.FormatURL $"/cards/{id}" [ param "closed" "true" ]
            |> this.OAuth1PutReturnString ignore "Failed to archive card."

        member this.AttachUrlToCard url id =
            this.FormatURL $"/cards/{id}/attachments" [ param "url" url ]
            |> this.OAuth1PostReturnString ignore "Could not attach url to card."

        member this.AddLabelToCard (id: string) labelId =
            this.FormatURL $"/cards/{id}/idLabels" [ param "value" labelId ]
            |> this.OAuth1PostReturnString ignore "Could not add label to card."

        member this.AddLabelToCardByName (id: string) (lblName: string) = 
            let card = this.GetCard id
            this.GetLabelsOnBoard card.BoardId
            |> Seq.tryFind (fun l -> l.Name.ToLower() = lblName)
            |> function
            | Some lbl -> this.AddLabelToCard card.Id lbl.Id
            | None -> ()

        member this.AddLablesToCardByName (id: string) (lbls: string list) = 
            lbls |> Seq.iter (fun lbl -> this.AddLabelToCardByName id lbl)

        member this.AddLablesToCardByNameArray (id: string) (lbls: string array) = 
            lbls |> Seq.iter (fun lbl -> this.AddLabelToCardByName id lbl)

        member this.MoveCard(id, newListId, ?pos) =
            this.FormatURL
                $"/cards/{id}"
                [ param "idList" newListId
                  param "pos" (pos |> SP.NoneToBlank) ]
            |> this.OAuth1PutReturnString ignore "Failed to move card."

        member this.QuickMoveCard id newListId =
            this.FormatURL
                $"/cards/{id}"
                [ param "idList" newListId ]
            |> this.OAuth1PutReturnString ignore "Failed to move card."

        member this.MoveCardToTop id newListId = 
            this.FormatURL
                $"/cards/{id}"
                [ param "idList" newListId
                  param "pos" "top" ]
            |> this.OAuth1PutReturnString ignore "Failed to move card."

        member this.MoveCardToBottom id newListId = 
            this.FormatURL
                $"/cards/{id}"
                [ param "idList" newListId
                  param "pos" "bottom" ]
            |> this.OAuth1PutReturnString ignore "Failed to move card."

        member this.GetCustomField id =
            this.FormatURL $"/customFields/{id}" []
            |> this.OAuth1GetReturnString Types.ParseCustomField "Failed to get custom field."

        member this.GetCustomFieldsOnCard id =
            this.FormatURL $"/cards/{id}/customFieldItems" []
            |> this.OAuth1GetReturnString Types.ParseCardCustomFields "Failed to get custom fields on card."

        member this.GetCustomFieldsOnCardFull id = 
            this.GetCustomFieldsOnCard id
            |> Seq.map (fun f -> 
                let cf = this.GetCustomField f.IdCustomField
                {
                    Id = f.Id
                    IdModel = f.IdModel
                    ModelType = f.ModelType
                    Name = cf.Name
                    Pos = cf.Pos
                    Type = cf.Type
                    IdValue = f.IdValue
                    IdCustomField = f.IdCustomField
                    Value = f.Value
                }
            )

        member this.SetDueDate id dueDate =
            this.FormatURL $"/cards/{id}" [ param "due" dueDate ]
            |> this.OAuth1PutReturnString ignore "Could not set due date on card."

        member this.SearchCards query =
            this.FormatURL
                $"/search"
                [ param "query" query
                  param "modelTypes" "cards" ]
            |> this.OAuth1GetReturnString Types.ParseCardSearchResults "Failed to search cards."

        member this.SetCustomFieldValue cardId customFieldId (newValue: string) = 
            this.FormatURL
                $"/cards/{cardId}/customField/{customFieldId}/item" []
            |> this.OAuth1PutJsonReturnString ignore "Failed to update field." newValue

        member this.RemoveCardCover cardId = 
            let o = {| cover = "null" |} |> JsonConvert.SerializeObject
            this.FormatURL $"/cards/{cardId}" []
            |> this.OAuth1PutJsonObjReturnString ignore "Failed to remove card cover." o

        member this.SetCardCoverColor (cardId: string) (cover: CardCover) = 
            let c = CardCoverColor.StringValue cover.Color
            let b = CardCoverBrightness.StringValue cover.Brightness
            let s = CardCoverSize.StringValue cover.Size
            let o = {| cover = {| color = c; brightness = b; size = s |} |} |> JsonConvert.SerializeObject
            //let fmt = String.Format("""{"cover":{"color":"{0}","brightness":{1},"size":{2}} }""", c, b, s)
            //let fmt = cover.ToString()
            //let json = """ {"cover":{"color":"{0}","brightness":{1},"size":{s}} } """
            this.FormatURL $"/cards/{cardId}" []
            |> this.OAuth1PutJsonObjReturnString ignore "Failed to update cover." o

        member this.SetCardCoverPink cardid = this.SetCardCoverColor cardid { Color = CardCoverColor.Pink; Brightness = CardCoverBrightness.Dark; Size = CardCoverSize.Full }
        member this.SetCardCoverYellow cardid = this.SetCardCoverColor cardid { Color = CardCoverColor.Yellow; Brightness = CardCoverBrightness.Dark; Size = CardCoverSize.Full }
        member this.SetCardCoverLime cardid = this.SetCardCoverColor cardid { Color = CardCoverColor.Lime; Brightness = CardCoverBrightness.Dark; Size = CardCoverSize.Full }
        member this.SetCardCoverBlue cardid = this.SetCardCoverColor cardid { Color = CardCoverColor.Blue; Brightness = CardCoverBrightness.Dark; Size = CardCoverSize.Full }
        member this.SetCardCoverBlack cardid = this.SetCardCoverColor cardid { Color = CardCoverColor.Black; Brightness = CardCoverBrightness.Dark; Size = CardCoverSize.Full }
        member this.SetCardCoverOrange cardid = this.SetCardCoverColor cardid { Color = CardCoverColor.Orange; Brightness = CardCoverBrightness.Dark; Size = CardCoverSize.Full }
        member this.SetCardCoverRed cardid = this.SetCardCoverColor cardid { Color = CardCoverColor.Red; Brightness = CardCoverBrightness.Dark; Size = CardCoverSize.Full }
        member this.SetCardCoverPurple cardid = this.SetCardCoverColor cardid { Color = CardCoverColor.Purple; Brightness = CardCoverBrightness.Dark; Size = CardCoverSize.Full }
        member this.SetCardCoverSky cardid = this.SetCardCoverColor cardid { Color = CardCoverColor.Sky; Brightness = CardCoverBrightness.Dark; Size = CardCoverSize.Full }
        member this.SetCardCoverGreen cardid = this.SetCardCoverColor cardid { Color = CardCoverColor.Green; Brightness = CardCoverBrightness.Dark; Size = CardCoverSize.Full }