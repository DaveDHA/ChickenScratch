namespace ChickenScratch


module ScratchNodeBuilder =
    type ScratchNodeBuilderNode =
    | Attribute of string * string
    | Child of ScratchNode

    module ScratchNodeBuilderNode =
        let MergeAttributes items =
            items
            |> Seq.choose (function | Attribute (k,v) -> Some (k,v) | _ -> None)
            |> Seq.groupBy fst
            |> Seq.map (fun (key, items) -> (key, items |> Seq.map snd |> Seq.distinct |> String.concat " "))
            |> Map.ofSeq

        let CollectChildren items = items |> List.choose (function | Child t -> Some t | _ -> None)



    type ScratchNodeBuilder(tag, allowChildren) = 
        member _.Zero() = []

        member _.Yield (x : ScratchNodeBuilderNode) = [ x ]

        member _.YieldFrom (x : ScratchNodeBuilderNode seq) = x |> List.ofSeq

        member this.Yield (x : ScratchElement) = x |> Element |> Child |> this.Yield

        member this.YieldFrom (x : ScratchElement seq) = x |> Seq.map (Element >> Child) |> this.YieldFrom

        member this.Yield (x : ScratchNode) = Child x |> this.Yield

        member this.YieldFrom (x : ScratchNode seq) = x |> Seq.map Child |> this.YieldFrom

        member this.Yield (x : string) = x |> Content |> this.Yield

        member this.YieldFrom (x : string seq) = x |> Seq.map Content |> this.YieldFrom

        member _.Combine((x : ScratchNodeBuilderNode list), y) = x @ y

        member _.Delay f = f()

        member _.For (items, f) = items |> Seq.collect f |> Seq.toList

        member _.Run (items : ScratchNodeBuilderNode list) =
            {
                Tag = tag
                Attributes = items |> ScratchNodeBuilderNode.MergeAttributes
                Children = if allowChildren then items |> ScratchNodeBuilderNode.CollectChildren else List.empty
                AllowChildren = allowChildren
            }
            |> Element


    let scratchNode t allowChildren = ScratchNodeBuilder(t, allowChildren)


