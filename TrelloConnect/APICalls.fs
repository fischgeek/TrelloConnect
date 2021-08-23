// fsharplint:disable NonPublicValuesNames
namespace TrelloConnect

open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open System

[<AutoOpen>]
module WebCalls =
    type HttpResponseStatus =
        | Unauthorized
        | Throttle
        | Success of string
        | Nothing of string
    let private ReturnResponse(res: HttpResponse): HttpResponseStatus =
        match res.StatusCode with
        | 401 -> Unauthorized
        | 429 -> Throttle
        | _ ->
            match res.Body with
            | Text txt -> Success txt
            | Binary value -> Nothing (System.Text.Encoding.Default.GetString(value))

    type HttpRequestBuilder = 
        {
            Url: string
            Query: (string * string) list
            Headers: (string * string) list
            Body: string
        }
        static member Empty = { Url = ""; Query = []; Headers = []; Body = "" }
    
    type HRB =
        static member Url x = {HttpRequestBuilder.Empty with Url=x}
        static member Query x b = {b with Query = x}
        static member Headers x b = {b with HttpRequestBuilder.Headers = x}
        static member Body x b = {b with HttpRequestBuilder.Body = x }
    
    let HttpGet (x:HttpRequestBuilder) = 
        Http.Request(url = x.Url, query = x.Query, headers = x.Headers, httpMethod = "get") |> ReturnResponse

    let HttpPut (x:HttpRequestBuilder) = 
        Http.Request(url = x.Url, query = x.Query, headers = x.Headers, httpMethod = "put", body = (x.Body |> TextRequest)) |> ReturnResponse
        
    let HttpPutJson (x:HttpRequestBuilder) = 
        Http.Request(url = x.Url, query = x.Query, headers = [ ContentType HttpContentTypes.Json ], httpMethod = "put", body = (x.Body |> TextRequest)) |> ReturnResponse

    let HttpDel (x:HttpRequestBuilder) = 
        Http.Request(url = x.Url, query = x.Query, headers = x.Headers, httpMethod = "delete") |> ReturnResponse

    let HttpPost (x:HttpRequestBuilder) =
        Http.Request (url = x.Url, query = x.Query, headers = [ ContentType HttpContentTypes.Json ], httpMethod = "post", body = (x.Body |> TextRequest)) |> ReturnResponse

    let HttpPostAsync (x:HttpRequestBuilder) =
        Http.AsyncRequest (url = x.Url, query = x.Query, headers = [ ContentType HttpContentTypes.Text ], httpMethod = "post", body = (x.Body |> TextRequest))

    //let Post url (username: string) jsonBody=
    //    Http.Request
    //        (url,
    //         headers =
    //             [ FSharp.Data.HttpRequestHeaders.ContentType HttpContentTypes.Json
    //               FSharp.Data.HttpRequestHeaders.BasicAuth username "" ], body = (jsonBody |> TextRequest), silentHttpErrors = true)
    //    |> ReturnResponse
