namespace dirsplitter

open Argu

type ActionArgs =
    | [<AltCommandLine("-d")>] Dir of directory: string
    | [<AltCommandLine("-m")>] Max of maxGB: float
    | [<AltCommandLine("-p")>] Prefix of prefix: string

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Dir _ -> "Target directory."
            | Max _ -> "Max part size in GB."
            | Prefix _ -> "Prefix for output files of the tar command."

type TargetDir =
    | [<AltCommandLine("-d")>] Dir of directory: string

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Dir _ -> "Target directory."


type DirSplitterArgs =
    | [<CliPrefix(CliPrefix.None)>] Split of ParseResults<ActionArgs>
    | [<CliPrefix(CliPrefix.None)>] Reverse of ParseResults<TargetDir>

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Split _ -> "Split directories into a specified maximum size."
            | Reverse _ -> "Opposite of the main function, moves all files from part folders to the root."
