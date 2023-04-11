module Tests

open Xunit
open GPT4Module
open System.Net.Http
open System.Net
open System.IO
open System
open System.Net.Http.Headers
open System.Text
open System.Net.Http
open System.Threading
open System.Threading.Tasks

let createHttpResponseMessage (statusCode: HttpStatusCode) (content: Option<HttpContent>) : HttpResponseMessage =
    let response = new HttpResponseMessage(statusCode)
    match content with
    | Some httpContent -> response.Content <- httpContent
    | None -> ()
    response

type MockHttpHandler() =
    inherit DelegatingHandler()

    override this.SendAsync(request: HttpRequestMessage, cancellationToken: CancellationToken) : Task<HttpResponseMessage> =
        if request.Method = HttpMethod.Post then
            let responseContent = new StringContent("{}", Encoding.UTF8, "application/json")
            Task.FromResult(createHttpResponseMessage HttpStatusCode.OK (Some responseContent))
        else
            Task.FromResult(createHttpResponseMessage HttpStatusCode.NotFound None)

[<Fact>]
let ``Test sendGpt4Request``() =
    // Mock HttpClient
    let mockHandler = new MockHttpHandler()
    let mockClient = new HttpClient(mockHandler)

    let requestBody = { prompt = "Test prompt"; max_tokens = 10; n = 1 }
    let baseUrl = "https://api.example.com/v1/engines/text-davinci-002/completions"
    let response = sendGpt4Request mockClient baseUrl requestBody |> Async.RunSynchronously
    Assert.True(response.IsSuccessStatusCode)

[<Fact>]
let ``Test createAuthorizedHttpClient with empty API key``() =
    let apiKey = ""
    let client = createAuthorizedHttpClient apiKey
    Assert.NotNull(client)

[<Fact>]
let ``Test sendGpt4Request with invalid base URL``() =
    let mockHandler = new MockHttpHandler()
    let mockClient = new HttpClient(mockHandler)

    let requestBody = { prompt = "Test prompt"; max_tokens = 10; n = 1 }
    let invalidBaseUrl = "https://invalid.example.com/invalid"
    let response = sendGpt4Request mockClient invalidBaseUrl requestBody |> Async.RunSynchronously
    Assert.False(response.IsSuccessStatusCode)

[<Fact>]
let ``Test sendGpt4Request with non-POST request``() =
    let mockHandler = new MockHttpHandler()
    let mockClient = new HttpClient(mockHandler)

    let requestBody = { prompt = "Test prompt"; max_tokens = 10; n = 1 }
    let validBaseUrl = "https://api.example.com/v1/engines/text-davinci-002/completions"
    let response = sendGpt4Request mockClient validBaseUrl requestBody |> Async.RunSynchronously
    Assert.False(response.IsSuccessStatusCode)

[<Fact>]
let ``Test handleResponse with successful status code and valid GPT4 response``() =
    let successfulResponseContent = new StringContent("""{"id": "", "object": "", "created": 0, "model": "", "usage": { "prompt_tokens": 0, "completion_tokens": 0, "total_tokens": 0 }, "choices": [ { "text": "Test response", "index": 0, "logprobs": null, "finish_reason": "" } ] }""", Encoding.UTF8, "application/json")
    let successfulResponse = createHttpResponseMessage HttpStatusCode.OK (Some successfulResponseContent)
    let result = handleResponse successfulResponse |> Async.RunSynchronously
    Assert.Equal("Test response", result)

[<Fact>]
let ``Test handleResponse with unsuccessful status code``() =
    let unsuccessfulResponse = createHttpResponseMessage HttpStatusCode.BadRequest None
    let exn = Assert.Throws<Exception>(fun () -> (handleResponse unsuccessfulResponse |> Async.RunSynchronously) |> ignore)
    Assert.Contains("Error", exn.Message)

