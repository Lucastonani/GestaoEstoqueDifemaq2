Imports Microsoft.Office.Interop.Excel

Public Class DataHelper
    
    Public Shared Function ConvertListObjectToDataTable(listObject As ListObject) As DataTable
        Try
            Dim dataTable As New DataTable()
            
            If listObject Is Nothing Then
                Return dataTable
            End If
            
            ' Criar colunas baseadas no cabeçalho
            Dim headerRange As Range = listObject.HeaderRowRange
            If headerRange IsNot Nothing Then
                For col As Integer = 1 To headerRange.Columns.Count
                    Dim columnName As String = headerRange.Cells(1, col).Value?.ToString() ?? $"Coluna{col}"
                    dataTable.Columns.Add(columnName)
                Next
            End If
            
            ' Adicionar dados do corpo da tabela
            Dim dataBodyRange As Range = listObject.DataBodyRange
            If dataBodyRange IsNot Nothing Then
                For row As Integer = 1 To dataBodyRange.Rows.Count
                    Dim dataRow As DataRow = dataTable.NewRow()
                    For col As Integer = 1 To Math.Min(dataBodyRange.Columns.Count, dataTable.Columns.Count)
                        Dim cellValue = dataBodyRange.Cells(row, col).Value
                        dataRow(col - 1) = If(cellValue Is Nothing, "", cellValue.ToString())
                    Next
                    dataTable.Rows.Add(dataRow)
                Next
            End If
            
            Return dataTable
            
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "DataHelper.ConvertListObjectToDataTable")
            Throw New Exception($"Erro ao converter ListObject para DataTable: {ex.Message}")
        End Try
    End Function
    
    Public Shared Function ConvertRangeToDataTable(range As Range, Optional hasHeaders As Boolean = True) As DataTable
        Try
            Dim dataTable As New DataTable()
            
            If range Is Nothing OrElse range.Rows.Count = 0 Then
                Return dataTable
            End If
            
            Dim startRow As Integer = 1
            
            ' Criar colunas
            If hasHeaders Then
                For col As Integer = 1 To range.Columns.Count
                    Dim columnName As String = range.Cells(1, col).Value?.ToString() ?? $"Coluna{col}"
                    dataTable.Columns.Add(columnName)
                Next
                startRow = 2
            Else
                For col As Integer = 1 To range.Columns.Count
                    dataTable.Columns.Add($"Coluna{col}")
                Next
            End If
            
            ' Adicionar dados
            For row As Integer = startRow To range.Rows.Count
                Dim dataRow As DataRow = dataTable.NewRow()
                For col As Integer = 1 To range.Columns.Count
                    Dim cellValue = range.Cells(row, col).Value
                    dataRow(col - 1) = If(cellValue Is Nothing, "", cellValue.ToString())
                Next
                dataTable.Rows.Add(dataRow)
            Next
            
            Return dataTable
            
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "DataHelper.ConvertRangeToDataTable")
            Throw New Exception($"Erro ao converter Range para DataTable: {ex.Message}")
        End Try
    End Function
    
    Public Shared Function FiltrarDataTable(dataTable As DataTable, coluna As String, valor As String) As DataTable
        Try
            If dataTable Is Nothing OrElse String.IsNullOrEmpty(coluna) Then
                Return New DataTable()
            End If
            
            Dim tabelaFiltrada As DataTable = dataTable.Clone()
            
            ' Verificar se a coluna existe
            If Not dataTable.Columns.Contains(coluna) Then
                Return tabelaFiltrada
            End If
            
            For Each row As DataRow In dataTable.Rows
                If row(coluna)?.ToString().Equals(valor, StringComparison.OrdinalIgnoreCase) = True Then
                    tabelaFiltrada.ImportRow(row)
                End If
            Next
            
            Return tabelaFiltrada
            
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "DataHelper.FiltrarDataTable")
            Throw New Exception($"Erro ao filtrar DataTable: {ex.Message}")
        End Try
    End Function
    
    Public Shared Function FiltrarDataTableMultiplosValores(dataTable As DataTable, coluna As String, valores As String()) As DataTable
        Try
            If dataTable Is Nothing OrElse String.IsNullOrEmpty(coluna) OrElse valores Is Nothing Then
                Return New DataTable()
            End If
            
            Dim tabelaFiltrada As DataTable = dataTable.Clone()
            
            If Not dataTable.Columns.Contains(coluna) Then
                Return tabelaFiltrada
            End If
            
            For Each row As DataRow In dataTable.Rows
                Dim valorCelula As String = row(coluna)?.ToString() ?? ""
                If valores.Any(Function(v) valorCelula.Equals(v, StringComparison.OrdinalIgnoreCase)) Then
                    tabelaFiltrada.ImportRow(row)
                End If
            Next
            
            Return tabelaFiltrada
            
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "DataHelper.FiltrarDataTableMultiplosValores")
            Throw New Exception($"Erro ao filtrar DataTable com múltiplos valores: {ex.Message}")
        End Try
    End Function
    
End Class