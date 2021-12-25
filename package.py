from pathlib import Path
import os

version = '1.0.1'

tar_names = {
    'linux-x64': f'fs_dirsplitter_{version}_linux_amd64.tar.gz',
    'win-x64': f'fs_dirsplitter_{version}_windows_amd64.tar.gz',
    'osx-x64': f'fs_dirsplitter_{version}_osx_amd64.tar.gz',
    'osx-arm64': f'fs_dirsplitter_{version}_osx_arm64.tar.gz',
}

os_releases = Path('./bin/Release').glob('*/*')

os_releases = [folder for folder in os_releases if folder.stem in tar_names]

for folder in os_releases:
    name = folder.stem
    tar_name = tar_names[name]
    publish_dir = folder.joinpath("publish")
    dest_dir = Path('./bin/Dist').joinpath(version, name)

    if dest_dir.exists() and os.listdir(dest_dir):
        print(f'{dest_dir} is not empty')
        continue

    mkdir_command = f'mkdir -p {dest_dir}'
    os.system(mkdir_command)
    command = f'cp {publish_dir} {dest_dir} -r'
    os.system(command)

for folder in os_releases:
    name = folder.stem
    tar_name = tar_names[name]
    tar_dir = Path('./bin/Dist').joinpath(version)
    target_dir = tar_dir.joinpath(name)
    tar_command = f'tar -czf "{tar_dir.joinpath(tar_name)}" "{target_dir}"'
    os.system(tar_command)
