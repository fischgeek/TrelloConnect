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
        | Nothing
    let private ReturnResponse(res: HttpResponse): HttpResponseStatus =
        match res.StatusCode with
        | 401 -> Unauthorized
        | 429 -> Throttle
        | _ ->
            match res.Body with
            | Text txt -> Success txt
            | _ -> Nothing

    type HttpRequestBuilder = 
        {
            Url: string
            Query: (string * string) list
            Headers: (string * string) list
            Body: HttpRequestBody
        }
        static member Empty = { Url = ""; Query = []; Headers = []; Body = HttpRequestBody.TextRequest "" }
    
    type HRB =
        static member Url x = {HttpRequestBuilder.Empty with Url=x}
        static member Query x b = {b with Query = x}
        static member Headers x b = {b with HttpRequestBuilder.Headers = x}
        static member Body x b = {b with HttpRequestBuilder.Body = x}
    
    let HttpGet (x:HttpRequestBuilder) = 
        Http.Request(url = x.Url, query = x.Query, headers = x.Headers, httpMethod = "get") |> ReturnResponse

    let HttpPut (x:HttpRequestBuilder) = 
        Http.Request(url = x.Url, query = x.Query, headers = x.Headers, httpMethod = "put") |> ReturnResponse

    let HttpDel (x:HttpRequestBuilder) = 
        Http.Request(url = x.Url, query = x.Query, headers = x.Headers, httpMethod = "delete") |> ReturnResponse

    let HttpPost (x:HttpRequestBuilder) =
        Http.Request (url = x.Url, query = x.Query, headers = x.Headers, httpMethod = "post") |> ReturnResponse

    //let Post url (username: string) jsonBody=
    //    Http.Request
    //        (url,
    //         headers =
    //             [ FSharp.Data.HttpRequestHeaders.ContentType HttpContentTypes.Json
    //               FSharp.Data.HttpRequestHeaders.BasicAuth username "" ], body = (jsonBody |> TextRequest), silentHttpErrors = true)
    //    |> ReturnResponse
