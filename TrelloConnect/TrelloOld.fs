// fsharplint:disable NonPublicValuesNames
namespace TrelloConnect

open System
open System.Configuration

module Trello_Old =
    ()

    //let private MakeCall verb url fn msg =
    //    HRB.Url url
    //    //|> HRB.Headers [("","")]
    //    |> verb
    //    |> function
    //    | Success x -> fn x
    //    | _ -> failwith msg

    //let private Get url fn msg = MakeCall HttpGet url fn msg
    //let private Put url fn msg = MakeCall HttpPut url fn msg
    //let private Del url fn msg = MakeCall HttpDel url fn msg
    //let private Pos url fn msg = MakeCall HttpPost url fn msg

    //let TR = new TrelloRoutes("","")

    ////let GetBoards() = Get Routes.MyBoards Types.ParseBoards "Could not get boards."
    ////let GetBoardById id = Get (Routes.Board id) Types.ParseBoard "Could not get board."
    ////let GetLabelsOnBoard id = Get (Routes.Labels id) Types.ParseLabels "Could not get labels."
    ////let GetLists boardId = Get (Routes.Lists boardId) Types.ParseLists "Could not get lists."
    ////let GetListById listId = Get (Routes.List listId) Types.ParseList "Could not get list."
    ////let GetCards listId = Get (Routes.Cards listId) Types.ParseCards "Could not get cards."
    ////let GetCardById cardId = Get (Routes.Card cardId) Types.ParseCard "Could not get card."
    //let GetCardAttachments cardId = Get (Routes.CardAttachments cardId) Types.ParseAttachments "Could not get attachments."
    //let DeleteCard cardId = Del (Routes.Card cardId) ignore "Failed to delete card."
    //let ArchiveCard cardId = Put (Routes.ArchiveCard cardId) (ignore) "Failed to archive card."
    //let CreateCard name desc listId = Pos (Routes.CreateCard name desc listId) (ignore) "Failed to create card."
    //let UpdateCard newName newDesc cardId = Put (Routes.UpdateCard newName newDesc cardId) Types.ParseCard "Could not update card."
    //let AddUrlAttachmentToCard url cardId = Pos (Routes.AttachUrlToCard url cardId) (ignore) "Could not attach url to card."
    //let AddLabelToCard cardId labelId = Pos (Routes.AddLabelToCard cardId labelId) (ignore) "Could not add label to card."
    //let MoveCard cardId newListId = Put (Routes.MoveCard cardId newListId) (ignore) "Failed to move card."
    //let MoveCardBottom cardId newListId = Put (Routes.MoveCardBottom cardId newListId) (ignore) "Failed to move card."
    //let GetCustomFieldsOnBoard id = Get (Routes.GetCustomFieldsOnBoard id) Types.ParseCustomFields "Failed to get custom fields on board."
    //let GetCustomFieldsOnCard id = Get (Routes.GetCustomFieldsOnCard id) Types.ParseCardCustomFields "Failed to get custom fields on card."
    ////let SetDueDate id dueDate = Put (Routes.SetDueDate id dueDate) (ignore) "Could not set due date on card."
    //let SearchCards query = 
    //    let getUrl id = $"{BaseURL}/boards/{id}/customFields?{Auth}"
    //    Get (Routes.SearchCards query) Types.ParseCardSearchResults "Failed to search."