Public Class ColumnConfig
    Public Property Index As Integer
    Public Property HeaderText As String
    Public Property Width As Integer
    Public Property Visible As Boolean = True
    Public Property ReadOnly As Boolean = True
    Public Property Alignment As DataGridViewContentAlignment?
    Public Property Format As String

    Public Sub New(index As Integer, headerText As String, width As Integer)
        Me.Index = index
        Me.HeaderText = headerText
        Me.Width = width
    End Sub

    Public Sub New(index As Integer, headerText As String, width As Integer, visible As Boolean)
        Me.New(index, headerText, width)
        Me.Visible = visible
    End Sub

    Public Sub New(index As Integer, headerText As String, width As Integer, visible As Boolean, alignment As DataGridViewContentAlignment)
        Me.New(index, headerText, width, visible)
        Me.Alignment = alignment
    End Sub

End Class