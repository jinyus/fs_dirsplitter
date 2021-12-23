open System
open Argu
open dirsplitter
open dirsplitter.split
open dirsplitter.reverse

let parseArgs (args: ParseResults<ActionArgs>) =
    args.GetResult(Dir, defaultValue = "."),
    args.GetResult(Max, defaultValue = 5.0),
    args.GetResult(Prefix, defaultValue = "")

[<EntryPoint>]
let main argv =
    let errorHandler =
        ProcessExiter(
            colorizer =
                function
                | ErrorCode.HelpText -> None
                | _ -> Some ConsoleColor.Red
        )

    let parser =
        ArgumentParser.Create<DirSplitterArgs>(programName = "dirsplitter", errorHandler = errorHandler)

    try
        let result =
            parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)

        let mode = result.GetSubCommand()

        match mode with
        | Split args -> splitDir (parseArgs args)
        | Reverse args -> reverseSplit (parseArgs args)
    with
    | e -> printfn "%s" e.Message

    0
