namespace TrelloConnect

open FSharp.Data

[<AutoOpen>]
module Types = 
    type Boards = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/boards.sample.json", RootName="Boards">
    type Board = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/board.sample.json", RootName="Board">
    type Labels = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/labels.sample.json", RootName="Labels">
    type Label = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/label.sample.json", RootName="Label">
    type Lists = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/lists.sample.json", RootName="Lists">
    type List = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/list.sample.json", RootName="List">
    type Cards = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/cards.sample.json", RootName="Cards">
    type Card = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/card.sample.json", RootName="Card">
    type Attachments = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/attachments.sample.json", RootName="Attachments">
    //type Attachment = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/attachment.sample.json", RootName="Attachment">
    type CustomFields = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/customfields.sample.json", RootName="CustomFields">
    type CardCustomFields = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/customfields.oncard.sample.json", RootName="CardCustomFields">
    type CardSearchResults = JsonProvider<"https://raw.githubusercontent.com/fischgeek/FSharpDataProviderSampleFiles/master/json/trello/cardsearchresults.sample.json",RootName="CardSearchResults">
    
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