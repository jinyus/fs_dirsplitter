namespace dirsplitter

open Argu

[<CliPrefix(CliPrefix.Dash)>]
type CleanArgs =
    | D
    | F
    | X

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | D -> "Remove untracked directories in addition to untracked files"
            | F -> "Git clean will refuse to delete files or directories unless given -f."
            | X -> "Remove only files ignored by Git."

type CommitArgs =
    | Amend
    | [<AltCommandLine("-p")>] Patch
    | [<AltCommandLine("-m")>] Message of msg: string

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Amend -> "Replace the tip of the current branch by creating a new commit."
            | Patch -> "Use the interactive patch selection interface to chose which changes to commit."
            | Message _ -> "Use the given <msg> as the commit message. "

type GitArgs =
    | Version
    | [<AltCommandLine("-v")>] Verbose
    | [<CliPrefix(CliPrefix.None)>] Clean of ParseResults<CleanArgs>
    | [<CliPrefix(CliPrefix.None)>] Commit of ParseResults<CommitArgs>

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Version -> "Prints the Git suite version that the git program came from."
            | Verbose -> "Print a lot of output to stdout."
            | Clean _ -> "Remove untracked files from the working tree."
            | Commit _ -> "Record changes to the repository."


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

type DirSplitterArgs =
    | [<CliPrefix(CliPrefix.None)>] Split of ParseResults<ActionArgs>
    | [<CliPrefix(CliPrefix.None)>] Reverse of ParseResults<ActionArgs>

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Split _ -> "Split directories into a specified maximum size."
            | Reverse _ -> "Opposite of the main function, moves all files from part folders to the root."
