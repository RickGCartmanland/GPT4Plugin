# GPT4Module for F#

This is a simple F# console application that uses the OpenAI API to generate text completions with the GPT-4 language model.

## Features

- Sends requests to the OpenAI API to generate text completions based on a given prompt.
- Supports setting maximum tokens and number of generated text choices.
- Reads the API key from an environment variable or command-line argument.
- Handles API responses and displays generated text.

## Dependencies

- FSharp.Data: Used for the JSON type provider.
- System.Net.Http: Used for making HTTP requests to the OpenAI API.
- Argu: Used for command-line argument parsing.

## Usage

1. Build the project with `dotnet build`.
2. Run the application with the following command:

    ```sh
    dotnet run -- --api-key API_KEY --prompt PROMPT [--max-tokens MAX_TOKENS] [--n N]
    ```

    - Replace `API_KEY` with your OpenAI API key. Alternatively, you can set the `OPENAI_API_KEY` environment variable.
    - Replace `PROMPT` with the text prompt you want to use for the GPT-4 model.
    - Optionally, set `MAX_TOKENS` to the maximum number of tokens you want in the generated text (default is 50).
    - Optionally, set `N` to the number of generated text choices (default is 1).

## Command-Line Arguments

| Argument      | Description                                                                                      |
|---------------|--------------------------------------------------------------------------------------------------|
| --api-key     | Your OpenAI API key. Can be set as an environment variable (OPENAI_API_KEY).                     |
| --prompt      | The text prompt for the GPT-4 model.                                                             |
| --max-tokens  | The maximum number of tokens in the generated text. Default is 50.                               |
| --n           | The number of generated text choices. Default is 1.                                              |
| --help        | Display help text for command-line arguments.                                                    |

## Example

To generate a text completion using the GPT-4 model with the prompt "Tell me something interesting about human life.", run:

```sh
dotnet run -- --api-key your_api_key --prompt "Tell me something interesting about human life."
```

Replace `your_api_key` with your actual OpenAI API key.