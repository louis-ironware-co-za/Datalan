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
        Me.TradeItemsBuyGridcontrol = New DevExpress.XtraGrid.GridControl()
        Me.TradeItemsBuyGridView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.TradeItemsSellGridControl = New DevExpress.XtraGrid.GridControl()
        Me.TradeItemsSellGridView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.SplitContainerControl1 = New DevExpress.XtraEditors.SplitContainerControl()
        Me.PanelControl1 = New DevExpress.XtraEditors.PanelControl()
        Me.SplitContainerControl2 = New DevExpress.XtraEditors.SplitContainerControl()
        Me.BuyC1Gridcontrol = New DevExpress.XtraGrid.GridControl()
        Me.GridView1 = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.SellC1Gridcontrol = New DevExpress.XtraGrid.GridControl()
        Me.GridView2 = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.TradeItemsBuyGridcontrol, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TradeItemsBuyGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TradeItemsSellGridControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TradeItemsSellGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainerControl1.Panel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerControl1.Panel1.SuspendLayout()
        CType(Me.SplitContainerControl1.Panel2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerControl1.Panel2.SuspendLayout()
        Me.SplitContainerControl1.SuspendLayout()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl1.SuspendLayout()
        CType(Me.SplitContainerControl2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainerControl2.Panel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerControl2.Panel1.SuspendLayout()
        CType(Me.SplitContainerControl2.Panel2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerControl2.Panel2.SuspendLayout()
        Me.SplitContainerControl2.SuspendLayout()
        CType(Me.BuyC1Gridcontrol, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SellC1Gridcontrol, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SimpleButton1
        '
        Me.SimpleButton1.Location = New System.Drawing.Point(688, 8)
        Me.SimpleButton1.Name = "SimpleButton1"
        Me.SimpleButton1.Size = New System.Drawing.Size(75, 23)
        Me.SimpleButton1.TabIndex = 0
        Me.SimpleButton1.Text = "SimpleButton1"
        '
        'TradeItemsBuyGridcontrol
        '
        Me.TradeItemsBuyGridcontrol.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TradeItemsBuyGridcontrol.Location = New System.Drawing.Point(0, 0)
        Me.TradeItemsBuyGridcontrol.MainView = Me.TradeItemsBuyGridView
        Me.TradeItemsBuyGridcontrol.Name = "TradeItemsBuyGridcontrol"
        Me.TradeItemsBuyGridcontrol.Size = New System.Drawing.Size(392, 185)
        Me.TradeItemsBuyGridcontrol.TabIndex = 1
        Me.TradeItemsBuyGridcontrol.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.TradeItemsBuyGridView})
        '
        'TradeItemsBuyGridView
        '
        Me.TradeItemsBuyGridView.GridControl = Me.TradeItemsBuyGridcontrol
        Me.TradeItemsBuyGridView.Name = "TradeItemsBuyGridView"
        '
        'TradeItemsSellGridControl
        '
        Me.TradeItemsSellGridControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TradeItemsSellGridControl.Location = New System.Drawing.Point(0, 0)
        Me.TradeItemsSellGridControl.MainView = Me.TradeItemsSellGridView
        Me.TradeItemsSellGridControl.Name = "TradeItemsSellGridControl"
        Me.TradeItemsSellGridControl.Size = New System.Drawing.Size(392, 242)
        Me.TradeItemsSellGridControl.TabIndex = 2
        Me.TradeItemsSellGridControl.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.TradeItemsSellGridView})
        '
        'TradeItemsSellGridView
        '
        Me.TradeItemsSellGridView.GridControl = Me.TradeItemsSellGridControl
        Me.TradeItemsSellGridView.Name = "TradeItemsSellGridView"
        '
        'SplitContainerControl1
        '
        Me.SplitContainerControl1.Dock = System.Windows.Forms.DockStyle.Right
        Me.SplitContainerControl1.Horizontal = False
        Me.SplitContainerControl1.Location = New System.Drawing.Point(376, 40)
        Me.SplitContainerControl1.Name = "SplitContainerControl1"
        '
        'SplitContainerControl1.Panel1
        '
        Me.SplitContainerControl1.Panel1.Controls.Add(Me.TradeItemsBuyGridcontrol)
        Me.SplitContainerControl1.Panel1.Text = "Panel1"
        '
        'SplitContainerControl1.Panel2
        '
        Me.SplitContainerControl1.Panel2.Controls.Add(Me.TradeItemsSellGridControl)
        Me.SplitContainerControl1.Panel2.Text = "Panel2"
        Me.SplitContainerControl1.Size = New System.Drawing.Size(392, 437)
        Me.SplitContainerControl1.SplitterPosition = 185
        Me.SplitContainerControl1.TabIndex = 3
        '
        'PanelControl1
        '
        Me.PanelControl1.Controls.Add(Me.SimpleButton1)
        Me.PanelControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelControl1.Location = New System.Drawing.Point(0, 0)
        Me.PanelControl1.Name = "PanelControl1"
        Me.PanelControl1.Size = New System.Drawing.Size(768, 40)
        Me.PanelControl1.TabIndex = 4
        '
        'SplitContainerControl2
        '
        Me.SplitContainerControl2.Dock = System.Windows.Forms.DockStyle.Left
        Me.SplitContainerControl2.Horizontal = False
        Me.SplitContainerControl2.Location = New System.Drawing.Point(0, 40)
        Me.SplitContainerControl2.Name = "SplitContainerControl2"
        '
        'SplitContainerControl2.Panel1
        '
        Me.SplitContainerControl2.Panel1.Controls.Add(Me.BuyC1Gridcontrol)
        Me.SplitContainerControl2.Panel1.Text = "Panel1"
        '
        'SplitContainerControl2.Panel2
        '
        Me.SplitContainerControl2.Panel2.Controls.Add(Me.SellC1Gridcontrol)
        Me.SplitContainerControl2.Panel2.Text = "Panel2"
        Me.SplitContainerControl2.Size = New System.Drawing.Size(352, 437)
        Me.SplitContainerControl2.SplitterPosition = 227
        Me.SplitContainerControl2.TabIndex = 5
        '
        'BuyC1Gridcontrol
        '
        Me.BuyC1Gridcontrol.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BuyC1Gridcontrol.Location = New System.Drawing.Point(0, 0)
        Me.BuyC1Gridcontrol.MainView = Me.GridView1
        Me.BuyC1Gridcontrol.Name = "BuyC1Gridcontrol"
        Me.BuyC1Gridcontrol.Size = New System.Drawing.Size(352, 227)
        Me.BuyC1Gridcontrol.TabIndex = 2
        Me.BuyC1Gridcontrol.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridView1})
        '
        'GridView1
        '
        Me.GridView1.GridControl = Me.BuyC1Gridcontrol
        Me.GridView1.Name = "GridView1"
        '
        'SellC1Gridcontrol
        '
        Me.SellC1Gridcontrol.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SellC1Gridcontrol.Location = New System.Drawing.Point(0, 0)
        Me.SellC1Gridcontrol.MainView = Me.GridView2
        Me.SellC1Gridcontrol.Name = "SellC1Gridcontrol"
        Me.SellC1Gridcontrol.Size = New System.Drawing.Size(352, 200)
        Me.SellC1Gridcontrol.TabIndex = 2
        Me.SellC1Gridcontrol.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridView2})
        '
        'GridView2
        '
        Me.GridView2.GridControl = Me.SellC1Gridcontrol
        Me.GridView2.Name = "GridView2"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(768, 477)
        Me.Controls.Add(Me.SplitContainerControl2)
        Me.Controls.Add(Me.SplitContainerControl1)
        Me.Controls.Add(Me.PanelControl1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.TradeItemsBuyGridcontrol, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TradeItemsBuyGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TradeItemsSellGridControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TradeItemsSellGridView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SplitContainerControl1.Panel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl1.Panel1.ResumeLayout(False)
        CType(Me.SplitContainerControl1.Panel2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl1.ResumeLayout(False)
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl1.ResumeLayout(False)
        CType(Me.SplitContainerControl2.Panel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl2.Panel1.ResumeLayout(False)
        CType(Me.SplitContainerControl2.Panel2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerControl2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl2.ResumeLayout(False)
        CType(Me.BuyC1Gridcontrol, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SellC1Gridcontrol, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridView2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SimpleButton1 As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TradeItemsBuyGridcontrol As DevExpress.XtraGrid.GridControl
    Friend WithEvents TradeItemsBuyGridView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents TradeItemsSellGridControl As DevExpress.XtraGrid.GridControl
    Friend WithEvents TradeItemsSellGridView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents SplitContainerControl1 As DevExpress.XtraEditors.SplitContainerControl
    Friend WithEvents PanelControl1 As DevExpress.XtraEditors.PanelControl
    Friend WithEvents SplitContainerControl2 As DevExpress.XtraEditors.SplitContainerControl
    Friend WithEvents BuyC1Gridcontrol As DevExpress.XtraGrid.GridControl
    Friend WithEvents GridView1 As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents SellC1Gridcontrol As DevExpress.XtraGrid.GridControl
    Friend WithEvents GridView2 As DevExpress.XtraGrid.Views.Grid.GridView

#End Region

End Class
