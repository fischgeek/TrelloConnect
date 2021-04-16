namespace TrelloConnect

open FSharp.Data

[<AutoOpen>]
module Types = 
    type Boards = JsonProvider<"[{}]", EmbeddedResource="TrelloConnect.Samples.boards.sample.json", RootName="Boards">
    type Board = JsonProvider<"{}", EmbeddedResource="TrelloConnect.Samples.board.sample.json", RootName="Board">
    type Labels = JsonProvider<"[{}]", EmbeddedResource="TrelloConnect.Samples.labels.sample.json", RootName="Labels">
    type Label = JsonProvider<"{}", EmbeddedResource="TrelloConnect.Samples.label.sample.json", RootName="Label">
    type Lists = JsonProvider<"[{}]", EmbeddedResource="TrelloConnect.Samples.lists.sample.json", RootName="Lists">
    type List = JsonProvider<"{}", EmbeddedResource="TrelloConnect.Samples.list.sample.json", RootName="List">
    type Cards = JsonProvider<"[{}]", EmbeddedResource="TrelloConnect.Samples.cards.sample.json", RootName="Cards">
    type Card = JsonProvider<"{}", EmbeddedResource="TrelloConnect.Samples.card.sample.json", RootName="Card">
    type Attachments = JsonProvider<"[{}]", EmbeddedResource="TrelloConnect.Samples.attachments.sample.json", RootName="Attachments">
    //type Attachment = JsonProvider<"{}", EmbeddedResource="TrelloConnect.Samples.attachment.sample.json", RootName="Attachment">
    type CustomFields = JsonProvider<"[{}]", EmbeddedResource="TrelloConnect.Samples.customfields.sample.json", RootName="CustomFields">
    type CardCustomFields = JsonProvider<"[{}]", EmbeddedResource="TrelloConnect.Samples.customfields.oncard.sample.json", RootName="CardCustomFields">
    type CardSearchResults = JsonProvider<"[{}]", EmbeddedResource="TrelloConnect.Samples.cardsearchresults.sample.json", RootName="CardSearchResults">
    let ParseBoards x = Boards.Parse x
    let ParseBoard x = Board.Parse x
    let ParseLabels x = Labels.Parse x
    let ParseLabel x = Label.Parse x
    let ParseLists x = Lists.Parse x
    let ParseList x = List.Parse x
    let ParseCards x = Cards.Parse x
    let ParseCard x = Card.Parse x
    let ParseAttachments x = Attachments.Parse x
    //let ParseAttachment x = Attachment.Parse x
    let ParseCustomFields x = CustomFields.Parse x
    let ParseCardCustomFields x = CardCustomFields.Parse x
    let ParseCardSearchResults x = CardSearchResults.Parse x