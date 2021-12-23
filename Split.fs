module dirsplitter.split

open dirsplitter.util
open System.IO

type SplitResult =
    { tracker: Map<int, int64>
      currentPart: int
      filesMoved: int
      failedOps: int }

    static member Default =
        { tracker = Map.empty
          currentPart = 1
          filesMoved = 0
          failedOps = 0 }

    member this.increment key byAmount =
        { this with tracker = this.tracker |> incrementMap key byAmount }


let moveFile (state: SplitResult) (filepath: string) dir maxBytes : SplitResult =
    let file = FileInfo(filepath)
    let size = file.Length
    let mutable currentPart = state.currentPart
    let mutable filesMoved = state.filesMoved
    let mutable failedOps = state.failedOps

    let mutable currentPartSize = state.tracker |> findOrZero currentPart


    let decrementOnFailure =
        if currentPartSize > 0
           && (currentPartSize + size) > maxBytes then
            currentPart <- currentPart + 1
            currentPartSize <- 0
            true
        else
            false


    let res = state.increment currentPart size

    let dest =
        Path.Join [| dir
                     $"part{currentPart}"
                     filepath.Replace(dir, "") |]

    try
        let parentDir = (Directory.GetParent dest).FullName
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
                currentPart

    { res with
        currentPart = currentPart
        filesMoved = filesMoved
        failedOps = failedOps }



let splitDir (dir, maxBytes: int64, prefix) =
    let result =
        getAllFiles dir
        |> Seq.toList
        |> List.fold (fun r s -> moveFile r s dir maxBytes) SplitResult.Default

    printfn "result %O %i" result maxBytes