module ScratchTagExpressions =
    let raw str = Raw str

    [<AutoOpen>]
    module Tags = 
        let tag t = ScratchNodeBuilder.scratchNode t true
        let selfClosingTag t = ScratchNodeBuilder.scratchNode t false

        let ``base`` = selfClosingTag "base"
        let a = tag "a"
        let abbr = tag "abbr"
        let address = tag "address"
        let area = selfClosingTag "area"
        let article = tag "article"
        let aside = tag "aside"
        let audio = tag "audio"
        let b = tag "b"
        let bdi = tag "bdi"
        let bdo = tag "bdo"
        let blockquote = tag "blockquote"
        let body = tag "body"
        let br = selfClosingTag "br"
        let button = tag "button"
        let canvas = tag "canvas"
        let caption = tag "caption"
        let cite = tag "cite"
        let code = tag "code"
        let col = selfClosingTag "col"
        let colgroup = tag "colgroup"
        let data = tag "data"
        let datalist = tag "datalist"
        let dd = tag "dd"
        let del = tag "del"
        let details = tag "details"
        let dfn = tag "dfn"
        let dialog = tag "dialog"
        let div = tag "div"
        let dl = tag "dl"
        let dt = tag "dt"
        let em = tag "em"
        let embed = selfClosingTag "embed"
        let fieldset = tag "fieldset"
        let figcaption = tag "figcaption"
        let figure = tag "figure"
        let footer = tag "footer"
        let form = tag "form"
        let h1 = tag "h1"
        let h2 = tag "h2"
        let h3 = tag "h3"
        let h4 = tag "h4"
        let h5 = tag "h5"
        let h6 = tag "h6"
        let head = tag "head"
        let header = tag "header"
        let hr = selfClosingTag "hr"
        let html = tag "html"
        let i = tag "i"
        let iframe = tag "iframe"
        let img = selfClosingTag "img"
        let input = selfClosingTag "input"
        let ins = tag "ins"
        let kbd = tag "kbd"
        let label = tag "label"
        let legend = tag "legend"
        let li = tag "li"
        let link = selfClosingTag "link"
        let main = tag "main"
        let map = tag "map"
        let mark = tag "mark"
        let meta = selfClosingTag "meta"
        let meter = tag "meter"
        let nav = tag "nav"
        let noscript = tag "noscript"
        let object = tag "object"
        let ol = tag "ol"
        let optgroup = tag "optgroup"
        let option = tag "option"
        let output = tag "output"
        let p = tag "p"
        let param = selfClosingTag "param"
        let picture = tag "picture"
        let pre = tag "pre"
        let progress = tag "progress"
        let q = tag "q"
        let rp = tag "rp"
        let rt = tag "rt"
        let ruby = tag "ruby"
        let s = tag "s"
        let samp = tag "samp"
        let script = tag "script"
        let section = tag "section"
        let select = tag "select"
        let small = tag "small"
        let source = selfClosingTag "source"
        let span = tag "span"
        let strong = tag "strong"
        let style = tag "style"
        let sub = tag "sub"
        let summary = tag "summary"
        let sup = tag "sup"
        let svg = tag "svg"
        let table = tag "table"
        let tbody = tag "tbody"
        let td = tag "td"
        let template = tag "template"
        let textarea = tag "textarea"
        let tfoot = tag "tfoot"
        let th = tag "th"
        let thead = tag "thead"
        let time = tag "time"
        let title = tag "title"
        let tr = tag "tr"
        let track = selfClosingTag "track"
        let u = tag "u"
        let ul = tag "ul"
        let var = tag "var"
        let video = tag "video"
        let wbr = selfClosingTag "wbr" 


    [<AutoOpen>]
    module Attributes = 
        let attr k v = ScratchNodeBuilder.Attribute (k,v)
        
        let ``accept-charset`` = attr "accept-charset"
        let ``checked`` = attr "checked"
        let ``class`` = attr "``class``"
        let ``for`` = attr "``for``"
        let ``http-equiv`` = attr "http-equiv"
        let ``id`` = attr "``id``"
        let ``list`` = attr "``list``"
        let ``open`` = attr "``open``"
        let ``type`` = attr "``type``"
        let abbr = attr "abbr"
        let accept = attr "accept"        
        let accesskey = attr "accesskey"
        let action = attr "action"
        let allow = attr "allow"
        let allowfullscreen = attr "allowfullscreen"
        let allowpaymentrequest = attr "allowpaymentrequest"
        let alt = attr "alt"
        let async = attr "async"
        let autocomplete = attr "autocomplete"
        let autofocus = attr "autofocus"
        let autoplay = attr "autoplay"
        let charset = attr "charset"        
        let cite = attr "cite"
        let cols = attr "cols"
        let colspan = attr "colspan"
        let content = attr "content"
        let contenteditable = attr "contenteditable"
        let controls = attr "controls"
        let coords = attr "coords"
        let crossorigin = attr "crossorigin"
        let data = attr "data"
        let datetime = attr "datetime"
        let defer = attr "defer"
        let dir = attr "dir"
        let dirname = attr "dirname"
        let disabled = attr "disabled"
        let download = attr "download"
        let draggable = attr "draggable"
        let enctype = attr "enctype"
        let form = attr "form"
        let formaction = attr "formaction"
        let formenctype = attr "formenctype"
        let formmethod = attr "formmethod"
        let formnovalidate = attr "formnovalidate"
        let formtarget = attr "formtarget"
        let headers = attr "headers"
        let height = attr "height"
        let hidden = attr "hidden"
        let high = attr "high"
        let href = attr "href"
        let hreflang = attr "hreflang"        
        let integrity = attr "integrity"
        let ismap = attr "ismap"
        let label = attr "label"
        let lang = attr "lang"
        let loading = attr "loading"
        let longdesc = attr "longdesc"
        let loop = attr "loop"
        let low = attr "low"
        let max = attr "max"
        let maxlength = attr "maxlength"
        let media = attr "media"
        let method = attr "method"
        let min = attr "min"
        let minlength = attr "minlength"
        let multiple = attr "multiple"
        let muted = attr "muted"
        let name = attr "name"
        let nomodule = attr "nomodule"
        let novalidate = attr "novalidate"
        let optimum = attr "optimum"
        let pattern = attr "pattern"
        let ping = attr "ping"
        let placeholder = attr "placeholder"
        let preload = attr "preload"
        let readonly = attr "readonly"
        let referrerpolicy = attr "referrerpolicy"
        let rel = attr "rel"
        let required = attr "required"
        let reversed = attr "reversed"
        let rows = attr "rows"
        let rowspan = attr "rowspan"
        let sandbox = attr "sandbox"
        let scope = attr "scope"
        let selected = attr "selected"
        let shape = attr "shape"
        let size = attr "size"
        let sizes = attr "sizes"
        let span = attr "span"
        let spellcheck = attr "spellcheck"
        let src = attr "src"
        let srcdoc = attr "srcdoc"
        let srcset = attr "srcset"
        let start = attr "start"
        let step = attr "step"
        let style = attr "style"
        let tabindex = attr "tabindex"
        let target = attr "target"
        let title = attr "title"
        let translate = attr "translate"
        let typemustmatch = attr "typemustmatch"
        let usemap = attr "usemap"
        let value = attr "value"
        let width = attr "width"
        let wrap = attr "wrap"
        let xmlns = attr "xmlns"

    [<AutoOpen>]
    module Events =
        let onabort = attr "onabort"
        let onblur = attr "onblur"
        let oncanplay = attr "oncanplay"
        let oncanplaythrough = attr "oncanplaythrough"
        let onchange = attr "onchange"
        let onclick = attr "onclick"
        let oncontextmenu = attr "oncontextmenu"
        let oncopy = attr "oncopy"
        let oncuechange = attr "oncuechange"
        let oncut = attr "oncut"
        let ondblclick = attr "ondblclick"
        let ondrag = attr "ondrag"
        let ondragend = attr "ondragend"
        let ondragenter = attr "ondragenter"
        let ondragleave = attr "ondragleave"
        let ondragover = attr "ondragover"
        let ondragstart = attr "ondragstart"
        let ondrop = attr "ondrop"
        let ondurationchange = attr "ondurationchange"
        let onemptied = attr "onemptied"
        let onended = attr "onended"
        let onerror = attr "onerror"
        let onfocus = attr "onfocus"
        let oninput = attr "oninput"
        let oninvalid = attr "oninvalid"
        let onkeydown = attr "onkeydown"
        let onkeypress = attr "onkeypress"
        let onkeyup = attr "onkeyup"
        let onloadeddata = attr "onloadeddata"
        let onloadedmetadata = attr "onloadedmetadata"
        let onloadstart = attr "onloadstart"
        let onmousedown = attr "onmousedown"
        let onmousemove = attr "onmousemove"
        let onmouseout = attr "onmouseout"
        let onmouseover = attr "onmouseover"
        let onmouseup = attr "onmouseup"
        let onmousewheel = attr "onmousewheel"
        let onpaste = attr "onpaste"
        let onpause = attr "onpause"
        let onplay = attr "onplay"
        let onplaying = attr "onplaying"
        let onprogress = attr "onprogress"
        let onratechange = attr "onratechange"
        let onreset = attr "onreset"
        let onscroll = attr "onscroll"
        let onsearch = attr "onsearch"
        let onseeked = attr "onseeked"
        let onseeking = attr "onseeking"
        let onselect = attr "onselect"
        let onstalled = attr "onstalled"
        let onsubmit = attr "onsubmit"
        let onsuspend = attr "onsuspend"
        let ontimeupdate = attr "ontimeupdate"
        let ontoggle = attr "ontoggle"
        let onvolumechange = attr "onvolumechange"
        let onwaiting = attr "onwaiting"
        let onwheel = attr "onwheel"


    

    
    