module dirsplitter.split

open dirsplitter.util
open System.IO
open System.Text.RegularExpressions


let increment key byAmount (map: Map<int, int64>) = map |> incrementMap key byAmount

let printTrackerMap (map: Map<int, int64>) =
    for KeyValue (key, value) in map do
        printfn "part%i : %iMB" key (value / int64 (1000.0 ** 2))

let splitDir (dir, maxBytes: int64, prefix) =
    let mutable tracker: Map<int, int64> = Map.empty
    let mutable currentPart = 1
    let mutable filesMoved = 0
    let mutable failedOps = 0
    let sep = Path.DirectorySeparatorChar

    // used to exclude files moved to newly created partDirs from getAllFiles
    let partDirRe = $"{dir}{sep}part\d+{sep}.*"

    getAllFiles dir
    |> Seq.where (fun s -> not (Regex.IsMatch(s, partDirRe)))
    |> Seq.iter (fun filepath ->
        let file = FileInfo(filepath)
        let size = file.Length
        let mutable currentPartSize = tracker |> findOrZero currentPart

        let decrementOnFailure =
            if currentPartSize > 0
               && (currentPartSize + size) > maxBytes then
                currentPart <- currentPart + 1
                currentPartSize <- 0
                true
            else
                false

        tracker <- tracker |> increment currentPart size

        let dest =
            Path.Join [| dir
                         $"part{currentPart}"
                         filepath.Replace(dir, "") |]

        let parentDir = (Directory.GetParent dest).FullName

        try
            Directory.CreateDirectory parentDir |> ignore
            file.MoveTo dest
            filesMoved <- filesMoved + 1
        with
        | e ->
            printfn "%s" e.Message
            failedOps <- failedOps + 1

            currentPart <-
                if decrementOnFailure then
                    currentPart - 1
                else
                    currentPart)


    printfn "Results:\nFiles Moved : %i \nFailed Operations : %i" filesMoved failedOps
    tracker |> printTrackerMap
