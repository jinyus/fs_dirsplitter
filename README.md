# dirsplitter
Split large directories into parts of a specified maximum size. This is a f# port of my dirsplitter tool.

This version is about 40MB
[Go version](https://github.com/jinyus/dirsplitter)<br> (5MB exe)
[Nim Version](https://github.com/jinyus/nim_dirsplitter)<br> (200KB exe)
[Crystal Version](https://github.com/jinyus/cr_dirsplitter) (300KB exe)

### How to install:
- Download the prebuild binary for your OS: https://github.com/jinyus/fs_dirsplitter/releases
- Extract the archive
- Open a terminal in the folder with the executeable
- Run  ```./dirsplitter --help```
- (Optional) Put the executable in your Path to run the tool from anywhere


### How to build:  
- Clone this git repo  
- Install dotnet : [https://dotnet.microsoft.com/en-us/download](https://dotnet.microsoft.com/en-us/download)
- cd into directory and compile with: 
```
dotnet publish -c Release -r <YOUR_RUNTIME>
```
- Avalaible runtimes:
    1. win-x64
    2. linux-x64
    3. linux-arm64
    4. osx-x64
    5. osx.12-arm64<br>
    full list [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#windows-rids)


```text
USAGE: dirsplitter [--help] [<subcommand> [<options>]]

SUBCOMMANDS:

    split <options>       Split directories into a specified maximum size.
    reverse <options>     Opposite of the main function, moves all files from part folders to the root.

    Use 'dirsplitter <subcommand> --help' for additional information.

OPTIONS:

    --help                display this list of options.
  ```
  ## SPLIT USAGE:
  
  ```text
  Splits directory into a specified maximum size

Usage:
  dirsplitter split [options] 

Options:
  -h, --help
  -d, --dir=DIR              Target directory (default: .)
  -m, --max=MAX              Max part size in GB (default: 5.0)
  -p, --prefix=PREFIX        Prefix for output files of the tar command. eg: myprefix.part1.tar (default:"")
 ```
  
### example: 
```text
dirsplitter split --dir ./mylarge2GBdirectory --max 0.5

This will yield the following directory structure:

📂mylarge2GBdirectory
 |- 📂part1
 |- 📂part2
 |- 📂part3
 |- 📂part4

with each part being a maximum of 500MB in size.
```
Undo splitting
```
dirsplitter reverse --dir ./mylarge2GBdirectory

```
