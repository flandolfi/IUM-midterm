// Francesco Landolfi
// Matr. 444151
//
// Sviluppato con:
//  - Linux Mint 16 (Ubuntu 13.10)
//  - MonoDevelop 4.2.2
//  - Mono 3.2.3
//  - F# 3.1 (Open Source Edition)

open System.Windows.Forms
open System.Drawing



type ButtonType =
    | Clear
    | AddState
    | AddTransition
    | Initial
    | Delete
    | UpArrow
    | RightArrow
    | DownArrow
    | LeftArrow
    | ZoomIn
    | ZoomOut
    | Reload
    | Play
    | Pause
    | Stop
    | Slower
    | Faster



type Status =
    | Disabled
    | Clicked
    | Normal



type Button (bt:ButtonType) =
    let mutable area = new RectangleF()
    let trs = new ResizeArray<Drawing2D.GraphicsPath>()
    let btColor = Brushes.White
    let btColorDisabled = Brushes.Gray
    let bgColorDark = Brushes.Black
    let bgColor = Brushes.Gray
    let bgColorLight = Brushes.DarkGray
    let mutable hasFocus = false
    let mutable bgC = bgColorDark
    let mutable btC = btColor
    let mutable (status:Status) = Normal
    let mutable descr = ""
    let mutable customFun = (fun args -> ())
    let initPolygons side =
        trs.Clear()
        match bt with
            | Play ->
                let gp = new Drawing2D.GraphicsPath()
                gp.AddPolygon([| new PointF(side*0.2f, side*0.2f); new PointF(side*0.8f, side*0.5f); new PointF(side*0.2f, side*0.8f); |])
                trs.Add(gp)
            | Faster ->
                let gp1 = new Drawing2D.GraphicsPath()
                let gp2 = new Drawing2D.GraphicsPath()
                gp1.AddPolygon([| new PointF(side*0.2f, side*0.2f); new PointF(side*0.5f, side*0.5f); new PointF(side*0.2f, side*0.8f); |])
                gp2.AddPolygon([| new PointF(side*0.5f, side*0.2f); new PointF(side*0.8f, side*0.5f); new PointF(side*0.5f, side*0.8f); |])
                trs.Add(gp1)
                trs.Add(gp2)
            | Slower ->
                let gp1 = new Drawing2D.GraphicsPath()
                let gp2 = new Drawing2D.GraphicsPath()
                gp1.AddPolygon([| new PointF(side*0.8f, side*0.2f); new PointF(side*0.5f, side*0.5f); new PointF(side*0.8f, side*0.8f); |])
                gp2.AddPolygon([| new PointF(side*0.5f, side*0.2f); new PointF(side*0.2f, side*0.5f); new PointF(side*0.5f, side*0.8f); |])
                trs.Add(gp1)
                trs.Add(gp2)
            | Clear ->
                let gp = new Drawing2D.GraphicsPath()
                gp.AddPolygon([| new PointF(side*0.4f, 0.f); new PointF(side, 0.f); new PointF(side, side*0.6f); |])
                trs.Add(gp)
            | LeftArrow ->
                let gp1 = new Drawing2D.GraphicsPath()
                let gp2 = new Drawing2D.GraphicsPath()
                gp1.AddPolygon([| new PointF(side*0.5f, side*0.5f); new PointF(0.f, side*0.97f); new PointF() |])
                gp2.AddPolygon([| new PointF(side*0.1f, side*0.5f); new PointF(side*0.3f, side*0.4f); new PointF(side*0.3f, side*0.6f); |])
                trs.Add(gp1)
                trs.Add(gp2)
            | RightArrow ->
                let gp1 = new Drawing2D.GraphicsPath()
                let gp2 = new Drawing2D.GraphicsPath()
                gp1.AddPolygon([| new PointF(side*0.5f, side*0.5f); new PointF(side*0.97f, 0.f); new PointF(side*0.97f, side*0.97f) |]) 
                gp2.AddPolygon([| new PointF(side*0.9f, side*0.5f); new PointF(side*0.7f, side*0.6f); new PointF(side*0.7f, side*0.4f); |])
                trs.Add(gp1)
                trs.Add(gp2)
            | UpArrow ->
                let gp1 = new Drawing2D.GraphicsPath()
                let gp2 = new Drawing2D.GraphicsPath()
                gp1.AddPolygon([| new PointF(side*0.5f, side*0.5f); new PointF(); new PointF(side, 0.f) |]) 
                gp2.AddPolygon([| new PointF(side*0.5f, side*0.1f); new PointF(side*0.6f, side*0.3f); new PointF(side*0.4f, side*0.3f); |])
                trs.Add(gp1)
                trs.Add(gp2)
            | DownArrow ->
                let gp1 = new Drawing2D.GraphicsPath()
                let gp2 = new Drawing2D.GraphicsPath()
                gp1.AddPolygon([| new PointF(side*0.5f, side*0.5f); new PointF(side*0.97f, side*0.97f); new PointF(0.f, side*0.97f) |])
                gp2.AddPolygon([| new PointF(side*0.5f, side*0.9f); new PointF(side*0.4f, side*0.7f); new PointF(side*0.6f, side*0.7f); |])
                trs.Add(gp1)
                trs.Add(gp2)
            | Reload ->
                let t1 = new Drawing2D.GraphicsPath()
                let t2 = new Drawing2D.GraphicsPath()
                let d = side*(0.5f + single(System.Math.Sqrt(0.08)))
                t1.AddPolygon([| new PointF(side*0.5f, side*0.5f); new PointF(side*0.97f, side*0.97f); new PointF(0.f, side*0.97f) |])
                t2.AddPolygon([| new PointF(side*0.6f, side*0.6f); new PointF(side*0.6f, d); new PointF(d, d) |])
                trs.Add(t1)
                trs.Add(t2)
            | ZoomIn | ZoomOut ->
                let rect = new Drawing2D.GraphicsPath()
                let mat = new Drawing2D.Matrix()
                mat.Rotate(-45.f)
                mat.Translate(side*0.4f, side*0.5f, Drawing2D.MatrixOrder.Append)
                rect.AddRectangle(new RectangleF(0.f, 0.f, side*0.12f, side*0.4f))
                rect.Transform(mat)
                trs.Add(rect)
            | AddTransition ->
                let t = new Drawing2D.GraphicsPath()
                t.AddPolygon([| new PointF(side*0.67f, side*0.67f); new PointF(side*0.67f, side*0.74f); new PointF(side*0.74f, side*0.74f) |])
                trs.Add(t)
            | _ -> ()
    
    do
        match bt with
            | Initial ->
                descr <- "Mark selected state as Initial State"
            | AddTransition ->
                descr <- "Add a new Transition to the current Diagram"
            | Delete ->
                descr <- "Delete selected Transition or State (all its transitions will be lost)"
            | Reload ->
                descr <- "Change the Input to the Machine"
            | Play ->
                descr <- "Start Animation"
            | Pause ->
                descr <- "Pause Animation"
            | Stop ->
                descr <- "Stop Animation"
            | Faster ->
                descr <- "Accelerate Animation"
            | Slower ->
                descr <- "Decelerate Animation"
            | ZoomIn ->
                descr <- "Zoom In"
            | ZoomOut ->
                descr <- "Zoom Out"
            | AddState ->
                descr <- "Add a new State to the current Diagram"
            | Clear ->
                descr <- "New Finite-State Machine (FSM) Diagram (Clear current Diagram)"
            | LeftArrow ->
                descr <- "Scroll Left"
            | RightArrow ->
                descr <- "Scroll Right"
            | UpArrow ->
                descr <- "Scroll Up"
            | DownArrow ->
                descr <- "Scroll Down"
    
    member this.Type = bt
    
    member this.HasFocus
        with get() = hasFocus
        and set(b) = hasFocus <- b
    
    member this.Description
        with get() = descr
        and set(s) = descr <- s
    
    member this.Status
        with get() = status
        and set(s) =
            match s with
                | Disabled ->
                    btC <- btColorDisabled
                    bgC <- bgColorDark
                | Clicked ->
                    bgC <- bgColorLight
                    btC <- btColor
                | Normal ->
                    bgC <- bgColorDark
                    btC <- btColor
            status <- s
    
    member this.CustomFun
        with get() = customFun
        and set(f) = customFun <- f

    member this.Location
        with get() = area.Location
        and set(l) = area.Location <- l
    
    member this.Side 
        with get() = area.Width
        and set(s) =
            area.Width <- s
            area.Height <- s
            initPolygons s
    
    member this.Contains(p:PointF) =
        match bt with
            | LeftArrow | RightArrow | UpArrow | DownArrow -> 
                let pTemp = new PointF(p.X - area.X, p.Y - area.Y)
                hasFocus <- trs.[0].IsVisible(pTemp)
            | _ ->
                hasFocus <- area.Contains(p)
        if hasFocus && status = Normal then
            bgC <- bgColor
        else
            match status with
                | Disabled ->
                    bgC <- bgColorDark
                | Clicked ->
                    bgC <- bgColorLight
                | Normal ->
                    bgC <- bgColorDark
        hasFocus
    
    member this.Paint(g:Graphics) =
        if ((bt <> LeftArrow) && (bt <> RightArrow) && (bt <> UpArrow) && (bt <> DownArrow)) then
            g.FillRectangle(bgC, area)
        g.TranslateTransform(area.X, area.Y)
        match bt with
            | LeftArrow | RightArrow | UpArrow | DownArrow -> 
                g.FillPath(bgC, trs.[0])
                g.FillPath(btC, trs.[1])
            | Play ->
                g.FillPath(btC, trs.[0])
            | Pause ->
                g.FillRectangle(btC, area.Width*0.2f, area.Width*0.2f, area.Width*0.2f, area.Width*0.6f)
                g.FillRectangle(btC, area.Width*0.6f, area.Width*0.2f, area.Width*0.2f, area.Width*0.6f)
            | Stop ->
                g.FillRectangle(btC, area.Width*0.2f, area.Width*0.2f, area.Width*0.6f, area.Width*0.6f)
            | Faster | Slower ->
                g.FillPath(btC, trs.[0])
                g.FillPath(btC, trs.[1])
            | AddState ->
                use p = new Pen(btC, area.Width*0.05f)
                g.DrawEllipse(p, area.Width*0.2f, area.Width*0.2f, area.Width*0.6f, area.Width*0.6f)
                g.DrawLine(p, area.Width*0.2f, area.Width*0.5f, area.Width*0.8f, area.Width*0.5f)
                g.DrawString("Sn", new Font("Courier", area.Width*0.15f, FontStyle.Bold), btC, new PointF(area.Width*0.35f, area.Width*0.25f))
                g.DrawString("010", new Font("Courier", area.Width*0.14f), btC, new PointF(area.Width*0.34f, area.Width*0.54f))
            | Initial ->
                use p = new Pen(btC, area.Width*0.05f)
                g.DrawEllipse(p, area.Width*0.2f, area.Width*0.2f, area.Width*0.6f, area.Width*0.6f)
                g.DrawLine(p, area.Width*0.2f, area.Width*0.5f, area.Width*0.8f, area.Width*0.5f)
                g.DrawEllipse(p, area.Width*0.3f, area.Width*0.3f, area.Width*0.4f, area.Width*0.4f)
            | AddTransition -> 
                use p = new Pen(btC, area.Width*0.05f)
                p.StartCap <- Drawing2D.LineCap.ArrowAnchor
                g.DrawArc(p, area.Width*0.2f, area.Width*0.2f, area.Width*0.6f, area.Width*0.6f, 45.f, -270.f)
                g.DrawString("010", new Font("Courier", area.Width*0.15f, FontStyle.Bold), btC, new PointF(area.Width*0.3f, area.Width*0.4f))
                g.FillPath(btC, trs.[0])
            | Delete ->
                use p = new Pen(btC, area.Width*0.05f)
                g.DrawLine(p, area.Width*0.2f, area.Width*0.2f, area.Width*0.8f, area.Width*0.8f)
                g.DrawLine(p, area.Width*0.2f, area.Width*0.8f, area.Width*0.8f, area.Width*0.2f)
            | Clear ->
                g.FillRectangle(btC, area.Width*0.2f, area.Width*0.2f, area.Width*0.6f, area.Width*0.6f)
                g.FillPath(bgC, trs.[0])
                g.DrawRectangle(new Pen(bgC), area.Width*0.6f, 0.f, area.Width*0.4f, area.Width*0.4f)
            | ZoomIn | ZoomOut ->
                g.FillPath(btC, trs.[0])
                g.FillEllipse(btC, area.Width*0.1f, area.Width*0.1f, area.Width*0.5f, area.Width*0.5f)
                g.FillEllipse(bgC, area.Width*0.2f, area.Width*0.2f, area.Width*0.3f, area.Width*0.3f)
                g.FillRectangle(btC, area.Width*0.7f, area.Width*0.25f, area.Width*0.2f, area.Width*0.1f)
                if bt = ZoomIn then
                    g.FillRectangle(btC, area.Width*0.75f, area.Width*0.2f, area.Width*0.1f, area.Width*0.2f)
            | Reload ->
                g.DrawEllipse(new Pen(btC, area.Width*0.1f), area.Width*0.2f, area.Width*0.2f, area.Width*0.6f, area.Width*0.6f)
                g.FillPath(bgC, trs.[0])
                g.FillPath(btC, trs.[1])
        g.TranslateTransform(-area.X, -area.Y)



