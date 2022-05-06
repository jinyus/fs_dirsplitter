module dirsplitter.split

open dirsplitter.util
open System.IO
open System.Text.RegularExpressions


let increment key byAmount (map: Map<int, int64>) = map |> incrementMap key byAmount

let formatOutput result key value =
    sprintf "%s\npart%i : %iMB" result key (value / int64 (1000.0 ** 2))

let trackerMapToString (map: Map<int, int64>) = map |> Map.fold formatOutput ""

let splitDir (dir, maxBytes: int64, prefix: string) =

    confirmOperation $"""Split "{dir}" into {float (maxBytes) / 1000_000_000.0}GB parts?"""


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
        let currentPartSize = tracker |> findOrZero currentPart

        let decrementOnFailure =
            if currentPartSize > 0
               && (currentPartSize + size) > maxBytes then
                currentPart <- currentPart + 1
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


    printfn
        "Results:\n\nFiles Moved : %i \nFailed Operations : %i\n%s\n"
        filesMoved
        failedOps
        (tracker |> trackerMapToString)


    if currentPart > 0 && prefix.Trim().Length <> 0 then
        let newPrefix =
            if prefix.EndsWith "." then
                prefix
            else
                prefix + "."

        if currentPart = 1 then
            printfn """Tar Command : tar -cf "%spart1.tar" "part1"; done""" newPrefix
        else
            printfn """Tar Command : for n in {1..%i}; do tar -cf "%spart$n.tar" "part$n"; done""" currentPart newPrefix
