Partial Public Class Form1
    Inherits DevExpress.XtraEditors.XtraForm

    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.SimpleButton1 = New DevExpress.XtraEditors.SimpleButton()
        Me.GridControl1 = New DevExpress.XtraGrid.GridControl()
        Me.GridView1 = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.SimpleButton2 = New DevExpress.XtraEditors.SimpleButton()
        Me.SplitterControl1 = New DevExpress.XtraEditors.SplitterControl()
        Me.AsksGridControl = New DevExpress.XtraGrid.GridControl()
        Me.GridView2 = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.SplitterControl2 = New DevExpress.XtraEditors.SplitterControl()
        Me.BidsGridControl = New DevExpress.XtraGrid.GridControl()
        Me.GridView3 = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.GridControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AsksGridControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BidsGridControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SimpleButton1
        '
        Me.SimpleButton1.Dock = System.Windows.Forms.DockStyle.Top
        Me.SimpleButton1.Location = New System.Drawing.Point(0, 0)
        Me.SimpleButton1.Name = "SimpleButton1"
        Me.SimpleButton1.Size = New System.Drawing.Size(971, 23)
        Me.SimpleButton1.TabIndex = 0
        Me.SimpleButton1.Text = "Start"
        '
        'GridControl1
        '
        Me.GridControl1.Dock = System.Windows.Forms.DockStyle.Left
        Me.GridControl1.Location = New System.Drawing.Point(0, 23)
        Me.GridControl1.MainView = Me.GridView1
        Me.GridControl1.Name = "GridControl1"
        Me.GridControl1.Size = New System.Drawing.Size(392, 493)
        Me.GridControl1.TabIndex = 1
        Me.GridControl1.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridView1})
        '
        'GridView1
        '
        Me.GridView1.GridControl = Me.GridControl1
        Me.GridView1.Name = "GridView1"
        '
        'SimpleButton2
        '
        Me.SimpleButton2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.SimpleButton2.Location = New System.Drawing.Point(0, 516)
        Me.SimpleButton2.Name = "SimpleButton2"
        Me.SimpleButton2.Size = New System.Drawing.Size(971, 23)
        Me.SimpleButton2.TabIndex = 2
        Me.SimpleButton2.Text = "Stop"
        '
        'SplitterControl1
        '
        Me.SplitterControl1.Location = New System.Drawing.Point(392, 23)
        Me.SplitterControl1.Name = "SplitterControl1"
        Me.SplitterControl1.Size = New System.Drawing.Size(10, 493)
        Me.SplitterControl1.TabIndex = 3
        Me.SplitterControl1.TabStop = False
        '
        'AsksGridControl
        '
        Me.AsksGridControl.Dock = System.Windows.Forms.DockStyle.Top
        Me.AsksGridControl.Location = New System.Drawing.Point(402, 23)
        Me.AsksGridControl.MainView = Me.GridView2
        Me.AsksGridControl.Name = "AsksGridControl"
        Me.AsksGridControl.Size = New System.Drawing.Size(569, 241)
        Me.AsksGridControl.TabIndex = 4
        Me.AsksGridControl.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridView2})
        '
        'GridView2
        '
        Me.GridView2.GridControl = Me.AsksGridControl
        Me.GridView2.Name = "GridView2"
        '
        'SplitterControl2
        '
        Me.SplitterControl2.Dock = System.Windows.Forms.DockStyle.Top
        Me.SplitterControl2.Location = New System.Drawing.Point(402, 264)
        Me.SplitterControl2.Name = "SplitterControl2"
        Me.SplitterControl2.Size = New System.Drawing.Size(569, 10)
        Me.SplitterControl2.TabIndex = 5
        Me.SplitterControl2.TabStop = False
        '
        'BidsGridControl
        '
        Me.BidsGridControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BidsGridControl.Location = New System.Drawing.Point(402, 274)
        Me.BidsGridControl.MainView = Me.GridView3
        Me.BidsGridControl.Name = "BidsGridControl"
        Me.BidsGridControl.Size = New System.Drawing.Size(569, 242)
        Me.BidsGridControl.TabIndex = 6
        Me.BidsGridControl.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridView3})
        '
        'GridView3
        '
        Me.GridView3.GridControl = Me.BidsGridControl
        Me.GridView3.Name = "GridView3"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(971, 539)
        Me.Controls.Add(Me.BidsGridControl)
        Me.Controls.Add(Me.SplitterControl2)
        Me.Controls.Add(Me.AsksGridControl)
        Me.Controls.Add(Me.SplitterControl1)
        Me.Controls.Add(Me.GridControl1)
        Me.Controls.Add(Me.SimpleButton2)
        Me.Controls.Add(Me.SimpleButton1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.GridControl1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AsksGridControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BidsGridControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SimpleButton1 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents GridControl1 As DevExpress.XtraGrid.GridControl
    Friend WithEvents GridView1 As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents SimpleButton2 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents SplitterControl1 As DevExpress.XtraEditors.SplitterControl
    Friend WithEvents AsksGridControl As DevExpress.XtraGrid.GridControl
    Friend WithEvents GridView2 As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents SplitterControl2 As DevExpress.XtraEditors.SplitterControl
    Friend WithEvents BidsGridControl As DevExpress.XtraGrid.GridControl
    Friend WithEvents GridView3 As DevExpress.XtraGrid.Views.Grid.GridView

#End Region

End Class