type Toolbar() as this =
    let mutable area = new RectangleF()
    let radRatio = 0.2f
    let mutable side = 40.f
    let bgC = new SolidBrush(Color.Black)
    let bts = new ResizeArray<Button>()
    let textBox = new TextBox()
    let mutable index = -1
    let statuses = new ResizeArray<Status>()
    let mutable textBoxStatus = false
    
    do
        bts.Add(new Button(Clear, Status = Disabled))
        bts.Add(new Button(AddState))
        bts.Add(new Button(AddTransition, Status = Disabled))
        bts.Add(new Button(Initial, Status = Disabled))
        bts.Add(new Button(Delete, Status = Disabled))
        bts.Add(new Button(UpArrow))
        bts.Add(new Button(RightArrow))
        bts.Add(new Button(DownArrow))
        bts.Add(new Button(LeftArrow))
        bts.Add(new Button(ZoomIn))
        bts.Add(new Button(ZoomOut))
        bts.Add(new Button(Reload, Status = Disabled))
        bts.Add(new Button(Play, Status = Disabled))
        bts.Add(new Button(Pause, Status = Disabled))
        bts.Add(new Button(Stop, Status = Disabled))
        bts.Add(new Button(Slower, Status = Disabled))
        bts.Add(new Button(Faster, Status = Disabled))
        this.Side <- side
        textBox.BorderStyle <- BorderStyle.FixedSingle
        textBox.TextAlign <- HorizontalAlignment.Right
        textBox.Enabled <- false

    member this.Buttons = bts

    member this.TextBox = textBox
    
    member this.Index = index
    
    member this.DisableAll() =
        for b in bts do
            match b.Type with
                | UpArrow | RightArrow | DownArrow | LeftArrow | ZoomIn | ZoomOut -> ()
                | _ -> b.Status <- Disabled
        textBox.Enabled <- false
    
    member this.Save() =
        statuses.Clear()
        for b in bts do
            statuses.Add(b.Status)
        textBoxStatus <- textBox.Enabled

    member this.Restore() =
        textBox.Enabled <- textBoxStatus
        statuses |> Seq.iteri(fun i s -> bts.[i].Status <- s)
        statuses.Clear()

    member this.Contains(p:PointF) =
        let pTemp = new PointF(p.X - area.X, p.Y - area.Y)
        let mutable ret = false
        let mutable i = 0
        index <- -1
        for b in bts do
            let bool = b.Contains(pTemp)
            if bool then
                index <- i
            ret <- bool || ret
            i <- i + 1
        ret
    
    member this.Location
        with get() = area.Location
        and set(p) = 
            area.Location <- p
            textBox.Location <- new Point(int(side*10.3f + p.X), int(side*0.27f + p.Y))
    
    member this.Size
        with get() = area.Size

    member this.Side
        with get() = side
        and set(s) =
            side <- s
            textBox.Size <- new Size(int(side*2.3f), textBox.Size.Height)
            area.Size <- new SizeF(19.f*s, s)
            let mutable i = 0.f
            let mutable count = 0.f
            let mutable gap = 0.f
            for b in bts do
                b.Side <- s
                match b.Type with
                    | UpArrow | RightArrow | DownArrow | LeftArrow ->
                        b.Location <- new PointF(s*5.f + s/3.f, 0.f)
                        count <- 3.f
                        gap <- s/3.f
                    | ZoomOut ->
                        b.Location <- new PointF(s*(i - count) + gap, 0.f)
                        count <- -1.f
                        gap <- s*2.f/3.f
                    | Reload ->
                        b.Location <- new PointF(s*(i - count) + gap, 0.f)
                        gap <- s
                    | _ ->
                        b.Location <- new PointF(s*(i - count) + gap, 0.f)
                i <- i + 1.f

    member this.Paint(g:Graphics) =
        g.FillRectangle(bgC, area)
        g.TranslateTransform(area.X, area.Y)
        g.FillEllipse(bgC, -side*radRatio, 0.f, side*radRatio*2.f, side*radRatio*2.f)
        g.FillEllipse(bgC, -side*radRatio, side - side*radRatio*2.f, side*radRatio*2.f, side*radRatio*2.f)
        g.FillEllipse(bgC, area.Width - side*radRatio, 0.f, side*radRatio*2.f, side*radRatio*2.f)
        g.FillEllipse(bgC, area.Width - side*radRatio, side - side*radRatio*2.f, side*radRatio*2.f, side*radRatio*2.f)
        g.FillRectangle(bgC, -side*radRatio, side*radRatio, side*radRatio, side*(1.f - 2.f*radRatio))
        g.FillRectangle(bgC, area.Width, side*radRatio, side*radRatio, side*(1.f - 2.f*radRatio))
        for b in bts do
            b.Paint(g)
        g.DrawLine(Pens.DarkGray, side*31.f/6.f, side/6.f, side*31.f/6.f, side*5.f/6.f)
        g.DrawLine(Pens.DarkGray, side*51.f/6.f, side/6.f, side*51.f/6.f, side*5.f/6.f)
        g.DrawLine(Pens.DarkGray, side*83.f/6.f, side/6.f, side*83.f/6.f, side*5.f/6.f)
        g.DrawString("INPUT:", new Font("Verdana", side*0.3f), Brushes.White, side*8.7f, side/4.5f)
        if ((index <> -1) && (bts.[index].Status <> Disabled)) then
            g.DrawString(bts.[index].Description, new Font("Verdana", side*0.2f), Brushes.Black, 0.f, side*1.1f)
        g.TranslateTransform(-area.X, -area.Y)



