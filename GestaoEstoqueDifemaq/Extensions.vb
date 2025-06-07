Imports System.Runtime.CompilerServices
Imports System.Drawing
Imports System.Windows.Forms

Public Module Extensions
    <Extension()>
    Public Sub EstilizarDataGridView(dgv As DataGridView)
        Try
            With dgv
                ' Configurações gerais
                .AllowUserToAddRows = False
                .AllowUserToDeleteRows = False
                .AllowUserToResizeRows = False
                .ReadOnly = True
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
                .MultiSelect = False
                .AutoGenerateColumns = True
                .BorderStyle = BorderStyle.Fixed3D
                .EnableHeadersVisualStyles = False

                ' Configurações visuais
                .BackgroundColor = Color.White
                .GridColor = Color.LightGray
                .RowHeadersVisible = False
                .ColumnHeadersHeight = 35
                .RowTemplate.Height = ConfiguracaoApp.ALTURA_LINHA_GRID

                ' Estilo do cabeçalho
                With .ColumnHeadersDefaultCellStyle
                    .BackColor = ColorTranslator.FromHtml(ConfiguracaoApp.COR_HEADER_GRID)
                    .ForeColor = Color.White
                    .Font = New Font("Segoe UI", 9, FontStyle.Bold)
                    .Alignment = DataGridViewContentAlignment.MiddleLeft
                End With

                ' Estilo das células
                With .DefaultCellStyle
                    .BackColor = Color.White
                    .ForeColor = Color.Black
                    .Font = New Font("Segoe UI", 9)
                    .SelectionBackColor = Color.CornflowerBlue
                    .SelectionForeColor = Color.White
                    .Alignment = DataGridViewContentAlignment.MiddleLeft
                    .Padding = New Padding(5, 2, 5, 2)
                End With

                ' Estilo das linhas alternadas
                With .AlternatingRowsDefaultCellStyle
                    .BackColor = ColorTranslator.FromHtml(ConfiguracaoApp.COR_LINHA_ALTERNADA)
                End With
            End With
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "Extensions.EstilizarDataGridView")
        End Try
    End Sub

    <Extension()>
    Public Sub ConfigurarColunas(dgv As DataGridView, ParamArray configuracoes As ColumnConfig())
        Try
            If configuracoes Is Nothing OrElse configuracoes.Length = 0 Then Return

            For Each config As ColumnConfig In configuracoes
                If dgv.Columns.Count > config.Index Then
                    With dgv.Columns(config.Index)
                        .HeaderText = config.HeaderText
                        .Width = config.Width
                        .Visible = config.Visible
                        .ReadOnly = config.ReadOnly

                        If config.Alignment.HasValue Then
                            .DefaultCellStyle.Alignment = config.Alignment.Value
                        End If

                        If Not String.IsNullOrEmpty(config.Format) Then
                            .DefaultCellStyle.Format = config.Format
                        End If
                    End With
                End If
            Next
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "Extensions.ConfigurarColunas")
        End Try
    End Sub
End Module