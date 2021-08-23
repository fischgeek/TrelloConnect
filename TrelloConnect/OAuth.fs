namespace TrelloConnect

open System.Net
open System.Net.Http

module OAuth = 
    let private MakeCallWithBody verb url (body:string) fn msg = 
        HRB.Url url
        |> HRB.Body body
        |> verb
        |> function
        | Success x -> fn x
        | _ -> failwith msg
    let private MakeCall verb url fn msg =
        HRB.Url url
        |> HRB.Body ""
        |> verb
        |> function
        | Success x -> fn x
        | _ -> failwith msg

    let requestTokenUrl = "https://trello.com/1/OAuthGetRequestToken"
    let authorizeTokenUrl = "https://trello.com/1/OAuthAuthorizeToken"
    let accessTokenUrl = "https://trello.com/1/OAuthGetAccessToken"

    let Authorize() =
        let http = new HttpClient()
        let res = MakeCall HttpPost requestTokenUrl (fun x -> printfn $"Response: {x}") ""
        res