type State() =
    let mutable radius = 30.f
    let mutable center = new PointF(400.f, 150.f)
    let mutable area = new RectangleF(center.X - radius, center.Y - radius, radius*2.f, radius*2.f)
    let mutable name = ""
    let mutable entryAction = ""
    let bgC = Brushes.White
    let lineUnselected = Pens.Black
    let lineSelected = Pens.Red
    let mutable lnC = lineUnselected
    let mutable selected = false
    let mutable initial = false
    let fontS = new Font("Courier", radius*0.3f, FontStyle.Bold)
    let fontEA = new Font("Courier", radius*0.3f)
    let transitions = new ResizeArray<Transition>()
        
    member this.Transitions = transitions
    
    member this.AddTransition(dest:State, cond:string) =
        if transitions |> Seq.exists(fun t -> t.Destination = dest) then
            let (t:Transition) = transitions |> Seq.find(fun t -> t.Destination = dest)
            if not (t.Conditions |> Seq.exists(fun c -> c = cond)) then
                t.Conditions.Add(cond)
        else
            transitions.Add(new Transition(this, Destination = dest, ConditionsString = cond))
    
    member this.Initial
        with get() = initial
        and set(b) = initial <- b
    
    member this.Contains(p:PointF) =
        ((radius*radius) >= ((p.X - center.X)*(p.X - center.X) + (p.Y - center.Y)*(p.Y - center.Y)))

    member this.Selected
        with get() = selected
        and set(b) =
            selected <- b
            if b then
                lnC <- lineSelected
            else
                lnC <- lineUnselected
    
    member this.Radius
        with get() = radius
        and set(r) =
            radius <- r
            area.Size <- new SizeF(radius*2.f, radius*2.f)
            area.Location <- new PointF(center.X - radius, center.Y - radius)
    
    member this.Location
        with get() = center
        and set(p) =
            center <- p
            area.Location <- new PointF(p.X - radius, p.Y - radius)
    
    member this.Name
        with get() = name
        and set(s) = name <- s
    
    member this.EntryAction
        with get() = entryAction
        and set(s) = entryAction <- s
    
    member this.Paint(g:Graphics) =
        let angle = single(45.*System.Math.PI/180.)
        let rName = new RectangleF(center.X - radius*cos(angle), center.Y - radius*sin(angle), radius*2.f*cos(angle), radius*sin(angle))
        let rEntry = new RectangleF(center.X - radius*cos(angle), center.Y, radius*2.f*cos(angle), radius*sin(angle))
        g.FillEllipse(bgC, area)
        g.DrawEllipse(lnC, area)
        if initial then
            let p = 0.1f
            g.DrawEllipse(lnC, new RectangleF(area.X + radius*p, area.Y + radius*p, radius*(2.f - p*2.f), radius*(2.f - p*2.f)))
        g.DrawLine(lnC, center.X - radius, center.Y, center.X + radius, center.Y)
        g.DrawString(name, fontS, Brushes.Black, rName,
            new StringFormat(Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center))
        g.DrawString(entryAction, fontEA, Brushes.Black, rEntry,
            new StringFormat(Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center))



