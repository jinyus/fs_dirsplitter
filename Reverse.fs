module dirsplitter.reverse

open System.IO
open dirsplitter.util
open System.Text.RegularExpressions

let sep = Path.DirectorySeparatorChar


// move file and return the part directory for deletion
let moveFile (dir: string) (filepath: string) : option<string> =
    let partDirRe = $"{dir}{sep}part\d+"
    let partDir = (Regex.Match(filepath, partDirRe)).Value
    let dest = filepath.Replace(partDir, dir)
    let parentDir = (Directory.GetParent dest).FullName

    try
        Directory.CreateDirectory parentDir |> ignore
        (FileInfo filepath).MoveTo dest
        Some(partDir)
    with
    | e ->
        printfn "%s" e.Message
        None

let reverseSplit dir =
    // used to include only files in partDirs from getAllFiles
    let partDirRe = $"{dir}{sep}part\d+{sep}.*"

    let foldersToDelete =
        getAllFiles dir
        |> Seq.where (fun s -> Regex.IsMatch(s, partDirRe))
        |> Seq.map (fun filepath -> moveFile dir filepath)
        |> Seq.distinct
        |> Seq.toList

    // if moveFile returned a None, then an error occured so
    // skip deletion of the part directories
    let errorOccurred =
        foldersToDelete |> List.exists (fun x -> x = None)

    if not errorOccurred then
        foldersToDelete
        |> List.filter (fun o -> o <> None)
        |> List.map (fun o -> Option.get o)
        |> List.iter (fun dir -> Directory.Delete(dir, true))
    |> ignore
