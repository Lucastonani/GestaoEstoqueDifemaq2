Imports System.Drawing
Imports System.Windows.Forms

Public Class MainForm
    Private ucReposicaoEstoque As UcReposicaoEstoque

    Public Sub New()
        InitializeComponent()
        ConfigurarFormulario()
        CarregarUserControlInicial()
    End Sub

    Private Sub ConfigurarFormulario()
        Try
            ' Configurações do formulário
            Me.Text = "Gestão de Estoque - Difemaq"
            Me.Size = New Size(1400, 900)
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.WindowState = FormWindowState.Maximized
            Me.MinimumSize = New Size(1200, 800)

            ' Ícone do formulário (se existir)
            Try
                Me.Icon = My.Resources.AppIcon
            Catch
                ' Ícone não encontrado, continuar sem
            End Try

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "MainForm.ConfigurarFormulario")
        End Try
    End Sub

    Private Sub CarregarUserControlInicial()
        Try
            ' Carregar UserControl de Reposição de Estoque como padrão
            CarregarUserControl(GetType(UcReposicaoEstoque))

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "MainForm.CarregarUserControlInicial")
            MessageBox.Show($"Erro ao carregar interface inicial: {ex.Message}", "Erro",
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub CarregarUserControl(tipoUserControl As Type)
        Try
            ' Limpar painel atual
            pnlConteudo.Controls.Clear()

            ' Criar instância do UserControl
            Dim novoUserControl As UserControl = CType(Activator.CreateInstance(tipoUserControl), UserControl)

            ' Configurar e adicionar ao painel
            With novoUserControl
                .Dock = DockStyle.Fill
                .Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
            End With

            pnlConteudo.Controls.Add(novoUserControl)

            ' Armazenar referência se for UcReposicaoEstoque
            If TypeOf novoUserControl Is UcReposicaoEstoque Then
                ucReposicaoEstoque = CType(novoUserControl, UcReposicaoEstoque)
            End If

        Catch ex As Exception
            LogErros.RegistrarErro(ex, $"MainForm.CarregarUserControl({tipoUserControl.Name})")
            MessageBox.Show($"Erro ao carregar módulo: {ex.Message}", "Erro",
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            ' Confirmar fechamento
            Dim resultado As DialogResult = MessageBox.Show(
                "Deseja realmente fechar o sistema?",
                "Confirmar Fechamento",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2)

            If resultado = DialogResult.No Then
                e.Cancel = True
                Return
            End If

            ' Limpar recursos
            LimparRecursos()

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "MainForm.MainForm_FormClosing")
        End Try
    End Sub

    Private Sub LimparRecursos()
        Try
            ' Limpar UserControls
            For Each control As Control In pnlConteudo.Controls
                If TypeOf control Is UserControl Then
                    control.Dispose()
                End If
            Next

            pnlConteudo.Controls.Clear()

            ' Limpar referências
            ucReposicaoEstoque = Nothing

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "MainForm.LimparRecursos")
        End Try
    End Sub

End Class

Partial Public Class MainForm

    Private timerStatusBar As Timer

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        ' Configurar timer para atualizar data/hora
        timerStatusBar = New Timer()
        timerStatusBar.Interval = 1000 ' 1 segundo
        AddHandler timerStatusBar.Tick, AddressOf TimerStatusBar_Tick
        timerStatusBar.Start()

        ' Atualizar status inicial
        AtualizarStatus("Pronto")
    End Sub

    Private Sub TimerStatusBar_Tick(sender As Object, e As EventArgs)
        lblDataHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
    End Sub

    Public Sub AtualizarStatus(mensagem As String)
        Try
            lblStatus.Text = mensagem
            StatusStrip.Refresh()
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "MainForm.AtualizarStatus")
        End Try
    End Sub

    Private Sub btnReposicaoEstoque_Click(sender As Object, e As EventArgs) Handles btnReposicaoEstoque.Click
        Try
            AtualizarStatus("Carregando Reposição de Estoque...")
            HighlightButtonMenu(btnReposicaoEstoque)
            CarregarUserControl(GetType(UcReposicaoEstoque))
            AtualizarStatus("Pronto")
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "MainForm.btnReposicaoEstoque_Click")
            AtualizarStatus("Erro ao carregar módulo")
        End Try
    End Sub

    Private Sub btnRelatorios_Click(sender As Object, e As EventArgs) Handles btnRelatorios.Click
        Try
            AtualizarStatus("Módulo de Relatórios em desenvolvimento...")
            HighlightButtonMenu(btnRelatorios)
            MessageBox.Show("Módulo de Relatórios será implementado em versão futura.", "Informação",
                          MessageBoxButtons.OK, MessageBoxIcon.Information)
            AtualizarStatus("Pronto")
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "MainForm.btnRelatorios_Click")
        End Try
    End Sub

    Private Sub btnConfiguracoes_Click(sender As Object, e As EventArgs) Handles btnConfiguracoes.Click
        Try
            AtualizarStatus("Módulo de Configurações em desenvolvimento...")
            HighlightButtonMenu(btnConfiguracoes)
            MessageBox.Show("Módulo de Configurações será implementado em versão futura.", "Informação",
                          MessageBoxButtons.OK, MessageBoxIcon.Information)
            AtualizarStatus("Pronto")
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "MainForm.btnConfiguracoes_Click")
        End Try
    End Sub

    Private Sub HighlightButtonMenu(botaoAtivo As Button)
        Try
            ' Resetar cores de todos os botões
            For Each control As Control In pnlMenu.Controls
                If TypeOf control Is Button AndAlso control IsNot botaoAtivo Then
                    control.BackColor = Color.Transparent
                End If
            Next

            ' Destacar botão ativo
            botaoAtivo.BackColor = ColorTranslator.FromHtml(ConfiguracaoApp.COR_HEADER_GRID)

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "MainForm.HighlightButtonMenu")
        End Try
    End Sub

    Private Sub btnMinimizar_Click(sender As Object, e As EventArgs) Handles btnMinimizar.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub btnMaximizar_Click(sender As Object, e As EventArgs) Handles btnMaximizar.Click
        If Me.WindowState = FormWindowState.Maximized Then
            Me.WindowState = FormWindowState.Normal
            btnMaximizar.Text = "□"
        Else
            Me.WindowState = FormWindowState.Maximized
            btnMaximizar.Text = "❐"
        End If
    End Sub

    Private Sub btnFechar_Click(sender As Object, e As EventArgs) Handles btnFechar.Click
        Me.Close()
    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        Try
            If timerStatusBar IsNot Nothing Then
                timerStatusBar.Stop()
                timerStatusBar.Dispose()
            End If
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "MainForm.OnFormClosed")
        Finally
            MyBase.OnFormClosed(e)
        End Try
    End Sub

End Class