and Transition(s:State) =
    let mutable dest = new State()
    let conditions = new ResizeArray<string>()
    let lnSelected = Brushes.Red
    let lnUnselected = Brushes.Gray
    let mutable lnC = new Pen(lnUnselected, 2.f)
    let mutable bgC = lnUnselected
    let lnWidth = s.Radius*0.3f
    let font = new Font("Courier", lnWidth, FontStyle.Bold)
    let mutable selected = false
    let mutable radius = 0.f
    let mutable arcAngle = 0.f
    let mutable rtAngle = 0.f
    let mutable isLine = true
    let mutable rect = new RectangleF()
    let w2v = new Drawing2D.Matrix()
    let v2w = new Drawing2D.Matrix()
    let dist() = sqrt((s.Location.X - dest.Location.X)*(s.Location.X - dest.Location.X) + 
                      (s.Location.Y - dest.Location.Y)*(s.Location.Y - dest.Location.Y))
    let chordAngle(c) = 2.f*asin(0.5f*c/radius)*180.f/single(System.Math.PI)
    let regMax = new Drawing2D.GraphicsPath()
    let regMin = new Drawing2D.GraphicsPath()
    let arrow() =
        let dim = 0.3f
        if isLine then
            let cosAlpha = (s.Location.X - dest.Location.X)/dist()
            let sinAlpha = (s.Location.Y - dest.Location.Y)/dist()
            [| new PointF(dest.Location.X + s.Radius*cosAlpha, dest.Location.Y + s.Radius*sinAlpha);
               new PointF(dest.Location.X + s.Radius*((dim + 1.f)*cosAlpha + 0.5f*dim*sinAlpha), dest.Location.Y + s.Radius*((dim + 1.f)*sinAlpha - 0.5f*dim*cosAlpha));
               new PointF(dest.Location.X + s.Radius*((dim + 1.f)*cosAlpha - 0.5f*dim*sinAlpha), dest.Location.Y + s.Radius*((dim + 1.f)*sinAlpha + 0.5f*dim*cosAlpha)); |]
        else
            let t = (1.f - chordAngle(s.Radius)/arcAngle)
            let t2 = (1.f - chordAngle(s.Radius*(dim + 1.f)/arcAngle))
            let alpha = (-90.f - arcAngle*0.5f + t*arcAngle)*single(System.Math.PI)/180.f
            let alpha2 = (-90.f - arcAngle*0.5f + t2*arcAngle)*single(System.Math.PI)/180.f
            let pt = [| new PointF(rect.X + rect.Width*0.5f + radius*cos(alpha), rect.Y + rect.Height*0.5f + radius*sin(alpha));
                        new PointF(rect.X + rect.Width*0.5f + (radius + 0.5f*dim*s.Radius)*cos(alpha2),
                                   rect.Y + rect.Height*0.5f + (radius + 0.5f*s.Radius*dim)*sin(alpha2));
                        new PointF(rect.X + rect.Width*0.5f + (radius - 0.5f*dim*s.Radius)*cos(alpha2), 
                                   rect.Y + rect.Height*0.5f + (radius - 0.5f*s.Radius*dim)*sin(alpha2)); |]
            w2v.TransformPoints(pt)
            pt
    
    member this.Selected
        with get() = selected
        and set(b) = 
            selected <- b
            if b then
                lnC <- new Pen(lnSelected, 2.f)
                bgC <- lnSelected
            else
                lnC <- new Pen(lnUnselected, 2.f)
                bgC <- lnUnselected
    
    member this.Conditions : ResizeArray<string> = conditions
    
    member this.ConditionsString
        with get() = "\"" + (String.concat "\", \"" conditions) + "\""
        and set(s:string) = conditions.Add(s)
    
    member this.Destination
        with get() = dest
        and set(d) = dest <- d
    
    member this.Source
        with get() = s
    
    member this.Contains(p:PointF) =
        if isLine then
            let dist = dist()
            if dist = 0.f then 
                rtAngle <- 0.f
            else 
                let h = acos((dest.Location.X - s.Location.X)/dist)*180.f/single(System.Math.PI)
                if s.Location.Y > dest.Location.Y then
                    rtAngle <- -h
                else
                    rtAngle <- h
            w2v.Reset()
            w2v.Translate(s.Location.X, s.Location.Y)
            w2v.Rotate(rtAngle)
            regMax.Reset()
            regMax.AddRectangle(new RectangleF(0.f, -lnWidth*0.5f, dist, lnWidth))
            regMax.Transform(w2v)
            regMax.IsVisible(p)
        else
            regMax.Transform(w2v)
            regMin.Transform(w2v)
            regMax.IsVisible(p) && not(regMin.IsVisible(p))
    
    member this.PointAt(t) =
        if isLine then
            let d = dist()
            let cosAlpha = (dest.Location.X - s.Location.X)/d
            let sinAlpha = (dest.Location.Y - s.Location.Y)/d
            let p = new PointF(s.Location.X + t*d*cosAlpha, s.Location.Y + t*d*sinAlpha)
            p
        else
            let alpha = (-90.f - arcAngle*0.5f + t*arcAngle)*single(System.Math.PI)/180.f
            let pt = [| new PointF(rect.X + rect.Width*0.5f + radius*cos(alpha), rect.Y + rect.Height*0.5f + radius*sin(alpha)); |]
            w2v.TransformPoints(pt)
            pt.[0]

    member this.Paint(g:Graphics) =
        let dist = dist()
        let cons = this.ConditionsString
        let sSize = g.MeasureString(cons, font)
        if dist <= 3.f*s.Radius || dest.Transitions |> Seq.exists(fun t -> t.Destination = s) then
            isLine <- false
            let k = single(System.Math.PI)/180.f
            arcAngle <- 720.f*s.Radius/(dist + 2.f*s.Radius)
            let side = max (s.Radius*2.f) (dist/sin(0.5f*arcAngle*k))
            radius <- side*0.5f
            rect <- new RectangleF((dist - side)*0.5f, radius*(cos(arcAngle*0.5f*k) - 1.f), side, side)
            let rMax = new RectangleF(rect.X - lnWidth*0.5f, rect.Y - lnWidth*0.5f, rect.Width + lnWidth, rect.Height + lnWidth)
            let rMin = new RectangleF(rect.X + lnWidth*0.5f, rect.Y + lnWidth*0.5f, rect.Width - lnWidth, rect.Height - lnWidth)
            if dist = 0.f then 
                rtAngle <- 0.f
            else 
                let h = acos((dest.Location.X - s.Location.X)/dist)/k
                if s.Location.Y > dest.Location.Y then
                    rtAngle <- -h
                else
                    rtAngle <- h
            let gs = g.Save()
            v2w.Reset()
            w2v.Reset()
            w2v.Translate(s.Location.X, s.Location.Y)
            w2v.Rotate(rtAngle)
            v2w.Translate(-s.Location.X, -s.Location.Y, Drawing2D.MatrixOrder.Append)
            v2w.Rotate(-rtAngle, Drawing2D.MatrixOrder.Append)
            g.MultiplyTransform(w2v)
            g.ResetClip()
            g.DrawArc(lnC, rect, -90.f - arcAngle*0.5f, arcAngle)
            regMax.Reset()
            regMin.Reset()
            regMax.AddArc(rMax, -90.f - arcAngle*0.5f, arcAngle)
            regMin.AddArc(rMin, -90.f - arcAngle*0.5f, arcAngle)
            g.Restore(gs)
        else
            isLine <- true
            g.DrawLine(lnC, s.Location, dest.Location)
        g.FillPolygon(bgC, arrow())
        let sPos = this.PointAt(0.5f)
        g.DrawString(cons, font, Brushes.DarkRed, sPos.X - sSize.Width*0.5f, sPos.Y - sSize.Height*0.5f) 

type ReturnType =
    | Abort
    | Ok
    | DeleteState
    | None



type RequestType =
    | NewState
    | NewTransition



