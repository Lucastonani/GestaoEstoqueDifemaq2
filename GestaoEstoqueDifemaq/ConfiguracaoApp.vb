Public Class ConfiguracaoApp

    ' Nomes das tabelas do Power Query
    Public Const TABELA_PRODUTOS As String = "tblProdutos"
    Public Const TABELA_ESTOQUE As String = "tblEstoqueVisao"
    Public Const TABELA_COMPRAS As String = "tblCompras"
    Public Const TABELA_VENDAS As String = "tblVendas"

    ' Configurações de imagens
    Public Const CAMINHO_IMAGENS As String = "C:\ImagesEstoque"
    Public Shared ReadOnly EXTENSOES_IMAGEM As String() = {".jpg", ".jpeg", ".png", ".bmp", ".gif"}

    ' Configurações de interface
    Public Const COR_HEADER_GRID As String = "#2E86AB"
    Public Const COR_LINHA_ALTERNADA As String = "#F5F5F5"
    Public Const ALTURA_LINHA_GRID As Integer = 25

    ' Configurações de sistema
    Public Const TIMEOUT_POWERQUERY As Integer = 60 ' segundos
    Public Const DEBOUNCE_DELAY As Integer = 300 ' milissegundos

    ' Configurações de log
    Public Const CAMINHO_LOG As String = "C:\Logs\GestaoEstoque"
    Public Const TAMANHO_MAX_LOG As Long = 10485760 ' 10MB

End Class