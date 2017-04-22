// Francesco Landolfi
// Matr. 444151
//
// Sviluppato con:
//  - Linux Mint 16 (Ubuntu 13.10)
//  - MonoDevelop 4.2.2
//  - Mono 3.2.3
//  - F# 3.0 (Open Source Edition)

open System.Windows.Forms
open System.Drawing

let fillPolygon(bmp:Bitmap, pts:Point array) =
    let minY = pts |> Seq.map(fun p -> p.Y) |> Seq.min
    let maxY = pts |> Seq.map(fun p -> p.Y) |> Seq.max
    let n = pts.Length
    let edges = ResizeArray<int>()
    for y = minY to maxY do
        for i = 0 to n - 1 do
            let p1 = pts.[i]
            let p2 = pts.[(i + 1) % n]
            if y < p1.Y && y > p2.Y || y > p1.Y && y < p2.Y then
                let x = (y - p2.Y)*(p1.X - p2.X)/(p1.Y - p2.Y) + p2.X
                edges.Add(x)
        edges.Sort()
        let m = edges.Count
        for j in 0 .. 2 .. m - 2 do
            for x = edges.[j] to edges.[j + 1] do
                bmp.SetPixel(x, y, Color.Black)
        edges.Clear()

//---------------------------------------- Test --------------------------------------------------

type Interface() as this =
    inherit UserControl()
//    let pts = [| new Point(200, 50); new Point(50, 300); new Point(300, 100); new Point(100, 100); new Point(300, 300); |]
//    let pts = [| new Point(100, 100); new Point(300, 100); new Point(300, 300); new Point(100, 300); |]
//    let pts = [| new Point(100, 100); new Point(100, 200); new Point(200, 200); new Point(300, 200); new Point(300,300); |]
    let pts = [| new Point(100, 100); new Point(100, 200); new Point(300, 200); new Point(300,300); |]

    let bmp = new Bitmap(400, 400)
    
    do
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer|||ControlStyles.AllPaintingInWmPaint,true)
        fillPolygon(bmp, pts)
        
    override this.OnPaint(e) =
        let g = e.Graphics
        g.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias
        g.DrawImage(bmp, 0, 0)
        
let form = new Form(Text = "FillPolygon", Height = 400, Width = 400)
let interf = new Interface(Dock = DockStyle.Fill)
form.Controls.Add(interf)
form.MaximumSize <- new Size(400, 400)
form.MinimumSize <- new Size(400, 400)
form.Show()  
Application.Run(form)









