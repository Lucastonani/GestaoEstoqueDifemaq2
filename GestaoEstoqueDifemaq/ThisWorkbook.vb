Public Class ThisWorkbook
    Private mainForm As MainForm
    Private WithEvents appEvents As Excel.Application

    Private Sub ThisWorkbook_Startup() Handles Me.Startup
        Try
            ' Configurar eventos da aplicação
            appEvents = Me.Application

            ' Configurações da aplicação Excel
            ConfigurarAplicacaoExcel()

            ' Criar e exibir o formulário principal
            InicializarFormularioPrincipal()

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "ThisWorkbook.ThisWorkbook_Startup")
            MessageBox.Show($"Erro ao inicializar aplicação: {ex.Message}", "Erro",
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ConfigurarAplicacaoExcel()
        Try
            With Me.Application
                .Visible = False
                .WindowState = Excel.XlWindowState.xlMinimized
                .DisplayAlerts = False
                .ScreenUpdating = False
                .EnableEvents = False
                .Calculation = Excel.XlCalculation.xlCalculationManual
            End With
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "ThisWorkbook.ConfigurarAplicacaoExcel")
        End Try
    End Sub

    Private Sub InicializarFormularioPrincipal()
        Try
            mainForm = New MainForm()
            mainForm.Show()

            ' Trazer formulário para frente
            mainForm.WindowState = FormWindowState.Normal
            mainForm.BringToFront()
            mainForm.Activate()

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "ThisWorkbook.InicializarFormularioPrincipal")
            Throw
        End Try
    End Sub

    Private Sub ThisWorkbook_Shutdown() Handles Me.Shutdown
        Try
            ' Fechar formulário principal
            If mainForm IsNot Nothing AndAlso Not mainForm.IsDisposed Then
                mainForm.Close()
                mainForm.Dispose()
            End If

            ' Restaurar configurações do Excel
            RestaurarConfiguracoes()

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "ThisWorkbook.ThisWorkbook_Shutdown")
        End Try
    End Sub

    Private Sub RestaurarConfiguracoes()
        Try
            If Me.Application IsNot Nothing Then
                With Me.Application
                    .Visible = True
                    .WindowState = Excel.XlWindowState.xlNormal
                    .DisplayAlerts = True
                    .ScreenUpdating = True
                    .EnableEvents = True
                    .Calculation = Excel.XlCalculation.xlCalculationAutomatic
                End With
            End If
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "ThisWorkbook.RestaurarConfiguracoes")
        End Try
    End Sub

    Private Sub AppEvents_WorkbookBeforeClose(Wb As Workbook, ByRef Cancel As Boolean) Handles appEvents.WorkbookBeforeClose
        Try
            If Wb Is Me Then
                ' Fechar aplicação quando workbook for fechado
                If mainForm IsNot Nothing AndAlso Not mainForm.IsDisposed Then
                    mainForm.Close()
                End If
            End If
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "ThisWorkbook.AppEvents_WorkbookBeforeClose")
        End Try
    End Sub

End Class