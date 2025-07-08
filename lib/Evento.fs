//! generic embedded project generation script in F#

// project metainfo
let APP   = "wecs"
let TITLE = "Entity-Component-System for Web"
let ABOUT = ""

// mostly constant metainfo
let VERSION = "0.0.1"
let AUTHOR = "Dmitry Ponyatov"
let EMAIL = "dponyatov@gmail.com"
let YEAR = 2025
let LICENSE = "MIT"
let GITHUB = $"https://github.com/ponyatov/{APP}"

// file generation
open System
open System.IO

let touch (path: string) : unit =
    if not (File.Exists(path)) then
        File.WriteAllText(path, "")

let mkdir (path: string) : unit =
    if not (Directory.Exists(path)) then
        Directory.CreateDirectory(path) |> ignore
    let giti = Path.Combine(path, ".gitignore")
    if not (File.Exists(giti)) then
        File.WriteAllText(giti,"!.gitignore\n")

let NewLines = List.reduce (fun a b -> $"{a}\n{b}")

// env
let USER = Environment.UserName
let HOME = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)

// project dir
let app  = APP.ToLower()
let CWD = $"{HOME}/{APP}"
mkdir CWD
Directory.SetCurrentDirectory(CWD)
let CODE = $"code -r {CWD} ; sleep 5 ; code {HOME}/em/lib/Evento.fs"