type MessageBox() =
    let mutable reqType = NewState
    let mutable side = 20.f
    let mutable area = new RectangleF()
    let textBox = new TextBox()
    let bgC = Brushes.Black
    let btC = Brushes.White
    let btCDisabled = Brushes.Gray
    let abortBt = new Button(Delete)
    let okBt = new Button(Play)
    let delBt = new Button(Delete)
    let mutable message = ""
    let mutable title = ""
    let mutable s1 = new State()
    let mutable s2 = new State()
    let mutable tRect = new RectangleF()
    let mutable mRect = new RectangleF()
    let mutable s1Rect = new RectangleF()
    let mutable s2Rect = new RectangleF()
    let mutable trState = new Drawing2D.GraphicsPath()
    let mutable trTransition = new Drawing2D.GraphicsPath()
    let mutable points = new PointF()
    let mutable isVisible = false
    let mutable s1Enabled = true
    let mutable s2Enabled = false
    
    do
        area <- new RectangleF(0.f, 0.f, side*6.5f, side*4.f)
        tRect <- new RectangleF(0.f, 0.f, side*5.f, side)
        mRect <- new RectangleF(0.f, side*1.5f, side*6.5f, side)
        s1Rect <- new RectangleF(0.f, 4.f*side, side*5.f, side)
        s2Rect <- new RectangleF(0.f, 5.5f*side, side*5.f, side)
        okBt.Side <- side
        okBt.Location <- new PointF(5.5f*side, 3.f*side)
        okBt.Status <- Disabled
        abortBt.Side <- side
        abortBt.Location <- new PointF(5.5f*side, 0.f)
        delBt.Side <- side*0.5f
        delBt.Location <- new PointF(5.5f*side, 4.25f*side)
        delBt.Status <- Disabled
        trState.AddPolygon([| new PointF(0.f, 4.5f*side); new PointF(0.5f*side, 5.f*side); new PointF(1.f*side, 4.5f*side) |])
        trTransition.AddPolygon([| new PointF(4.f*side, 8.5f*side); new PointF(4.5f*side, 9.f*side); new PointF(5.f*side, 8.5f*side) |])
        textBox.BorderStyle <- BorderStyle.FixedSingle
        textBox.TextAlign <- HorizontalAlignment.Right
        textBox.Hide()
    
    member this.TextBox 
        with get() = textBox
    
    member this.AbortButton 
        with get() = abortBt
    
    member this.OkButton 
        with get() = okBt
    
    member this.DeleteButton 
        with get() = delBt
    
    member this.IsVisible 
        with get() = isVisible
        and set(b) = isVisible <- b
    
    member this.SndStringEnabled
        with get() = s2Enabled
        and set(b) = s2Enabled <- b
    
    member this.FstStringEnabled
        with get() = s1Enabled
        and set(b) = s1Enabled <- b
    
    member this.FstState
        with get() = s1
        and set(s) = s1 <- s
    
    member this.SndState
        with get() = s2
        and set(s) = s2 <- s
    
    member this.RequestType
        with get() = reqType
        and set(rt) =
            reqType <- rt
            match rt with
                | NewState ->
                    area.Size <- new SizeF(side*6.5f, side*4.f)
                    tRect.Size <- new SizeF(side*5.f, side)
                    mRect.Size <- new SizeF(side*6.5f, side)
                    okBt.Location <- new PointF(5.5f*side, 3.f*side)
                    abortBt.Location <- new PointF(5.5f*side, 0.f)
                    okBt.HasFocus <- false
                    okBt.Status <- Disabled
                    textBox.Enabled <- true
                | NewTransition ->
                    area.Size <- new SizeF(side*6.5f, side*8.f)
                    tRect.Size <- new SizeF(side*5.f, side)
                    mRect.Size <- new SizeF(side*6.5f, 2.f*side)
                    abortBt.Location <- new PointF(5.5f*side, 0.f)
                    okBt.Location <- new PointF(5.5f*side, 7.f*side)
                    textBox.Enabled <- false
                    delBt.Location <- new PointF(5.5f*side, 4.25f*side)
                    delBt.Status <- Disabled
                    okBt.HasFocus <- false
                    okBt.Status <- Disabled
                    s1Enabled <- true
                    s2Enabled <- false
                    s1 <- new State()
                    s2 <- new State()
    
    member this.Show() =
        isVisible <- true
        textBox.Text <- ""
        textBox.Show()
    
    member this.Hide() =
        isVisible <- false
        textBox.Hide()
    
    member this.Title
        with get() = title
        and set(s) = title <- s
    
    member this.Message
        with get() = message
        and set(s) = message <- s
    
    member this.Contains(p:PointF) = 
        area.Contains(p)
    
    member this.ButtonSelected(p:PointF) : ReturnType =
        let pt = new PointF(p.X - area.X, p.Y - area.Y)
        if abortBt.Contains(pt) then
            Abort
        elif okBt.Contains(pt) && okBt.Status <> Disabled then
            Ok
        elif delBt.Contains(pt) then
            DeleteState
        else
            None
    
    member this.Points
        with get() = points
        and set(p:PointF) =
            points <- p
            match reqType with
                | NewState ->
                    area.Location <- new PointF(p.X - side*0.5f, p.Y - side*5.f)
                    textBox.Location <- new Point(int(p.X - side*0.5f), int(p.Y - side*2.f))
                | NewTransition -> 
                    area.Location <- new PointF(p.X - side*4.5f, p.Y - side*9.f)
                    textBox.Location <- new Point(int(p.X - side*4.5f), int(p.Y - side*2.f))
            
    member this.Side
        with get() = side
        and set(s) =
            side <- s
            match reqType with
                | NewState ->
                    area.Size <- new SizeF(side*6.5f, side*4.f)
                    tRect.Size <- new SizeF(side*5.f, side)
                    mRect.Size <- new SizeF(side*6.5f, side)
                    okBt.Location <- new PointF(5.5f*side, 3.f*side)
                    textBox.Bounds <- new Rectangle(int(points.X - side*0.5f), int(points.Y - side*2.f), int(side*5.f), int(side))
                    abortBt.Location <- new PointF(5.5f*side, 0.f)
                    abortBt.Side <- s
                    okBt.Location <- new PointF(5.5f*side, 3.f*side)
                    okBt.Side <- s
                | NewTransition -> 
                    area.Size <- new SizeF(side*6.5f, side*8.f)
                    tRect.Size <- new SizeF(side*5.f, side)
                    mRect.Size <- new SizeF(side*6.5f, 2.f*side)
                    s1Rect.Size <- new SizeF(side*5.f, side)
                    s2Rect.Size <- new SizeF(side*5.f, side)
                    abortBt.Location <- new PointF(5.5f*side, 0.f)
                    textBox.Bounds <- new Rectangle(int(points.X - side*4.5f), int(points.Y - side*9.f), int(side*5.f), int(side))
                    abortBt.Location <- new PointF(5.5f*side, 0.f)
                    abortBt.Side <- s
                    okBt.Location <- new PointF(5.5f*side, 7.f*side)
                    okBt.Side <- s
                    delBt.Location <- new PointF(5.5f*side, 4.25f*side)
                    delBt.Side <- s*0.5f
    
    member this.Paint(g:Graphics) =
        if isVisible then
            match reqType with
                | NewState ->
                    g.TranslateTransform(area.X, area.Y)
                    g.FillEllipse(bgC, -side*0.5f, -side*0.5f, side, side)
                    g.FillEllipse(bgC, -side*0.5f, side*3.5f, side, side)
                    g.FillEllipse(bgC, side*6.f, -side*0.5f, side, side)
                    g.FillEllipse(bgC, side*6.f, side*3.5f, side, side)
                    g.FillRectangle(bgC, -side*0.5f, 0.f, area.Width + side, area.Height)
                    g.FillRectangle(bgC, 0.f, -side*0.5f, area.Width, area.Height + side)
                    g.FillPath(bgC, trState)
                    g.DrawString(title, new Font("Verdana", side*0.5f, FontStyle.Bold), btC, tRect)
                    g.DrawString(message, new Font("Verdana", side*0.4f), btC, mRect)
                    abortBt.Paint(g)
                    okBt.HasFocus <- false
                    okBt.Paint(g)
                    g.TranslateTransform(-area.X, -area.Y)
                | NewTransition -> 
                    g.TranslateTransform(area.X, area.Y)
                    g.FillEllipse(bgC, -side*0.5f, -side*0.5f, side, side)
                    g.FillEllipse(bgC, -side*0.5f, side*7.5f, side, side)
                    g.FillEllipse(bgC, side*6.f, -side*0.5f, side, side)
                    g.FillEllipse(bgC, side*6.f, side*7.5f, side, side)
                    g.FillRectangle(bgC, -side*0.5f, 0.f, area.Width + side, area.Height)
                    g.FillRectangle(bgC, 0.f, -side*0.5f, area.Width, area.Height + side)
                    g.FillPath(bgC, trTransition)
                    g.DrawString(title, new Font("Verdana", side*0.5f, FontStyle.Bold), btC, tRect)
                    g.DrawString(message, new Font("Verdana", side*0.4f), btC, mRect)
                    if s1Enabled then
                        g.DrawString("S1: " + s1.Name, new Font("Verdana", side*0.4f, FontStyle.Bold), btC, s1Rect)
                    else
                        g.DrawString("S1: " + s1.Name, new Font("Verdana", side*0.4f, FontStyle.Bold), btCDisabled, s1Rect)
                    if s2Enabled then
                        g.DrawString("S2: " + s2.Name, new Font("Verdana", side*0.4f, FontStyle.Bold), btC, s2Rect)
                    else
                        g.DrawString("S2: " + s2.Name, new Font("Verdana", side*0.4f, FontStyle.Bold), btCDisabled, s2Rect)
                    abortBt.Paint(g)
                    okBt.Paint(g)
                    if not s1Enabled then
                        delBt.Paint(g)
                    g.TranslateTransform(-area.X, -area.Y)



