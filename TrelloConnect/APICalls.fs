// fsharplint:disable NonPublicValuesNames
namespace TrelloConnect

open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open System

[<AutoOpen>]
module WebCalls =
    type WebCallResult = 
        | Success
        | Failure of string
    type HttpResultDataType =
        | String of string
        | Bytes of byte seq
    type HttpResponseStatus =
        | BadRequest
        | Unauthorized
        | Throttle
        | Success of HttpResultDataType
        //| Nothing of string
    let private ReturnResponse(res: HttpResponse): HttpResponseStatus =
        match res.StatusCode with
        | 400 -> BadRequest
        | 401 -> Unauthorized
        | 429 -> Throttle
        | _ ->
            match res.Body with
            | Text txt -> Success (String txt)
            | Binary value -> Success (Bytes value)
                //Nothing (System.Text.Encoding.Default.GetString(value))

    type HttpRequestBuilder = 
        {
            Url: string
            Query: (string * string) list
            Headers: (string * string) list
            Body: string
            OAuth1Key: string
            OAuth1Token: string
        }
        static member Empty = { Url = ""; Query = []; Headers = []; Body = ""; OAuth1Key = ""; OAuth1Token = "" }
        member this.GetHeaders() =
            let authString = 
                $""" 
                OAuth oauth_consumer_key="{this.OAuth1Key}"
                , oauth_nonce=""
                , oauth_signature=""
                , oauth_signature_method="HMAC-SHA1"
                , oauth_timestamp=""
                , oauth_token="{this.OAuth1Token}"
                , oauth_version="1.0"
                """
            this.Headers @
                if this.OAuth1Key |> SP.IsEmpty then []
                else [("authorization", authString)]
    type HRB =
        static member Url x = {HttpRequestBuilder.Empty with Url=x}
        static member Query x b = {b with Query = x}
        static member Headers x b = {b with HttpRequestBuilder.Headers = x}
        static member Body x b = {b with HttpRequestBuilder.Body = x }
        static member OAuth1 key tok b = {b with OAuth1Key = key; OAuth1Token = tok}
    
    let HttpGet (x:HttpRequestBuilder) =
        Http.Request(url = x.Url, query = x.Query, headers = x.GetHeaders(), httpMethod = "get") |> ReturnResponse

    let HttpPut (x:HttpRequestBuilder) = 
        Http.Request(url = x.Url, query = x.Query, headers = x.GetHeaders(), httpMethod = "put", body = (x.Body |> TextRequest)) |> ReturnResponse
        
    let HttpPutJson (x:HttpRequestBuilder) = 
        Http.Request(url = x.Url, query = x.Query, headers = x.GetHeaders() @ [ ContentType HttpContentTypes.Json ], httpMethod = "put", body = (x.Body |> TextRequest)) |> ReturnResponse

    let HttpDel (x:HttpRequestBuilder) = 
        let h = x.GetHeaders()
        let q = x.Query
        let u = x.Url
        try Http.Request(url = u, query = q, headers = h, httpMethod = HttpMethod.Delete) |> ReturnResponse
        with ex -> failwith ex.Message

    let HttpPost (x:HttpRequestBuilder) =
        Http.Request (url = x.Url, query = x.Query, headers = x.GetHeaders() @ [ ContentType HttpContentTypes.Json ], httpMethod = "post", body = (x.Body |> TextRequest)) |> ReturnResponse

    let HttpPostAsync (x:HttpRequestBuilder) =
        Http.AsyncRequest (url = x.Url, query = x.Query, headers = x.GetHeaders() @ [ ContentType HttpContentTypes.Text ], httpMethod = "post", body = (x.Body |> TextRequest))

    //let Post url (username: string) jsonBody=
    //    Http.Request
    //        (url,
    //         headers =
    //             [ FSharp.Data.HttpRequestHeaders.ContentType HttpContentTypes.Json
    //               FSharp.Data.HttpRequestHeaders.BasicAuth username "" ], body = (jsonBody |> TextRequest), silentHttpErrors = true)
    //    |> ReturnResponse