let README:unit = //
    File.WriteAllText ("README.md",$"# ![](doc/logo.png) `{APP}` {VERSION}
## {TITLE}

(c) {AUTHOR} <{EMAIL}> {YEAR} {LICENSE}

github: {GITHUB}
{ABOUT}")

let LICFILE:unit = //
    File.WriteAllText ("LICENSE",$"MIT License

Copyright (c) {YEAR} {AUTHOR} <{EMAIL}>

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the \"Software\"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
")


// github repo
let SHELL = $"cd {CWD}"
let INIT = "git init"
let CHECKOUT = $"git checkout --orphan {USER}"
let RC = "ln -fs ../rc rc"
let CLONE = $"git clone -o gh git@github.com:ponyatov/{app}.git {HOME}/{APP}"
let GH   = $"git remote add gh git@github.com:ponyatov/{app}.git"
let FLIC = $"git remote add flic git@gitflic.ru:dponyatov/{app}.git"
let GITGUI = $"git gui &"
let PULL = $"git pull -v gh {USER}"
let COMMIT = $"git add -A ; git commit -am \".\" ; git push -v -u gh {USER}"

let bin:unit = //
    for d in ["bin"; "tmp"; "ref"] do
        mkdir d
        File.WriteAllText($"{d}/.gitignore","*\n!.gitignore\n")

let doc:unit = //
    mkdir "doc"
    File.WriteAllText($"doc/.gitignore","html/\n!.gitignore\n")
    doxy

let doxy: unit = //
    mkdir "doc"
    File.WriteAllText (".doxygen",$"PROJECT_NAME           = \"{APP}\"
PROJECT_BRIEF          = \"{TITLE}\"
PROJECT_LOGO           = doc/logo.png
")
    let LOGO = "cp ~/icons/control64.png doc/logo.png"
    let DOXY = "doxygen -l ; mv DoxygenLayout.xml doc/"
    let DOTX = "meld .doxygen ~/em/.doxygen"

let lib:unit = //
    mkdir "lib"
    File.WriteAllText($"lib/{APP}.ini", "# line comment\n")

let cpp: unit = //
    mkdir "inc"
    touch $"inc/{APP}.hpp"
    mkdir "src"
    touch $"src/{APP}.cpp"
    touch $"src/{APP}.lex"
    touch $"src/{APP}.yacc"

let rust: unit = //
    mkdir ".cargo"
    touch ".cargo/config.toml"
    mkdir "src"
    touch "src/lib.rs"
    File.WriteAllText ( "src/main.rs",
        "fn main() { println!(\"Hello, world!\"); }\n")
    File.WriteAllText ( "Cargo.toml", $"\
[package]
name        =  \"{app}\"
version     =  \"{VERSION}\"
description =  \"{TITLE}\"
authors     = [\"{AUTHOR} <{EMAIL}>\"]
license     =  \"{LICENSE}\"
repository  =  \"{GITHUB}\"
edition     =  \"2024\"

[dependencies]
const_format = \"0.2\"
")
    touch "src/config.rs" ; touch "src/server.rs"

let html:unit = //
    mkdir "static"
    mkdir "static/cdn"
    File.WriteAllText ("static/cdn/.gitignore","*\n!.gitignore\n")
    touch "static/index.html"
    touch "static/css.css"
    touch $"static/{app}.js"
    touch $"src/{app}.ts"

let src:unit = //
    cpp
    rust
    html

let cross_ name = //
    mkdir $"{name}"
    mkdir $"{name}/inc"
    mkdir $"{name}/src"
    File.WriteAllText ($"{name}/inc/{name}.hpp",$"/// @defgroup {name} {name}\n/// @ingroup cross\n")
    File.WriteAllText ($"{name}/src/{name}.cpp",$"#include \"{name}.hpp\"\n")

let hw:unit = //
    cross_ "hw"

    for hw,cpu in [
        ("qemu386","i486"); ("retro","i686"); ("pc","i5");
        ("rpi3","bcm2837"); ("rpi4","bcm2711"); ("rpi5","bcm2712"); ("opi800","rk3399");
        ("pillf103","stm32f103c8"); ("f429disco","stm32f429zi");
        ("netduinoplus2","stm32f405rg");
        ("iskra","stm32f405rg"); ("f4disco","stm32f407vg");
        ("esp8266","lx106"); ("esp32","lx106");
        ] do
            mkdir $"hw/{hw}"
            File.WriteAllText ($"hw/{hw}/{hw}.mk",$"CPU = {cpu}")
            touch $"hw/{hw}/{hw}.cmake"
            mkdir $"hw/{hw}/inc"
            mkdir $"hw/{hw}/src"
            touch $"hw/{hw}/inc/{hw}.hpp"
            touch $"hw/{hw}/src/{hw}.cpp"

let cpu:unit = //
    cross_ "cpu"

    for cpu,arch in [
        ("i5","x86_64"); ("i486","i386"); ("i686","i386");
        ("stm32f103c8","cortexm3"); ("stm32f429zi","cortexm4");
        ("stm32f405rg","cortexm4"); ("stm32f407vg","cortexm4");
        ("lx106","xtensa");
        ] do
            mkdir $"cpu/{cpu}"
            File.WriteAllText ($"cpu/{cpu}/{cpu}.mk",$"ARCH = {arch}")
            touch $"cpu/{cpu}/{cpu}.cmake"
            mkdir $"cpu/{cpu}/inc"
            mkdir $"cpu/{cpu}/src"
            File.WriteAllText ( $"cpu/{cpu}/inc/{cpu}.hpp",$"/// #defgroup {cpu} {cpu}\n/// @ingroup cpu\n")
            File.WriteAllText ( $"cpu/{cpu}/src/{cpu}.cpp",$"#include \"{cpu}.hpp\"\n")

let arch:unit = //
    cross_ "arch"

    for arch in [
        "x86_64"; "i386";
        "aarch64";
        "cortexm"; "cortexm3"; "cortexm4"; "xtensa";
        ] do
            mkdir $"arch/{arch}"
            touch $"arch/{arch}/{arch}.mk"
            touch $"arch/{arch}/{arch}.cmake"
            mkdir $"arch/{arch}/inc"
            mkdir $"arch/{arch}/src"
            File.WriteAllText ( $"arch/{arch}/inc/{arch}.hpp",$"/// #defgroup {arch} {arch}\n/// @ingroup arch\n")
            File.WriteAllText ( $"arch/{arch}/src/{arch}.cpp",$"#include \"{arch}.hpp\"\n")

let os:unit = //
    cross_ "os"
    for os in ["linux";"none";"freertos";"win32";"rtos8266";"idf"] do
        mkdir $"os/{os}" ; touch $"os/{os}/{os}.mk" ; touch $"os/{os}/{os}.cmake"
        mkdir $"os/{os}/inc" ; mkdir $"os/{os}/src"
        File.WriteAllText ( $"os/{os}/inc/{os}.hpp",$"/// #defgroup {os} {os}\n/// @ingroup os\n")
        File.WriteAllText ( $"os/{os}/src/{os}.cpp",$"#include \"{os}.hpp\"\n")

let cross:unit = //
    hw
    cpu
    arch
    os

let vscode:unit = //
    mkdir ".vscode"
    let jsons = [
        "c_cpp_properties";
        "extensions";
        "launch";
        "settings";
        "tasks" ]
    for j in jsons do
        File.WriteAllText($".vscode/{j}.json","{\n}\n")
    let MELD = "meld .vscode ~/em/.vscode"

let dirs:unit = //
    bin
    doc
    lib
    src
    cross
    vscode

let mk: unit = //
    mkdir "mk"
    let makes = ["var";"version";"dir";"tool";"src";"all";"format";"rule";"doc";"rust";"python";"ts";"gz";"install";"ai"]
    for m in makes do
        touch $"mk/{m}.mk"
    File.WriteAllText("Makefile",
        makes |> List.map (fun m -> $"include mk/{m}.mk") |> NewLines)
    let MK = $"meld mk ~/em/mk"

let cmake: unit = //
    touch "CMakeLists.txt"
    touch "CMakePresets.json"
    mkdir "cmake"
    let cmakes = ["any_toolchain"; "x86_64-linux-gnu"; "arm-none-eabi";
        "xtensa-lx106-elf"; "aarch64-linux-gnu"; "i686-w64-mingw32";
        "syntax"; "FindLEMON"; "FindRAGEL"; "FindReadline";
        "version"; "src"; "install"; "cross"; "clean"]
    for cm in cmakes do
        touch $"cmake/{cm}.cmake"

let giti:unit = //
    File.WriteAllText(".gitignore","""*~
*.swp
*.log
*.o
*.exe
node_modules/
/target/
/obj/
!.gitignore
""")

let apt:unit = //
    File.WriteAllText ("apt.Debian","""git make curl
code meld doxygen clang-format
g++ cmake gdb gdb-multiarch
flex bison libreadline-dev ragel lemon
python3 python3-venv python3-autopep8 python3-ply
dotnet-runtime-9.0 dotnet-sdk-9.0
qemu-system-arm
    gcc-arm-none-eabi openocd newlib-source dfu-util stlink-tools
qemu-system-x86
    g++-mingw-w64-i686
""")


let clang_format:unit = //
    File.WriteAllText (".clang-format","""BasedOnStyle: Google
IndentWidth:  4
TabWidth:     4
UseTab:       Never
ColumnLimit:  80
UseCRLF:      false

SortIncludes: false

AllowShortBlocksOnASingleLine: Always
AllowShortFunctionsOnASingleLine: All
""")

let prettierrc:unit = //
    File.WriteAllText (".prettierrc","""{
    "tabWidth"    : 4,
    "useTabs"     : false,
    "endOfLine"   : "lf",
    "singleQuote" : true,
    "semi"        : true,
    "printWidth"  : 80
}
""")

let editorconfig:unit = //
    File.WriteAllText (".editorconfig","""# fantomas config
indent_size = 4
max_line_length = 80
end_of_line = lf
insert_final_newline = true
""")

let gitattributes:unit = //
    File.WriteAllText (".gitattributes","""* text=auto eol=lf

# All source code in UNIX format
*.c   text diff=cpp
*.cpp text diff=cpp
*.h   text diff=cpp
*.hpp text diff=cpp
*.s   text diff=cpp
*.ld  text diff=cpp

# Binary files
*.bin  binary
*.elf  binary
*.dfu  binary
*.png  binary
*.pdf  binary
*.doc  binary
*.docx binary

# Linux
*.sh      text eol=lf
*.rc      text eol=lf
*.service text eol=lf

# Windows/MSYS
*.bat text eol=crlf
*.ps* text eol=crlf
""")

let format: unit = //
    clang_format
    prettierrc
    editorconfig
    gitattributes

let fs:unit = //
    mkdir "lib"
    for f in ["Evento";"Sestoft";"Parser";"AST"] do
        touch $"lib/{f}.fs"
    touch "Evento.fsproj"

let files :unit = //
    dirs
    mk
    cmake
    giti
    apt
    format
    fs

let package:unit = //
    touch $"src/{APP}.js"
    File.WriteAllText ("package.json",$"{{
    \"name\"        : \"{app}\",
    \"version\"     : \"{VERSION}\",
    \"description\" : \"{TITLE}\",
    \"main\"        : \"src/{app}.js\",
    \"directories\" : {{ \"doc\": \"doc\", \"src\": \"src\" }},
    \"scripts\": {{
        \"test\": \"echo \\\"Error: no test specified\\\" && exit 1\"
    }},
    \"author\": \"{AUTHOR} <{EMAIL}>\",
    \"license\": \"{LICENSE}\"
}}
")
    // npm -g deno

COMMIT
