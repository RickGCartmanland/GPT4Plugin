open System
open Argu
open GPT4Module

let defaultPrompt = "Kérlek, mondj valamit érdekeset az emberi életről."
let defaultMaxTokens = 500
let defaultN = 1

type CommandLineArgs =
    | [<MainCommand>] ApiKey of apiKey: string
    | Prompt of prompt: string
    | [<AltCommandLine("maxTokens", "mt")>] MaxTokens of maxTokens: int
    | [<AltCommandLine("n", "num")>] N of n: int
    | [<CliPrefix(CliPrefix.None); CustomCommandLine("help")>] Help
    with
        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | ApiKey _ -> "Your OpenAI API key. Can be set as an environment variable (OPENAI_API_KEY)."
                | Prompt _ -> "The text prompt for the GPT-4 model. Default is: " + defaultPrompt
                | MaxTokens _ -> "The maximum number of tokens in the generated text. Default is " + string defaultMaxTokens + "."
                | N _ -> "The number of generated text choices. Default is " + string defaultN + "."
                | Help -> "Display this help text."

let printHelp (parser:ArgumentParser<CommandLineArgs>)=
    printfn "Usage: GPT4App --api-key API_KEY --prompt PROMPT [--max-tokens MAX_TOKENS] [--n N]"
    printfn "%s" (parser.PrintUsage())

[<EntryPoint>]
let main args =
    let parser : ArgumentParser<CommandLineArgs> = ArgumentParser.Create<CommandLineArgs>()
    let parsedArgs = parser.Parse(args)

    try
        if parsedArgs.Contains Help then
            printHelp parser
        else
            let apiKey =
                match parsedArgs.TryGetResult(ApiKey) with
                | Some apiKey -> apiKey
                | None -> System.Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            
            let prompt = parsedArgs.GetResult(Prompt, defaultPrompt)
            let maxTokens = parsedArgs.GetResult(MaxTokens, defaultMaxTokens)
            let n = parsedArgs.GetResult(N, defaultN)

            GPT4Module.main apiKey prompt maxTokens n
        0
    with
    | :? ArguParseException as ex ->
        printfn "Error: %s" ex.Message
        printHelp parser
        -1
