Imports System.Drawing

Public Module ExtensaoDataGridView
    <Runtime.CompilerServices.Extension()>
    Public Sub EstilizarDataGridView(dgv As DataGridView)
        With dgv
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .ReadOnly = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            .RowHeadersVisible = False
            .BackgroundColor = Color.White
            .BorderStyle = BorderStyle.Fixed3D
            .GridColor = Color.LightGray
            .AlternatingRowsDefaultCellStyle.BackColor = ConfiguracaoApp.COR_ALTERNADA
            .DefaultCellStyle.SelectionBackColor = ConfiguracaoApp.COR_SELECAO
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .ColumnHeadersDefaultCellStyle.BackColor = ConfiguracaoApp.COR_HEADER
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Arial", 9, FontStyle.Bold)
            .EnableHeadersVisualStyles = False
            .RowTemplate.Height = 25
        End With
    End Sub
End Module