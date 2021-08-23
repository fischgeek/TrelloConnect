namespace TrelloConnect

open System.Net
open System.Net.Http
open System.Text.RegularExpressions
open System.IO

module Authorize = 
    type HttpListener with
        static member RunFull maxAttempts (url: string) (handler: HttpListenerRequest -> HttpListenerResponse -> string -> Async<unit>) =
            let rec run attempt =
                try
                    let listener = new HttpListener()
                    listener.Prefixes.Add url
                    listener.Start()
                    let asynctask = Async.FromBeginEnd(listener.BeginGetContext, listener.EndGetContext)
                    async {
                        while true do
                            let! context = asynctask
                            let str = new StreamReader(context.Request.InputStream)
                            let s = str.ReadToEnd()
                            Async.Start(handler context.Request context.Response s)
                    }
                    |> Async.Start
                    listener
                with ex -> failwith $"error: {ex.Message}"
            run 0
    let private outInfo s = printfn $"{s.ToString()}"
    let private LightListenerFull maxAttempts (port: int) path (handler: HttpListenerRequest -> HttpListenerResponse -> string -> Async<unit>) =
        if port >= int32 System.Int16.MaxValue then failwith $"Port number is too high {port}"
        let addr = sprintf "http://*:%i/%s/" port path
        sprintf "Listen to %s" addr |> outInfo
        HttpListener.RunFull maxAttempts addr handler
    let LightListener (port: int) path (handler: HttpListenerRequest -> HttpListenerResponse -> string -> Async<unit>) = LightListenerFull 15 port path handler
    let LightListenerQuiet port path handler = LightListener port path handler |> ignore

    let private makeCallWithBody verb url (body:string) fn msg = 
        HRB.Url url
        |> HRB.Body body
        |> verb
        |> function
        | Success x -> fn x
        | _ -> failwith msg
    let private makeCall verb url fn msg =
        HRB.Url url
        |> HRB.Body ""
        |> verb
        |> function
        | Success x -> fn x
        | _ -> failwith msg

    let private callback = "fragment"
    let private exp = "1day"
    let private name = "WallDash"
    let private responseType = "fragment"
    let private responseUrl = "localhost:5150/walldash"
    let private key = ""
    let private authUrl = $"https://trello.com/1/authorize?callback_method={callback}&expiration={exp}&name={name}&response_type={responseType}&response_url={responseUrl}&key={key}&scope=read"

    let TryAuthorize() =
        LightListenerQuiet 5150 "walldash" (fun req resp z ->
            async {
                printfn $"{resp}"
            }
        )
        let res = makeCall HttpGet authUrl (fun x -> printfn $"Response: {x}") ""
        res

