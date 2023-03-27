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
        
        let _acceptCharset = attr "accept-charset"
        let _checked = attr "checked"
        let _class = attr "class"
        let _for = attr "for"
        let _httpEquiv = attr "http-equiv"
        let _id = attr "id"
        let _list = attr "list"
        let _open = attr "open"
        let _type = attr "type"
        let _abbr = attr "abbr"
        let _accept = attr "accept"        
        let _accesskey = attr "accesskey"
        let _action = attr "action"
        let _allow = attr "allow"
        let _allowfullscreen = attr "allowfullscreen"
        let _allowpaymentrequest = attr "allowpaymentrequest"
        let _alt = attr "alt"
        let _async = attr "async"
        let _autocomplete = attr "autocomplete"
        let _autofocus = attr "autofocus"
        let _autoplay = attr "autoplay"
        let _charset = attr "charset"        
        let _cite = attr "cite"
        let _cols = attr "cols"
        let _colspan = attr "colspan"
        let _content = attr "content"
        let _contenteditable = attr "contenteditable"
        let _controls = attr "controls"
        let _coords = attr "coords"
        let _crossorigin = attr "crossorigin"
        let _data = attr "data"
        let _datetime = attr "datetime"
        let _defer = attr "defer"
        let _dir = attr "dir"
        let _dirname = attr "dirname"
        let _disabled = attr "disabled"
        let _download = attr "download"
        let _draggable = attr "draggable"
        let _enctype = attr "enctype"
        let _form = attr "form"
        let _formaction = attr "formaction"
        let _formenctype = attr "formenctype"
        let _formmethod = attr "formmethod"
        let _formnovalidate = attr "formnovalidate"
        let _formtarget = attr "formtarget"
        let _headers = attr "headers"
        let _height = attr "height"
        let _hidden = attr "hidden"
        let _high = attr "high"
        let _href = attr "href"
        let _hreflang = attr "hreflang"        
        let _integrity = attr "integrity"
        let _ismap = attr "ismap"
        let _label = attr "label"
        let _lang = attr "lang"
        let _loading = attr "loading"
        let _longdesc = attr "longdesc"
        let _loop = attr "loop"
        let _low = attr "low"
        let _max = attr "max"
        let _maxlength = attr "maxlength"
        let _media = attr "media"
        let _method = attr "method"
        let _min = attr "min"
        let _minlength = attr "minlength"
        let _multiple = attr "multiple"
        let _muted = attr "muted"
        let _name = attr "name"
        let _nomodule = attr "nomodule"
        let _novalidate = attr "novalidate"
        let _optimum = attr "optimum"
        let _pattern = attr "pattern"
        let _ping = attr "ping"
        let _placeholder = attr "placeholder"
        let _preload = attr "preload"
        let _readonly = attr "readonly"
        let _referrerpolicy = attr "referrerpolicy"
        let _rel = attr "rel"
        let _required = attr "required"
        let _reversed = attr "reversed"
        let _rows = attr "rows"
        let _rowspan = attr "rowspan"
        let _sandbox = attr "sandbox"
        let _scope = attr "scope"
        let _selected = attr "selected"
        let _shape = attr "shape"
        let _size = attr "size"
        let _sizes = attr "sizes"
        let _span = attr "span"
        let _spellcheck = attr "spellcheck"
        let _src = attr "src"
        let _srcdoc = attr "srcdoc"
        let _srcset = attr "srcset"
        let _start = attr "start"
        let _step = attr "step"
        let _style = attr "style"
        let _tabindex = attr "tabindex"
        let _target = attr "target"
        let _title = attr "title"
        let _translate = attr "translate"
        let _typemustmatch = attr "typemustmatch"
        let _usemap = attr "usemap"
        let _value = attr "value"
        let _width = attr "width"
        let _wrap = attr "wrap"
        let _xmlns = attr "xmlns"

    [<AutoOpen>]
    module Events =
        let _onabort = attr "onabort"
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


    

    
    