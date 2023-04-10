module GPT4Module

open System
open System.Net.Http
open System.Text.Json
open FSharp.Data

// Define Gpt4Response and RequestBody types
type Gpt4Response = JsonProvider<"""{ "id": "", "object": "", "created": 0, "model": "", "usage": { "prompt_tokens": 0, "completion_tokens": 0, "total_tokens": 0 }, "choices": [ { "text": "", "index": 0, "logprobs": null, "finish_reason": "" } ] }""">
type RequestBody = { prompt : string; max_tokens : int; n : int }

let createAuthorizedHttpClient (apiKey: string) : HttpClient = 
    let client = new HttpClient()
    client.DefaultRequestHeaders.Authorization <- new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey)
    client

let sendGpt4Request (client: HttpClient) (baseUrl: string) (requestBody: RequestBody) : Async<HttpResponseMessage> =
    async {
        let requestJson = JsonSerializer.Serialize(requestBody)
        let content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json")
        let! response = client.PostAsync(baseUrl, content) |> Async.AwaitTask
        return response
    }

let handleResponse (response: HttpResponseMessage) : Async<unit> =
    async {
        if response.IsSuccessStatusCode then
            let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            let gpt4Response = Gpt4Response.Parse(content)
            let generatedText = gpt4Response.Choices.[0].Text
            let trimmedText = generatedText.ToString().Trim()
            printfn "Generated text: %s" trimmedText
        else
            printfn "Error: %s" response.ReasonPhrase
    }

let main (apiKey: string) (prompt: string) (maxTokens: int) (n: int) : unit =
    if String.IsNullOrEmpty(apiKey) then
        printfn "Error: Please set the OPENAI_API_KEY environment variable."
    else
        let baseUrl = "https://api.openai.com/v1/engines/text-davinci-002/completions"
        let client = createAuthorizedHttpClient apiKey
        let requestBody = { prompt = prompt; max_tokens = maxTokens; n = n }
        async {
            let! response = sendGpt4Request client baseUrl requestBody
            do! handleResponse response
        }
        |> Async.RunSynchronously
