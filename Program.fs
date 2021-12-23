open System
open Argu
open System.IO
open dirsplitter
open dirsplitter.split
open dirsplitter.reverse


let GBMUltiple = 1000.0 ** 3

let parseArgs (args: ParseResults<ActionArgs>) =
    Path.GetFullPath(args.GetResult(Dir, defaultValue = ".")),
    int64 (
        args.GetResult(Max, defaultValue = 5.0)
        * GBMUltiple
    ),
    args.GetResult(Prefix, defaultValue = "")

[<EntryPoint>]
let main argv =
    let parser =
        ArgumentParser.Create<DirSplitterArgs>(programName = "dirsplitter")

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