type Activity =
    | Idle
    | RequestingName
    | RequestingEntryAction
    | RequestingFstState
    | RequestingSndState
    | RequestingCondition



type Animation =
    | OnState
    | OnTransition



type Interface() as this =
    inherit UserControl()
    let tools = new Toolbar()
    let mutable lastBtClicked = -1
    let mutable lastStClicked = -1
    let mutable lastMsClicked = None
    let mutable off = new PointF()
    let mutable input = ""
    let states = new ResizeArray<State>()
    let mutable zOrder = []
    let mBox = new MessageBox()
    let mutable activity = Idle
    let mutable ind = -1
    let v2w = new Drawing2D.Matrix()
    let w2v = new Drawing2D.Matrix()
    let timer = new Timer(Interval = 30)
    let mutable scrolling = false
    let mutable animating = false
    let zoomIn = 1.25f
    let zoomOut = 1.f/zoomIn
    let scroll = 3.f
    let mutable mBoxPos = new PointF()
    let mutable mBoxTrPos = new PointF()
    let mutable lastTrSelected = new Transition(new State())
    let mutable trClicked = false
    let mutable currentState = new State()
    let mutable currentTransition = new Transition(new State())
    let mutable t = 0.f
    let mutable speed = 1.f
    let step = 0.01f
    let markerDim = 5.f
    let mutable anim = OnState
    let mutable animationStatus = ""
    let font = new Font("Verdana", 8.f)
    let mutable random = false
    let rec gradientCircle(g:Graphics, center:PointF, radius:float32, color:Color) =
        if int(color.A) > 0 then
            let ratio = 1.f
            let step = 20
            let rect = new RectangleF(center.X - radius, center.Y - radius, radius*2.f, radius*2.f)
            gradientCircle(g, center, radius + ratio, Color.FromArgb(max 0 (int(color.A) - step), color))
            g.DrawEllipse(new Pen(color, ratio), rect)
    let alphaFun(color:Color, t) = 
        let s = min 1. (max 0. (System.Math.Sin(System.Math.PI*10.*double(t))))
        Color.FromArgb(int(s*s*255.), color)
    let animation(g:Graphics) =
        if animating then
            let animC = Color.FromArgb(255, Color.Red)
            if t >= 1.f then
                t <- 0.f
                match anim with
                    | OnState ->
                        let founds = currentState.Transitions |> Seq.where(fun (t:Transition) -> t.Conditions |> Seq.exists(fun c -> c = input)) |> Seq.toArray
                        let n = founds |> Seq.length
                        if n > 1 then
                            let rnd = System.Random()
                            random <- true
                            currentTransition <- founds.[rnd.Next(n)]
                            anim <- OnTransition
                        elif n = 1 then
                            random <- false
                            currentTransition <- founds.[0]
                            anim <- OnTransition
                        else
                            animating <- false
                            if not scrolling then
                                timer.Stop()
                            currentState <- states.Find(fun s -> s.Initial)
                            if lastStClicked <> -1 then
                                states.[lastStClicked].Selected <- false
                                lastStClicked <- -1
                            if trClicked then
                                trClicked <- false
                                lastTrSelected.Selected <- false
                            currentState <- states.Find(fun s -> s.Initial)
                            tools.Buttons.[0].Status <- Normal
                            tools.Buttons.[1].Status <- Normal
                            tools.Buttons.[2].Status <- Disabled
                            tools.Buttons.[3].Status <- Disabled
                            tools.Buttons.[3].Status <- Disabled
                            tools.Buttons.[12].Status <- Normal
                            tools.Buttons.[13].Status <- Disabled
                            tools.Buttons.[14].Status <- Disabled
                            tools.Buttons.[15].Status <- Disabled
                            tools.Buttons.[16].Status <- Disabled
                            speed <- 1.0f
                            animationStatus <- ""
                    | OnTransition ->
                        currentState <- currentTransition.Destination
                        anim <- OnState
            else
                match anim with
                    | OnState ->
                        gradientCircle(g, currentState.Location, currentState.Radius, alphaFun(animC, t))
                        animationStatus <- "State: " + currentState.Name + "\nTransiting to: -\nInput: " + input
                    | OnTransition ->
                        gradientCircle(g, currentTransition.PointAt(t), 0.f, alphaFun(animC, t))
                        animationStatus <- "State: " + currentState.Name + "\nTransiting to: " + currentTransition.Destination.Name
                            + (if random then " (Random)" else "") + "\nInput: " + input
    do
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer ||| ControlStyles.AllPaintingInWmPaint, true)
        this.Controls.Add(tools.TextBox)
        this.Controls.Add(mBox.TextBox)
        timer.Tick.Add(fun _ -> 
            if scrolling then
                match tools.Buttons.[lastBtClicked].Type with
                    | LeftArrow ->
                        w2v.Translate(scroll, 0.f)
                        v2w.Translate(-scroll, 0.f, Drawing2D.MatrixOrder.Append)
                    | UpArrow ->
                        w2v.Translate(0.f, scroll)
                        v2w.Translate(0.f, -scroll, Drawing2D.MatrixOrder.Append)
                    | RightArrow ->
                        w2v.Translate(-scroll, 0.f)
                        v2w.Translate(scroll, 0.f, Drawing2D.MatrixOrder.Append)
                    | DownArrow ->
                        w2v.Translate(0.f, -scroll)
                        v2w.Translate(0.f, scroll, Drawing2D.MatrixOrder.Append)
                    | _ -> ()
            if animating then
                t <- min 1.f (t + speed*step)
            this.Invalidate())
        mBox.TextBox.TextChanged.Add(fun _ ->
            if activity = RequestingName then
                if mBox.TextBox.Text = "" then
                    mBox.OkButton.Status <- Disabled
                else
                    mBox.OkButton.Status <- Normal
            this.Invalidate())
        tools.TextBox.TextChanged.Add(fun _ ->
            if tools.TextBox.Text = input then
                tools.Buttons.[11].Status <- Disabled
            else
                tools.Buttons.[11].Status <- Normal
            this.Invalidate())
    
    override this.OnMouseDown(e) =
        let p = new PointF(single(e.X), single(e.Y))
        let p2w = [| p |]
        v2w.TransformPoints(p2w)
        let p2w = p2w.[0]
        if tools.Contains(p) then
            if tools.Buttons.[tools.Index].Status <> Disabled then
                if ((tools.Index <> lastBtClicked) && (lastBtClicked <> -1)) then
                    tools.Buttons.[lastBtClicked].Status <- Normal
                tools.Buttons.[tools.Index].Status <- Clicked
                lastBtClicked <- tools.Index
                match tools.Buttons.[lastBtClicked].Type with
                    | LeftArrow | RightArrow | UpArrow | DownArrow ->
                        if not animating then
                            timer.Start()
                        scrolling <- true
                    | _ -> ()
        if mBox.IsVisible && mBox.Contains(p) then
            lastMsClicked <- mBox.ButtonSelected(p)
            match lastMsClicked with
                | Ok ->
                    mBox.OkButton.Status <- Clicked
                | Abort ->
                    mBox.AbortButton.Status <- Clicked
                | DeleteState ->
                    mBox.DeleteButton.Status <- Clicked
                | _ -> ()
        if states |> Seq.exists(fun s -> s.Contains(p2w)) then
            for i in zOrder do
                if states.[i].Contains(p2w) then
                    lastStClicked <- states.IndexOf(states.[i])
                    off <- new PointF(p2w.X - states.[lastStClicked].Location.X, p2w.Y - states.[lastStClicked].Location.Y)
                    if mBox.IsVisible && mBox.RequestType = NewTransition then
                        match activity with
                            | RequestingFstState ->
                                mBox.FstState <- states.[i]
                                activity <- RequestingSndState
                                mBox.DeleteButton.Status <- Normal
                                mBox.FstStringEnabled <- false
                                mBox.SndStringEnabled <- true
                            | RequestingSndState ->
                                mBox.SndState <- states.[i]
                                activity <- RequestingCondition
                                mBox.DeleteButton.Location <- new PointF(mBox.DeleteButton.Location.X,
                                    mBox.DeleteButton.Location.Y + mBox.Side*1.5f)
                                mBox.TextBox.Enabled <- true
                                mBox.SndStringEnabled <- false
                                mBox.OkButton.Status <- Normal
                            | _ -> ()
                    elif tools.Buttons.[14].Status = Disabled then
                        if states.[i].Initial then
                            tools.Buttons.[3].Status <- Disabled
                        else
                            tools.Buttons.[3].Status <- Normal
                        tools.Buttons.[4].Status <- Normal
                states.[i].Selected <- false
            zOrder <- zOrder |> List.filter(fun i -> i <> lastStClicked)
            zOrder <- zOrder @ [ lastStClicked ]
            states.[lastStClicked].Selected <- true
            if trClicked then
                trClicked <- false
                lastTrSelected.Selected <- false
        elif not(mBox.IsVisible || tools.Contains(p)) && tools.Buttons.[14].Status = Disabled then
            trClicked <- false
            lastStClicked <- -1
            for s in states do
                s.Selected <- false
                for t in s.Transitions do
                    if t.Contains(p) && not(trClicked) then
                        trClicked <- true
                        t.Selected <- true
                        lastTrSelected <- t
                    else
                        t.Selected <- false
            if trClicked then
                tools.Buttons.[4].Status <- Normal
            else
                tools.Buttons.[4].Status <- Disabled
            tools.Buttons.[3].Status <- Disabled
        this.Invalidate()
    
    override this.OnMouseUp(e) =
        let p = new PointF(single(e.X), single(e.Y))
        let p2w = [| p |]
        v2w.TransformPoints(p2w)
        let p2w = p2w.[0]
        if ((lastBtClicked <> -1) && (tools.Buttons.[lastBtClicked].Status = Clicked)) then
            tools.Buttons.[lastBtClicked].Status <- Normal
        if lastBtClicked <> -1 && tools.Index = lastBtClicked then
            match tools.Buttons.[lastBtClicked].Type with
                | Delete ->
                    if trClicked then
                        lastTrSelected.Source.Transitions.Remove(lastTrSelected) |> ignore
                        trClicked <- false
                    else
                        try
                            let id = states.FindIndex(fun s -> s.Selected)
                            if states.Count > 1 && states.[id].Initial then
                                let newIn = if id = 0 then 1 else 0
                                states.[newIn].Initial <- true
                                currentState <- states.[newIn]
                            for t in states.[id].Transitions do
                                t.Destination.Transitions.RemoveAll(fun tr -> tr.Destination = states.[id]) |> ignore
                            states.RemoveAt(id)
                            zOrder <- zOrder |> List.filter(fun i -> i <> id) |> List.map(fun i -> if i > id then i - 1 else i)
                            tools.Buttons.[4].Status <- Disabled
                            tools.Buttons.[3].Status <- Disabled
                            if states.Count = 0 then
                                tools.DisableAll()
                                tools.Buttons.[1].Status <- Normal
                                tools.TextBox.Text <- ""
                                input <- ""
                                tools.TextBox.Enabled <- false
                        with _ -> ()
                | Initial ->
                    states.Find(fun s -> s.Initial).Initial <- false
                    currentState <- states.Find(fun s -> s.Selected)
                    currentState.Initial <- true
                    tools.Buttons.[3].Status <- Disabled
                | Clear ->
                    states.Clear()
                    zOrder <- []
                    tools.DisableAll()
                    tools.Buttons.[1].Status <- Normal
                    tools.TextBox.Text <- ""
                    input <- ""
                    tools.TextBox.Enabled <- false
                | AddState ->
                    ind <- states.Count
                    states.Add(new State())
                    zOrder <- zOrder @ [ ind ]
                    if states.Count = 1 then
                        states.[0].Initial <- true
                        currentState <- states.[0]
                        tools.Buttons.[12].Status <- Normal
                        tools.TextBox.Enabled <- true
                    activity <- RequestingName
                    mBoxPos <- new PointF(states.[ind].Location.X, states.[ind].Location.Y - states.[ind].Radius*0.5f)
                    mBox.Title <- "Name"
                    mBox.Message <- "Insert a Name:"
                    mBox.RequestType <- NewState
                    mBox.Show()
                    tools.Save()
                    tools.DisableAll()
                | AddTransition ->
                    lastStClicked <- -1
                    states |> Seq.iter(fun s -> s.Selected <- false)
                    activity <- RequestingFstState
                    mBox.RequestType <- NewTransition
                    mBox.Title <- "Transition"
                    mBox.Message <- "Select two states and insert a condition to create a new transition:"
                    mBox.Show()
                    tools.Save()
                    tools.DisableAll()
                | LeftArrow | RightArrow | UpArrow | DownArrow ->
                    if not animating then
                        timer.Stop()
                    scrolling <- false
                | ZoomIn ->
                    w2v.Translate(single(this.Width)*0.5f, tools.Location.Y*0.5f)
                    w2v.Scale(zoomIn, zoomIn)
                    w2v.Translate(-single(this.Width)*0.5f, -tools.Location.Y*0.5f)
                    v2w.Translate(-single(this.Width)*0.5f, -tools.Location.Y*0.5f, Drawing2D.MatrixOrder.Append)
                    v2w.Scale(zoomOut, zoomOut, Drawing2D.MatrixOrder.Append)
                    v2w.Translate(single(this.Width)*0.5f, tools.Location.Y*0.5f, Drawing2D.MatrixOrder.Append)
                | ZoomOut ->
                    w2v.Translate(single(this.Width)*0.5f, tools.Location.Y*0.5f)
                    w2v.Scale(zoomOut, zoomOut)
                    w2v.Translate(-single(this.Width)*0.5f, -tools.Location.Y*0.5f)
                    v2w.Translate(-single(this.Width)*0.5f, -tools.Location.Y*0.5f, Drawing2D.MatrixOrder.Append)
                    v2w.Scale(zoomIn, zoomIn, Drawing2D.MatrixOrder.Append)
                    v2w.Translate(single(this.Width)*0.5f, tools.Location.Y*0.5f, Drawing2D.MatrixOrder.Append)
                | Reload ->
                    input <- tools.TextBox.Text
                    tools.Buttons.[11].Status <- Disabled
                | Play ->
                    animating <- true
                    if not scrolling then
                        timer.Start()
                    tools.Buttons.[0].Status <- Disabled
                    tools.Buttons.[1].Status <- Disabled
                    tools.Buttons.[2].Status <- Disabled
                    tools.Buttons.[3].Status <- Disabled
                    tools.Buttons.[4].Status <- Disabled
                    tools.Buttons.[12].Status <- Disabled
                    tools.Buttons.[13].Status <- Normal
                    tools.Buttons.[14].Status <- Normal
                    tools.Buttons.[15].Status <- Normal
                    tools.Buttons.[16].Status <- Normal
                | Pause ->
                    animating <- false
                    if not scrolling then
                        timer.Stop()
                    tools.Buttons.[12].Status <- Normal
                    tools.Buttons.[13].Status <- Disabled
                | Stop ->
                    animating <- false
                    if not scrolling then
                        timer.Stop()
                    t <- 0.f
                    if lastStClicked <> -1 then
                        states.[lastStClicked].Selected <- false
                        lastStClicked <- -1
                    if trClicked then
                        trClicked <- false
                        lastTrSelected.Selected <- false
                    currentState <- states.Find(fun s -> s.Initial)
                    tools.Buttons.[0].Status <- Normal
                    tools.Buttons.[1].Status <- Normal
                    tools.Buttons.[2].Status <- Disabled
                    tools.Buttons.[3].Status <- Disabled
                    tools.Buttons.[4].Status <- Disabled
                    tools.Buttons.[12].Status <- Normal
                    tools.Buttons.[13].Status <- Disabled
                    tools.Buttons.[14].Status <- Disabled
                    tools.Buttons.[15].Status <- Disabled
                    tools.Buttons.[16].Status <- Disabled
                    speed <- 1.0f
                | Faster -> 
                    speed <- speed*1.2f
                | Slower ->
                    speed <- speed/1.2f
            lastBtClicked <- -1
        if mBox.IsVisible then
            if mBox.ButtonSelected(p) = lastMsClicked then
                match lastMsClicked with
                    | Ok ->
                        match activity with
                            | RequestingName ->
                                mBox.OkButton.Status <- Normal
                                states.[ind].Name <- mBox.TextBox.Text
                                activity <- RequestingEntryAction
                                mBoxPos <- new PointF(states.[ind].Location.X, states.[ind].Location.Y + states.[ind].Radius*0.5f)
                                mBox.Title <- "Entry Action"
                                mBox.Message <- "Insert the Entry Action:"
                                mBox.TextBox.Text <- ""
                            | RequestingEntryAction ->
                                mBox.OkButton.Status <- Disabled
                                states.[ind].EntryAction <- mBox.TextBox.Text
                                activity <- Idle
                                mBox.Hide()
                                tools.Restore()
                                tools.Buttons.[0].Status <- Normal
                                tools.Buttons.[2].Status <- Normal
                            | RequestingCondition ->
                                mBox.FstState.AddTransition(mBox.SndState, mBox.TextBox.Text)
                                mBox.OkButton.Status <- Disabled
                                activity <- Idle
                                mBox.Hide()
                                tools.Restore()
                            | _ -> ()
                    | Abort -> 
                        mBox.AbortButton.Status <- Normal
                        match activity with
                            | RequestingName | RequestingEntryAction ->
                                activity <- Idle
                                mBox.OkButton.Status <- Disabled
                                mBox.Hide()
                                states.RemoveAt(ind)
                                zOrder <- zOrder |> List.filter(fun i -> i <> ind)
                                tools.Restore()
                            | RequestingCondition | RequestingFstState | RequestingSndState ->
                                activity <- Idle
                                mBox.Hide()
                                tools.Restore()
                            |_ -> ()
                    | DeleteState ->
                        mBox.OkButton.Status <- Disabled
                        match activity with
                            | RequestingCondition ->
                                activity <- RequestingSndState
                                mBox.TextBox.Enabled <- false
                                mBox.DeleteButton.Location <- new PointF(mBox.DeleteButton.Location.X,
                                    mBox.DeleteButton.Location.Y - mBox.Side*1.5f)
                                mBox.DeleteButton.Status <- Normal
                                mBox.SndStringEnabled <- true
                                mBox.SndState <- new State()
                            | RequestingSndState ->
                                activity <- RequestingFstState
                                mBox.FstState <- new State()
                                mBox.SndStringEnabled <- false
                                mBox.FstStringEnabled <- true
                            | _ -> ()
                    | None -> ()
            else
                match lastMsClicked with
                | Abort ->
                    mBox.AbortButton.Status <- Normal
                | Ok ->
                    mBox.OkButton.Status <- Disabled
                | DeleteState ->
                    mBox.DeleteButton.Status <- Normal
                | _ -> ()
        lastStClicked <- -1
        lastMsClicked <- None
        lastBtClicked <- -1
        this.Invalidate()

    override this.OnMouseMove(e) =
        let p = new PointF(single(e.X), single(e.Y))
        let p2w = [| p |]
        v2w.TransformPoints(p2w)
        let p2w = p2w.[0]
        if mBox.IsVisible then
            mBox.ButtonSelected(p) |> ignore
        elif lastStClicked <> -1 then
            states.[lastStClicked].Location <- new PointF(p2w.X - off.X, p2w.Y - off.Y)
        else
            tools.Contains(p) |> ignore
        this.Invalidate()
    
    override this.OnResize(e) =
        tools.Location <- new PointF((single(this.Width) - tools.Size.Width)*0.5f, single(this.Height) - tools.Size.Height*1.5f)
        mBoxTrPos <- new PointF(tools.Location.X + tools.Buttons.[2].Location.X + tools.Buttons.[2].Side*0.5f,
            tools.Location.Y + tools.Buttons.[2].Location.Y)
        this.Invalidate()
 
    override this.OnPaint(e) =
        let g = e.Graphics
        g.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias
        g.Clear(Color.WhiteSmoke)
        let s = g.Save()
        g.MultiplyTransform(w2v)
        animation(g)
        for s in states do
            for t in s.Transitions do
                t.Paint(g)
        for i in zOrder do
            states.[i].Paint(g)
        g.Restore(s)
        if mBox.IsVisible then
            if mBox.RequestType = NewState then
                let pt = [| mBoxPos |]
                w2v.TransformPoints(pt)
                mBox.Points <- pt.[0]
            else
                mBox.Points <- mBoxTrPos
            mBox.Paint(g)
        if animating then
            let strSize = g.MeasureString(animationStatus, font)
            let strLoc = new PointF(tools.Location.X + tools.Buttons.[12].Location.X,
                                    tools.Location.Y + tools.Buttons.[12].Location.Y - strSize.Height - 3.f)
            g.DrawString(animationStatus, font, Brushes.Black, strLoc)
        tools.Paint(g)



let form = new Form(Text="FSM Diagram Generator", Height = 400, Width = 800)
let interf = new Interface(Dock=DockStyle.Fill)
form.Controls.Add(interf)
form.MinimumSize <- new Size(800, 400)
form.Show()  
Application.Run(form) 
Application.Exit()
