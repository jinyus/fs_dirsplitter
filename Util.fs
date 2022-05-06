module dirsplitter.util

open System.IO

let rec getAllFiles dir =
    seq {
        yield! Directory.EnumerateFiles(dir)

        for d in Directory.EnumerateDirectories(dir) do
            yield! getAllFiles d
    }

let incrementOption (o: option<int64>) (byAmount: int64) =
    match o with
    | Some value -> Some(value + byAmount)
    | None -> Some byAmount

let incrementMap (key: int) (byAmount: int64) (map: Map<int, int64>) =
    map
    |> Map.change key (fun o -> incrementOption o byAmount)

let findOrZero key (map: Map<int, int64>) =
    match map.TryFind key with
    | Some v -> v
    | None -> 0


let confirmOperation s =
    printf "%s \n(yes/no) : " s

    let ans =
        System.Console.ReadLine().Trim().ToLower()

    if ans = "n" || ans = "no" then exit 0
