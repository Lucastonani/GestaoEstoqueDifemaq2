Imports System.Drawing
Imports System.IO
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports Microsoft.Office.Interop.Excel

Public Class UcReposicaoEstoque
    Private powerQueryManager As PowerQueryManager
    Private produtoSelecionado As String = String.Empty
    Private Const DEBOUNCE_DELAY As Integer = 300 ' ms para evitar múltiplas chamadas
    Private debounceTimer As Timer

    Public Sub New()
        InitializeComponent()
        ConfigurarComponentes()
        InicializarDados()
    End Sub

    Private Sub ConfigurarComponentes()
        Try
            ' Aplicar estilização aos DataGridViews
            dgvProdutos.EstilizarDataGridView()
            dgvEstoque.EstilizarDataGridView()
            dgvCompras.EstilizarDataGridView()
            dgvVendas.EstilizarDataGridView()

            ' Configurar PictureBox
            ConfigurarPictureBox()

            ' Configurar timer para debounce
            debounceTimer = New Timer()
            debounceTimer.Interval = DEBOUNCE_DELAY
            AddHandler debounceTimer.Tick, AddressOf DebounceTimer_Tick

            ' Configurar eventos
            AddHandler dgvProdutos.SelectionChanged, AddressOf DgvProdutos_SelectionChanged
            AddHandler dgvProdutos.CellDoubleClick, AddressOf DgvProdutos_CellDoubleClick

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.ConfigurarComponentes")
            MessageBox.Show($"Erro ao configurar componentes: {ex.Message}", "Erro",
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub InicializarDados()
        Try
            powerQueryManager = New PowerQueryManager(Globals.ThisWorkbook)

            ' Mostrar cursor de espera
            Me.Cursor = Cursors.WaitCursor

            ' Atualizar dados do Power Query se necessário (descomente para atualização automática)
            ' AtualizarDadosPowerQuery()

            CarregarProdutos()

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.InicializarDados")
            MessageBox.Show($"Erro ao inicializar dados: {ex.Message}", "Erro",
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub AtualizarDadosPowerQuery()
        Try
            Me.Cursor = Cursors.WaitCursor
            lblProdutos.Text = "PRODUTOS - Atualizando..."

            powerQueryManager.AtualizarTodasConsultas()

            lblProdutos.Text = "PRODUTOS"

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.AtualizarDadosPowerQuery")
            MessageBox.Show($"Erro ao atualizar Power Query: {ex.Message}", "Erro",
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
            lblProdutos.Text = "PRODUTOS"
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub ConfigurarPictureBox()
        With pbProduto
            .SizeMode = PictureBoxSizeMode.Zoom
            .BorderStyle = BorderStyle.FixedSingle
            .BackColor = Color.White
            .BackgroundImage = Nothing
            .BackgroundImageLayout = ImageLayout.Center
        End With
    End Sub

    Private Sub CarregarProdutos()
        Try
            Dim tabelaProdutos As ListObject = powerQueryManager.ObterTabela(ConfiguracaoApp.TABELA_PRODUTOS)
            If tabelaProdutos Is Nothing Then
                MessageBox.Show($"Tabela '{ConfiguracaoApp.TABELA_PRODUTOS}' não encontrada!", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim dataTable As DataTable = DataHelper.ConvertListObjectToDataTable(tabelaProdutos)
            dgvProdutos.DataSource = dataTable

            ' Ajustar larguras das colunas
            AjustarColunasProdutos()

            ' Selecionar primeiro produto se existir
            If dgvProdutos.Rows.Count > 0 Then
                dgvProdutos.Rows(0).Selected = True
            End If

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.CarregarProdutos")
            MessageBox.Show($"Erro ao carregar produtos: {ex.Message}", "Erro",
                          MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AjustarColunasProdutos()
        Try
            If dgvProdutos.Columns.Count > 0 Then
                ' Primeira coluna (código) com largura fixa menor
                dgvProdutos.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                dgvProdutos.Columns(0).MinimumWidth = 80

                ' Outras colunas preenchem proporcionalmente
                For i As Integer = 1 To dgvProdutos.Columns.Count - 1
                    dgvProdutos.Columns(i).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                Next
            End If
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.AjustarColunasProdutos")
        End Try
    End Sub

    Private Sub DgvProdutos_SelectionChanged(sender As Object, e As EventArgs)
        Try
            ' Usar debounce para evitar múltiplas chamadas
            debounceTimer.Stop()
            debounceTimer.Start()

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.DgvProdutos_SelectionChanged")
        End Try
    End Sub

    Private Sub DebounceTimer_Tick(sender As Object, e As EventArgs)
        Try
            debounceTimer.Stop()

            If dgvProdutos.SelectedRows.Count > 0 Then
                Dim produtoSelecionadoRow As DataGridViewRow = dgvProdutos.SelectedRows(0)
                Dim codigoProduto As String = produtoSelecionadoRow.Cells(0).Value?.ToString() ?? ""
                
                If Not String.IsNullOrEmpty(codigoProduto) AndAlso codigoProduto <> produtoSelecionado Then
                    produtoSelecionado = codigoProduto
                    CarregarDadosProdutoAsync(codigoProduto)
                End If
            End If

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.DebounceTimer_Tick")
        End Try
    End Sub

    Private Sub DgvProdutos_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        Try
            If e.RowIndex >= 0 Then
                Dim produto As String = dgvProdutos.Rows(e.RowIndex).Cells(0).Value?.ToString() ?? ""
                Dim descricao As String = ""

                If dgvProdutos.Columns.Count > 1 Then
                    descricao = dgvProdutos.Rows(e.RowIndex).Cells(1).Value?.ToString() ?? ""
                End If

                MessageBox.Show($"Produto: {produto}{Environment.NewLine}Descrição: {descricao}",
                              "Detalhes do Produto", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.DgvProdutos_CellDoubleClick")
        End Try
    End Sub

    Private Sub CarregarDadosProdutoAsync(codigoProduto As String)
        Try
            ' Limpar dados anteriores
            LimparDadosSecundarios()

            ' Mostrar indicadores de carregamento
            MostrarIndicadoresCarregamento(True)

            ' Carregar dados de forma assíncrona
            Task.Run(Sub()
                         Try
                             ' Carregar estoque
                             Dim estoqueData = CarregarDadosFiltrados(ConfiguracaoApp.TABELA_ESTOQUE, codigoProduto)
                             Me.Invoke(Sub()
                                           dgvEstoque.DataSource = estoqueData
                                           lblEstoque.Text = $"ESTOQUE ({estoqueData.Rows.Count} registros)"
                                       End Sub)

                             ' Carregar compras
                             Dim comprasData = CarregarDadosFiltrados(ConfiguracaoApp.TABELA_COMPRAS, codigoProduto)
                             Me.Invoke(Sub()
                                           dgvCompras.DataSource = comprasData
                                           lblCompras.Text = $"COMPRAS ({comprasData.Rows.Count} registros)"
                                       End Sub)

                             ' Carregar vendas
                             Dim vendasData = CarregarDadosFiltrados(ConfiguracaoApp.TABELA_VENDAS, codigoProduto)
                             Me.Invoke(Sub()
                                           dgvVendas.DataSource = vendasData
                                           lblVendas.Text = $"VENDAS ({vendasData.Rows.Count} registros)"
                                       End Sub)

                             ' Carregar imagem
                             Me.Invoke(Sub() CarregarImagemProduto(codigoProduto))

                             ' Ocultar indicadores de carregamento
                             Me.Invoke(Sub() MostrarIndicadoresCarregamento(False))

                         Catch ex As Exception
                             LogErros.RegistrarErro(ex, $"UcReposicaoEstoque.CarregarDadosProdutoAsync({codigoProduto})")
                             Me.Invoke(Sub()
                                           MostrarIndicadoresCarregamento(False)
                                           MessageBox.Show($"Erro ao carregar dados do produto: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                       End Sub)
                         End Try
                     End Sub)

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.CarregarDadosProdutoAsync")
            MostrarIndicadoresCarregamento(False)
        End Try
    End Sub

    Private Sub LimparDadosSecundarios()
        Try
            dgvEstoque.DataSource = Nothing
            dgvCompras.DataSource = Nothing
            dgvVendas.DataSource = Nothing
            pbProduto.Image?.Dispose()
            pbProduto.Image = Nothing
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.LimparDadosSecundarios")
        End Try
    End Sub

    Private Sub MostrarIndicadoresCarregamento(mostrar As Boolean)
        Try
            If mostrar Then
                lblEstoque.Text = "ESTOQUE - Carregando..."
                lblCompras.Text = "COMPRAS - Carregando..."
                lblVendas.Text = "VENDAS - Carregando..."
                lblImagem.Text = "IMAGEM DO PRODUTO - Carregando..."
            Else
                lblImagem.Text = "IMAGEM DO PRODUTO"
            End If
        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.MostrarIndicadoresCarregamento")
        End Try
    End Sub

    Private Function CarregarDadosFiltrados(nomeTabela As String, codigoProduto As String) As DataTable
        Try
            Dim tabela As ListObject = powerQueryManager.ObterTabela(nomeTabela)
            If tabela Is Nothing Then Return New DataTable()

            Dim dataTable As DataTable = DataHelper.ConvertListObjectToDataTable(tabela)

            ' Filtrar por código do produto (assumindo que a primeira coluna é o código)
            If dataTable.Columns.Count > 0 Then
                Return DataHelper.FiltrarDataTable(dataTable, dataTable.Columns(0).ColumnName, codigoProduto)
            End If

            Return dataTable

        Catch ex As Exception
            LogErros.RegistrarErro(ex, $"UcReposicaoEstoque.CarregarDadosFiltrados({nomeTabela}, {codigoProduto})")
            Return New DataTable()
        End Try
    End Function

    Private Sub CarregarImagemProduto(codigoProduto As String)
        Try
            ' Limpar imagem anterior
            If pbProduto.Image IsNot Nothing Then
                pbProduto.Image.Dispose()
                pbProduto.Image = Nothing
            End If

            ' Procurar imagem com diferentes extensões
            Dim imagemEncontrada As Boolean = False

            For Each extensao As String In ConfiguracaoApp.EXTENSOES_IMAGEM
                Dim caminhoImagem As String = Path.Combine(ConfiguracaoApp.CAMINHO_IMAGENS, $"{codigoProduto}{extensao}")

                If File.Exists(caminhoImagem) Then
                    Try
                        ' Carregar imagem usando FileStream para evitar bloqueio do arquivo
                        Using fs As New FileStream(caminhoImagem, FileMode.Open, FileAccess.Read)
                            pbProduto.Image = Image.FromStream(fs)
                        End Using
                        imagemEncontrada = True
                        Exit For
                    Catch imgEx As Exception
                        LogErros.RegistrarErro(imgEx, $"UcReposicaoEstoque.CarregarImagemProduto - Erro ao carregar {caminhoImagem}")
                        Continue For
                    End Try
                End If
            Next

            If Not imagemEncontrada Then
                ' Verificar se o diretório existe e criar se necessário
                If Not Directory.Exists(ConfiguracaoApp.CAMINHO_IMAGENS) Then
                    Try
                        Directory.CreateDirectory(ConfiguracaoApp.CAMINHO_IMAGENS)
                    Catch dirEx As Exception
                        LogErros.RegistrarErro(dirEx, "UcReposicaoEstoque.CarregarImagemProduto - Erro ao criar diretório")
                    End Try
                End If

                pbProduto.Image = Nothing
            End If

        Catch ex As Exception
            LogErros.RegistrarErro(ex, $"UcReposicaoEstoque.CarregarImagemProduto({codigoProduto})")
            pbProduto.Image = Nothing
        End Try
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing Then
                ' Limpar recursos
                debounceTimer?.Stop()
                debounceTimer?.Dispose()

                If pbProduto?.Image IsNot Nothing Then
                    pbProduto.Image.Dispose()
                End If

                powerQueryManager = Nothing
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

End Class

Partial Public Class UcReposicaoEstoque

    Private Sub ConfigurarColunasDataGridViews()
        Try
            ' Configurar colunas do dgvProdutos após carregar dados
            If dgvProdutos.DataSource IsNot Nothing AndAlso dgvProdutos.Columns.Count > 0 Then
                dgvProdutos.ConfigurarColunas(
                    New ColumnConfig(0, "Código", 100),
                    New ColumnConfig(1, "Descrição", 300),
                    New ColumnConfig(2, "Categoria", 150),
                    New ColumnConfig(3, "Unidade", 80),
                    New ColumnConfig(4, "Preço", 100, True, DataGridViewContentAlignment.MiddleRight)
                )
            End If

            ' Configurar colunas do dgvEstoque
            If dgvEstoque.DataSource IsNot Nothing AndAlso dgvEstoque.Columns.Count > 0 Then
                dgvEstoque.ConfigurarColunas(
                    New ColumnConfig(0, "Produto", 120),
                    New ColumnConfig(1, "Local", 150),
                    New ColumnConfig(2, "Quantidade", 100, True, DataGridViewContentAlignment.MiddleRight),
                    New ColumnConfig(3, "Mín.", 80, True, DataGridViewContentAlignment.MiddleRight),
                    New ColumnConfig(4, "Máx.", 80, True, DataGridViewContentAlignment.MiddleRight)
                )
            End If

            ' Configurar colunas do dgvCompras
            If dgvCompras.DataSource IsNot Nothing AndAlso dgvCompras.Columns.Count > 0 Then
                dgvCompras.ConfigurarColunas(
                    New ColumnConfig(0, "Produto", 120),
                    New ColumnConfig(1, "Data", 100),
                    New ColumnConfig(2, "Fornecedor", 200),
                    New ColumnConfig(3, "Qtd.", 80, True, DataGridViewContentAlignment.MiddleRight),
                    New ColumnConfig(4, "Valor Unit.", 100, True, DataGridViewContentAlignment.MiddleRight),
                    New ColumnConfig(5, "Total", 100, True, DataGridViewContentAlignment.MiddleRight)
                )
            End If

            ' Configurar colunas do dgvVendas
            If dgvVendas.DataSource IsNot Nothing AndAlso dgvVendas.Columns.Count > 0 Then
                dgvVendas.ConfigurarColunas(
                    New ColumnConfig(0, "Produto", 120),
                    New ColumnConfig(1, "Data", 100),
                    New ColumnConfig(2, "Cliente", 200),
                    New ColumnConfig(3, "Qtd.", 80, True, DataGridViewContentAlignment.MiddleRight),
                    New ColumnConfig(4, "Valor Unit.", 100, True, DataGridViewContentAlignment.MiddleRight),
                    New ColumnConfig(5, "Total", 100, True, DataGridViewContentAlignment.MiddleRight)
                )
            End If

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.ConfigurarColunasDataGridViews")
        End Try
    End Sub

    ' Método para atualizar as configurações após carregar dados
    Private Sub AplicarConfiguracoes()
        Try
            ConfigurarColunasDataGridViews()

            ' Log da operação
            LogErros.RegistrarInfo($"Dados carregados para produto: {produtoSelecionado}", "UcReposicaoEstoque.AplicarConfiguracoes")

        Catch ex As Exception
            LogErros.RegistrarErro(ex, "UcReposicaoEstoque.AplicarConfiguracoes")
        End Try
    End Sub

End Class