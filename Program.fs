open System
open Argu
open System.IO
open dirsplitter
open dirsplitter.split
open dirsplitter.reverse

let version = "1.0.2"
let GBMUltiple = 1000.0 ** 3

let getDir (args: ParseResults<ActionArgs>) =
    Path.GetFullPath(args.GetResult(Dir, defaultValue = "."))

let parseArgs (args: ParseResults<ActionArgs>) =
    let maxSize =
        int64 (
            args.GetResult(Max, defaultValue = 5.0)
            * GBMUltiple
        )

    let prefix =
        args.GetResult(Prefix, defaultValue = "")


    (getDir args), maxSize, prefix



[<EntryPoint>]
let main argv =
    let parser =
        ArgumentParser.Create<DirSplitterArgs>(programName = "dirsplitter")

    try
        let result =
            parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)

        let mode = result.GetAllResults()

        match mode with
        | [ Split args ] -> splitDir (parseArgs args)
        | [ Reverse args ] -> reverseSplit (getDir args)
        | [ Version ] -> printfn "Dirsplitter version : %s" version
        | _ -> printfn "%s" (parser.PrintUsage())
    with
    | e -> printfn "%s" e.Message

    0
