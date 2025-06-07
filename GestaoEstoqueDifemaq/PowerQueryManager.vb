Imports Microsoft.Office.Interop.Excel

Public Class PowerQueryManager
    Private workbook As Workbook
    Private ReadOnly timeoutSegundos As Integer = 60

    Public Sub New(wb As Workbook)
        If wb Is Nothing Then
            Throw New ArgumentNullException("workbook", "Workbook não pode ser nulo")
        End If
        workbook = wb
    End Sub

    Public Sub AtualizarTodasConsultas()
        Try
            Dim startTime As DateTime = DateTime.Now

            ' Desabilitar alertas temporariamente
            Dim alertasAnteriores As Boolean = workbook.Application.DisplayAlerts
            workbook.Application.DisplayAlerts = False

            Try
                ' Atualizar todas as conexões
                For Each connection As WorkbookConnection In workbook.Connections
                    Try
                        If EhConnectionPowerQuery(connection) Then
                            connection.Refresh()
                        End If
                    Catch connEx As Exception
                        LogErros.RegistrarErro(connEx, $"PowerQueryManager.AtualizarTodasConsultas - Erro na conexão {connection.Name}")
                    End Try
                Next

                ' Aguardar conclusão com timeout
                AguardarConclusaoAtualizacao(startTime)

            Finally
                ' Restaurar configuração de alertas
                workbook.Application.DisplayAlerts = alertasAnteriores
            End Try

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "PowerQueryManager.AtualizarTodasConsultas")
            Throw New Exception($"Erro ao atualizar consultas Power Query: {ex.Message}")
        End Try
    End Sub

    Private Function EhConnectionPowerQuery(connection As WorkbookConnection) As Boolean
        Try
            Return connection.Type = XlConnectionType.xlConnectionTypeOLEDB OrElse
                   connection.Type = XlConnectionType.xlConnectionTypeODBC OrElse
                   connection.Type = XlConnectionType.xlConnectionTypeTEXT OrElse
                   connection.Type = XlConnectionType.xlConnectionTypeWEB
        Catch
            Return False
        End Try
    End Function

    Private Sub AguardarConclusaoAtualizacao(startTime As DateTime)
        Try
            Do While workbook.Application.CalculationState <> XlCalculationState.xlDone
                Application.DoEvents()
                Threading.Thread.Sleep(100)

                ' Verificar timeout
                If DateTime.Now.Subtract(startTime).TotalSeconds > timeoutSegundos Then
                    Throw New TimeoutException($"Timeout de {timeoutSegundos} segundos excedido ao aguardar atualização das consultas")
                End If
            Loop
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "PowerQueryManager.AguardarConclusaoAtualizacao")
            Throw
        End Try
    End Sub

    Public Function ObterTabela(nomeTabela As String) As ListObject
        Try
            If String.IsNullOrEmpty(nomeTabela) Then
                Return Nothing
            End If

            For Each worksheet As Worksheet In workbook.Worksheets
                Try
                    For Each tabela As ListObject In worksheet.ListObjects
                        If tabela.Name.Equals(nomeTabela, StringComparison.OrdinalIgnoreCase) Then
                            Return tabela
                        End If
                    Next
                Catch wsEx As Exception
                    LogErros.RegistrarErro(wsEx, $"PowerQueryManager.ObterTabela - Erro na planilha {worksheet.Name}")
                    Continue For
                End Try
            Next

            Return Nothing

        Catch ex As Exception
            LogErros.RegistrarErro(ex, $"PowerQueryManager.ObterTabela({nomeTabela})")
            Return Nothing
        End Try
    End Function

    Public Function ListarTabelas() As List(Of String)
        Try
            Dim tabelas As New List(Of String)()

            For Each worksheet As Worksheet In workbook.Worksheets
                Try
                    For Each tabela As ListObject In worksheet.ListObjects
                        tabelas.Add(tabela.Name)
                    Next
                Catch wsEx As Exception
                    LogErros.RegistrarErro(wsEx, $"PowerQueryManager.ListarTabelas - Erro na planilha {worksheet.Name}")
                    Continue For
                End Try
            Next

            Return tabelas

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "PowerQueryManager.ListarTabelas")
            Return New List(Of String)()
        End Try
    End Function

    Public Function VerificarStatusConexoes() As Dictionary(Of String, String)
        Try
            Dim status As New Dictionary(Of String, String)()

            For Each connection As WorkbookConnection In workbook.Connections
                Try
                    status.Add(connection.Name, "Ativa")
                Catch connEx As Exception
                    status.Add(connection.Name, $"Erro: {connEx.Message}")
                End Try
            Next

            Return status

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "PowerQueryManager.VerificarStatusConexoes")
            Return New Dictionary(Of String, String)()
        End Try
    End Function

End Class