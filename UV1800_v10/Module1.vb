Imports System.Drawing
Imports System.IO
Imports System.Reflection.Emit
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms
Imports Label = System.Windows.Forms.Label


' ==========================================================================
' 設定・ログ表示ウィンドウ (Module1より先に定義することで参照エラーを回避)
' ==========================================================================
Public Class SettingsForm
    Inherits Form

    Public Property TotalDurationHours As Double = 12.0
    Public Property IntervalMinutes As Integer = 10

    Private lblTitle As Label
    Private lblHours As Label
    Private nudHours As NumericUpDown
    Private lblInterval As Label
    Private nudInterval As NumericUpDown
    Private lblStatus As Label
    Private btnStart As Button
    Private btnStop As Button
    Private btnBS As Button
    Private txtLog As RichTextBox

    Public Sub New()
        InitializeComponents()
    End Sub

    Private Sub InitializeComponents()
        Me.Text = "UV-1800 測定設定"
        Me.Size = New Size(800, 800)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.FromArgb(30, 30, 40)

        lblTitle = New Label()
        lblTitle.Text = "UV-1800 連続測定システム v10"
        lblTitle.ForeColor = Color.FromArgb(100, 200, 255)
        lblTitle.Font = New Font("Consolas", 10, FontStyle.Bold)
        lblTitle.Location = New Point(20, 15)
        lblTitle.Size = New Size(400, 20)

        lblHours = New Label()
        lblHours.Text = "総測定時間 (時間):"
        lblHours.ForeColor = Color.White
        lblHours.Font = New Font("Consolas", 10)
        lblHours.Location = New Point(20, 50)
        lblHours.Size = New Size(165, 20)

        nudHours = New NumericUpDown()
        nudHours.Location = New Point(200, 47)
        nudHours.Size = New Size(100, 15)
        nudHours.Font = New Font("Console", 10)
        nudHours.Minimum = 1
        nudHours.Maximum = 72
        nudHours.Value = 12
        nudHours.DecimalPlaces = 1
        nudHours.Increment = 0.5
        nudHours.BackColor = Color.FromArgb(50, 50, 65)
        nudHours.ForeColor = Color.White

        lblInterval = New Label()
        lblInterval.Text = "測定間隔 (分):"
        lblInterval.ForeColor = Color.White
        lblInterval.Font = New Font("Consolas", 10)
        lblInterval.Location = New Point(20, 85)
        lblInterval.Size = New Size(165, 20)

        nudInterval = New NumericUpDown()
        nudInterval.Location = New Point(200, 82)
        nudInterval.Size = New Size(100, 15)
        nudInterval.Font = New Font("Console", 10)
        nudInterval.Minimum = 1
        nudInterval.Maximum = 60
        nudInterval.Value = 10
        nudInterval.BackColor = Color.FromArgb(50, 50, 65)
        nudInterval.ForeColor = Color.White

        lblStatus = New Label()
        lblStatus.Text = "設定を確認してSTARTを押してください"
        lblStatus.ForeColor = Color.FromArgb(180, 180, 180)
        lblStatus.Font = New Font("Consolas", 12)
        lblStatus.Location = New Point(20, 118)
        lblStatus.Size = New Size(300, 20)

        btnStart = New Button()
        btnStart.Text = "START"
        btnStart.Location = New Point(20, 145)
        btnStart.Size = New Size(130, 45)
        btnStart.BackColor = Color.FromArgb(40, 160, 80)
        btnStart.ForeColor = Color.White
        btnStart.Font = New Font("Consolas", 12, FontStyle.Bold)
        btnStart.FlatStyle = FlatStyle.Flat
        btnStart.FlatAppearance.BorderColor = Color.FromArgb(60, 200, 100)
        AddHandler btnStart.Click, AddressOf BtnStart_Click

        btnStop = New Button()
        btnStop.Text = "STOP"
        btnStop.Location = New Point(175, 145)
        btnStop.Size = New Size(130, 45)
        btnStop.BackColor = Color.FromArgb(160, 40, 40)
        btnStop.ForeColor = Color.Black
        btnStop.Font = New Font("Consolas", 12, FontStyle.Bold)
        btnStop.FlatStyle = FlatStyle.Flat
        btnStop.FlatAppearance.BorderColor = Color.FromArgb(200, 60, 60)
        btnStop.Enabled = False
        AddHandler btnStop.Click, AddressOf BtnStop_Click

        btnBS = New Button()
        btnBS.Text = "Baseline"
        btnBS.Location = New Point(350, 47)
        btnBS.Size = New Size(100, 58)
        btnBS.BackColor = Color.FromArgb(230, 200, 0)
        btnBS.ForeColor = Color.Black
        btnBS.Font = New Font("Consolas", 12, FontStyle.Bold)
        btnBS.FlatStyle = FlatStyle.Flat
        btnBS.FlatAppearance.BorderColor = Color.FromArgb(220, 181, 0)
        AddHandler btnBS.Click, AddressOf BtnBS_Click


        Dim lblLog As New Label()
        lblLog.Text = "通信ログ"
        lblLog.ForeColor = Color.FromArgb(150, 150, 150)
        lblLog.Font = New Font("Consolas", 10)
        lblLog.Location = New Point(20, 200)
        lblLog.Size = New Size(100, 15)

        txtLog = New RichTextBox()
        txtLog.Location = New Point(20, 230)
        txtLog.Size = New Size(690, 448)
        txtLog.BackColor = Color.FromArgb(15, 15, 20)
        txtLog.ForeColor = Color.FromArgb(180, 255, 180)
        txtLog.Font = New Font("Consolas", 12)
        txtLog.ReadOnly = True
        txtLog.ScrollBars = RichTextBoxScrollBars.Vertical
        txtLog.BorderStyle = BorderStyle.FixedSingle

        Me.Controls.AddRange(New Control() {
            lblTitle,
            lblHours, nudHours,
            lblInterval, nudInterval,
            lblStatus,
            btnStart, btnStop, btnBS,
            lblLog, txtLog
        })
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As EventArgs)
        TotalDurationHours = CDbl(nudHours.Value)
        IntervalMinutes = CInt(nudInterval.Value)
        btnStart.Enabled = False
        nudHours.Enabled = False
        nudInterval.Enabled = False
        EnableStopButton()

        Dim measureThread As New Thread(
            Sub()
                Module1.RunSession(TotalDurationHours, IntervalMinutes)
            End Sub)
        measureThread.IsBackground = True
        measureThread.Start()
    End Sub

    Private Sub BtnStop_Click(sender As Object, e As EventArgs)
        Dim ans As DialogResult = MessageBox.Show(
            "測定を強制終了しますか？" & Environment.NewLine &
            "現在のサイクルは保存されません。",
            "強制終了の確認",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning)
        If ans = DialogResult.Yes Then
            Module1.RequestStop()
            btnStop.Enabled = False
            UpdateStatus("停止処理中... 現在のステップ完了後に終了します")
        End If
    End Sub

    Private Sub BtnBS_Click(sender As Object, e As EventArgs)
        ' 誤操作防止のためUIを無効化
        btnStart.Enabled = False
        btnBS.Enabled = False
        nudHours.Enabled = False
        nudInterval.Enabled = False
        UpdateStatus("ベースライン測定中...")

        ' 通信処理を別スレッドで実行
        Dim bsThread As New Thread(
            Sub()
                ' COMポートをオープン
                Dim hCom As IntPtr = CreateFile(Module1.PORT_NAME, GENERIC_READ Or GENERIC_WRITE, 0, IntPtr.Zero,
                                                 OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero)

                If hCom = INVALID_HANDLE_VALUE Then
                    Module1.Log("ERROR: ポートオープン失敗。")
                    Me.Invoke(Sub() RestoreUI("ポートオープン失敗"))
                    Return
                End If

                ' 新しく追加したベースライン関数を呼び出す
                Dim result As Boolean = Module1.RunBaseline(hCom)

                CloseHandle(hCom)

                ' 結果に応じてUIを復帰
                Me.Invoke(Sub()
                              If result Then
                                  RestoreUI("ベースライン完了。STARTを押せます。")
                              Else
                                  RestoreUI("ベースライン失敗。再度試してください。")
                              End If
                          End Sub)
            End Sub)

        bsThread.IsBackground = True
        bsThread.Start()
    End Sub

    ' ボタンなどを一括で元の有効状態に戻すための補助サブルーチン
    Private Sub RestoreUI(statusMessage As String)
        btnStart.Enabled = True
        btnBS.Enabled = True
        nudHours.Enabled = True
        nudInterval.Enabled = True
        UpdateStatus(statusMessage)
    End Sub

    Public Sub AppendLog(message As String)
        If txtLog.InvokeRequired Then
            txtLog.Invoke(Sub() AppendLog(message))
        Else
            txtLog.AppendText(message & Environment.NewLine)
            txtLog.ScrollToCaret()
        End If
    End Sub

    Public Sub EnableStopButton()
        If btnStop.InvokeRequired Then
            btnStop.Invoke(Sub() btnStop.Enabled = True)
        Else
            btnStop.Enabled = True
        End If
    End Sub

    Public Sub DisableStopButton()
        If btnStop.InvokeRequired Then
            btnStop.Invoke(Sub() btnStop.Enabled = False)
        Else
            btnStop.Enabled = False
        End If
    End Sub

    Public Sub UpdateStatus(message As String)
        If lblStatus.InvokeRequired Then
            lblStatus.Invoke(Sub()
                                 lblStatus.Text = message
                                 lblStatus.ForeColor = Color.FromArgb(100, 220, 100)
                             End Sub)
        Else
            lblStatus.Text = message
            lblStatus.ForeColor = Color.FromArgb(100, 220, 100)
        End If
    End Sub

End Class


' ==========================================================================
' メインモジュール
' ==========================================================================
Module Module1

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Function CreateFile(ByVal lpFileName As String, ByVal dwDesiredAccess As Int32, ByVal dwShareMode As Int32, ByVal lpSecurityAttributes As IntPtr, ByVal dwCreationDisposition As Int32, ByVal dwFlagsAndAttributes As Int32, ByVal hTemplateFile As IntPtr) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function WriteFile(ByVal hFile As IntPtr, ByVal lpBuffer As Byte(), ByVal nNumberOfBytesToWrite As Int32, ByRef lpNumberOfBytesWritten As Int32, ByVal lpOverlapped As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function ReadFile(ByVal hFile As IntPtr, ByVal lpBuffer As Byte(), ByVal nNumberOfBytesToRead As Int32, ByRef lpNumberOfBytesRead As Int32, ByVal lpOverlapped As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Public Function CloseHandle(ByVal hObject As IntPtr) As Boolean
    End Function

    Private Const ENQ As Byte = &H5
    Private Const EOT As Byte = &H4
    Private Const ACK As Byte = &H6
    Private Const NAK As Byte = &H15
    Private Const NUL As Byte = &H0

    Public Const GENERIC_READ As Int32 = &H80000000
    Public Const GENERIC_WRITE As Int32 = &H40000000
    Public Const OPEN_EXISTING As Int32 = 3
    Public Const FILE_ATTRIBUTE_NORMAL As Int32 = &H80
    Public ReadOnly INVALID_HANDLE_VALUE As New IntPtr(-1)

    Public Const PORT_NAME As String = "COM3"
    Private Const SCAN_POINTS As Integer = 1101
    Private Const MAX_RETRY As Integer = 5
    Private Const SCAN_WAIT_SEC As Integer = 360

    Private _stopRequested As Boolean = False
    Private _form As SettingsForm = Nothing

    Public Sub Log(message As String)
        Dim line As String = String.Format("[{0}] {1}", DateTime.Now.ToString("HH:mm:ss"), message)
        If _form IsNot Nothing Then
            _form.AppendLog(line)
        End If
    End Sub

    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Dim f As New SettingsForm()
        _form = f
        Application.Run(f)
    End Sub

    Public Sub RunSession(totalHours As Double, intervalMin As Integer)
        _stopRequested = False

        Dim rootFolder As String = String.Format("UV_Log_{0}", DateTime.Now.ToString("yyyyMMdd_HHmm"))
        If Not Directory.Exists(rootFolder) Then Directory.CreateDirectory(rootFolder)
        Dim logFile As String = Path.Combine(rootFolder, "session_log.txt")

        Log("==============================================")
        Log("   UV-1800 連続測定システム v10")
        Log("==============================================")
        Log(String.Format("保存フォルダ : {0}", rootFolder))
        Log(String.Format("総測定時間   : {0} 時間", totalHours))
        Log(String.Format("測定間隔     : {0} 分", intervalMin))
        Log(String.Format("取得点数     : {0} 点", SCAN_POINTS))
        Log("")

        Dim hCom As IntPtr = CreateFile(PORT_NAME, GENERIC_READ Or GENERIC_WRITE, 0, IntPtr.Zero,
                                         OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero)

        If hCom = INVALID_HANDLE_VALUE Then
            Log("ERROR: ポートオープン失敗。COM番号や接続を確認してください。")
            WriteLogFile(logFile, "ERROR: ポートオープン失敗")
            _form.DisableStopButton()
            Return
        End If

        Log(String.Format("ポート {0} オープン成功", PORT_NAME))
        WriteLogFile(logFile, String.Format("INFO : セッション開始 フォルダ={0}", rootFolder))

        Dim sessionStart As DateTime = DateTime.Now
        Dim sessionEnd As DateTime = sessionStart.AddHours(totalHours)
        Dim cycleIndex As Integer = 0
        Dim successCount As Integer = 0
        Dim failCount As Integer = 0

        Log(String.Format("連続測定開始 (終了予定: {0})", sessionEnd.ToString("HH:mm:ss")))
        Log("--------------------------------------------------")

        While DateTime.Now < sessionEnd AndAlso Not _stopRequested

            cycleIndex += 1
            Dim cycleStart As DateTime = DateTime.Now

            Log("")
            Log(String.Format("===== サイクル {0} 開始 (残り {1:F1} 時間) =====",
                cycleIndex, (sessionEnd - DateTime.Now).TotalHours))
            WriteLogFile(logFile, String.Format("INFO : サイクル {0} 開始", cycleIndex))
            _form.UpdateStatus(String.Format("測定中... サイクル {0}  残り {1:F1}時間",
                               cycleIndex, (sessionEnd - DateTime.Now).TotalHours))

            Dim result As Boolean = RunMeasureAndSave(hCom, rootFolder, cycleIndex, logFile)

            If result Then
                successCount += 1
                Log(String.Format("サイクル {0} 完了 [成功 {1}回 / 失敗 {2}回]",
                    cycleIndex, successCount, failCount))
                WriteLogFile(logFile, String.Format("INFO : サイクル {0} 正常完了", cycleIndex))
            Else
                If _stopRequested Then Exit While
                failCount += 1
                Log(String.Format("サイクル {0} 失敗 [成功 {1}回 / 失敗 {2}回]",
                    cycleIndex, successCount, failCount))
                WriteLogFile(logFile, String.Format("WARN : サイクル {0} 失敗", cycleIndex))
                SendByte(hCom, EOT)
                Threading.Thread.Sleep(1000)
            End If

            Dim elapsed As TimeSpan = DateTime.Now - cycleStart
            Dim waitSec As Integer = CInt(intervalMin * 60 - elapsed.TotalSeconds)

            If DateTime.Now >= sessionEnd OrElse _stopRequested Then Exit While

            If waitSec > 0 Then
                Dim nextStart As DateTime = DateTime.Now.AddSeconds(waitSec)
                Log(String.Format("待機中... (次回開始: {0})", nextStart.ToString("HH:mm:ss")))
                _form.UpdateStatus(String.Format("待機中... 次回: {0}  残り {1:F1}時間",
                                   nextStart.ToString("HH:mm:ss"),
                                   (sessionEnd - DateTime.Now).TotalHours))
                Dim waitDeadline As DateTime = DateTime.Now.AddSeconds(waitSec)
                While DateTime.Now < waitDeadline AndAlso DateTime.Now < sessionEnd AndAlso Not _stopRequested
                    Threading.Thread.Sleep(1000)
                End While
            Else
                Log("測定時間が間隔を超過したため即座に次サイクルへ")
            End If

        End While

        CloseHandle(hCom)

        Dim endReason As String = If(_stopRequested, "手動停止", "時間終了")
        Log("")
        Log("==============================================")
        Log(String.Format("セッション終了 ({0})", endReason))
        Log(String.Format("総サイクル数 : {0}", cycleIndex))
        Log(String.Format("成功         : {0} 回", successCount))
        Log(String.Format("失敗         : {0} 回", failCount))
        Log(String.Format("保存フォルダ : {0}", rootFolder))
        Log("==============================================")
        WriteLogFile(logFile, String.Format("INFO : セッション終了({0}) 総サイクル={1} 成功={2} 失敗={3}",
                              endReason, cycleIndex, successCount, failCount))
        _form.UpdateStatus(String.Format("終了({0})  成功:{1}回 / 失敗:{2}回", endReason, successCount, failCount))
        _form.DisableStopButton()
    End Sub

    Private Function RunMeasureAndSave(hCom As IntPtr, folder As String, cycleIndex As Integer, logFile As String) As Boolean

        Log("[STEP0] 通信リセット")
        SendByte(hCom, EOT)
        Threading.Thread.Sleep(500)
        If _stopRequested Then Return False

        Log("[STEP1] スキャン範囲設定 'h800,250'")
        Log("  ENQ送信 -> ACK待ち...")
        If Not SendEnqAndWaitAck(hCom) Then
            Log("  FAILED: ACK受信失敗")
            WriteLogFile(logFile, String.Format("ERROR: サイクル {0} STEP1 ENQ->ACK失敗", cycleIndex))
            Return False
        End If
        Log("  OK: ACK受信")
        Log("  'h800,250'+NUL送信 -> ACK待ち...")
        SendCmd(hCom, "h800,250")
        If Not WaitForByte(hCom, ACK, 5) Then
            Log("  FAILED: ACK受信失敗")
            WriteLogFile(logFile, String.Format("ERROR: サイクル {0} STEP1 hコマンドACK失敗", cycleIndex))
            Return False
        End If
        Log("  OK: ACK受信")
        Log("  設定完了待ち(EOT) 最大10秒...")
        If Not WaitForByte(hCom, EOT, 10) Then
            Log("  FAILED: EOTタイムアウト")
            WriteLogFile(logFile, String.Format("ERROR: サイクル {0} STEP1 EOTタイムアウト", cycleIndex))
            Return False
        End If
        SendByte(hCom, ACK)
        Log("  OK: EOT受信 -> ACK送信 (範囲設定完了)")
        Threading.Thread.Sleep(500)
        If _stopRequested Then Return False

        Log("[STEP2] 測定コマンド 'a'")
        Log("  ENQ送信 -> ACK待ち...")
        If Not SendEnqAndWaitAck(hCom) Then
            Log("  FAILED: ACK受信失敗")
            WriteLogFile(logFile, String.Format("ERROR: サイクル {0} STEP2 ENQ->ACK失敗", cycleIndex))
            Return False
        End If
        Log("  OK: ACK受信")
        Log("  'a'+NUL送信 -> ACK待ち...")
        SendCmd(hCom, "a")
        If Not WaitForByte(hCom, ACK, 5) Then
            Log("  FAILED: ACK受信失敗")
            WriteLogFile(logFile, String.Format("ERROR: サイクル {0} STEP2 'a'コマンドACK失敗", cycleIndex))
            Return False
        End If
        Log("  OK: ACK受信 -- スキャン実行中")
        Log(String.Format("  スキャン完了待ち (最大{0}秒)...", SCAN_WAIT_SEC))
        If Not WaitForByte(hCom, EOT, SCAN_WAIT_SEC) Then
            Log("  FAILED: EOTタイムアウト")
            WriteLogFile(logFile, String.Format("ERROR: サイクル {0} STEP2 EOTタイムアウト", cycleIndex))
            Return False
        End If
        SendByte(hCom, ACK)
        Log("  OK: スキャン完了 (EOT受信 -> ACK送信)")
        Threading.Thread.Sleep(500)
        If _stopRequested Then Return False

        Log(String.Format("[STEP3] データ転送 'f{0}'", SCAN_POINTS))
        Log("  ENQ送信 -> ACK待ち...")
        If Not SendEnqAndWaitAck(hCom) Then
            Log("  FAILED: ACK受信失敗")
            WriteLogFile(logFile, String.Format("ERROR: サイクル {0} STEP3 ENQ->ACK失敗", cycleIndex))
            Return False
        End If
        Log("  OK: ACK受信")
        Dim fCmd As String = String.Format("f{0}", SCAN_POINTS)
        Log(String.Format("  '{0}'+NUL送信 -> ACK待ち...", fCmd))
        SendCmd(hCom, fCmd)
        If Not WaitForByte(hCom, ACK, 5) Then
            Log("  FAILED: ACK受信失敗")
            WriteLogFile(logFile, String.Format("ERROR: サイクル {0} STEP3 fnコマンドACK失敗", cycleIndex))
            Return False
        End If
        Log("  OK: ACK受信")
        Log("  UV1800からのENQ待ち -> ACK応答...")
        If Not WaitForByte(hCom, ENQ, 10) Then
            Log("  FAILED: ENQ受信タイムアウト")
            WriteLogFile(logFile, String.Format("ERROR: サイクル {0} STEP3 ENQタイムアウト", cycleIndex))
            Return False
        End If
        SendByte(hCom, ACK)
        Log("  OK: ENQ受信 -> ACK送信")

        Log(String.Format("  データ受信開始 ({0}点)...", SCAN_POINTS))
        Dim dataLines As New List(Of String)
        Dim pointCount As Integer = 0
        Dim lineBuffer As New StringBuilder()
        Dim recvBuf(0) As Byte
        Dim readLen As Int32 = 0
        Dim deadline As DateTime = DateTime.Now.AddSeconds(120)

        While DateTime.Now < deadline AndAlso Not _stopRequested
            If Not ReadFile(hCom, recvBuf, 1, readLen, IntPtr.Zero) OrElse readLen = 0 Then
                Threading.Thread.Sleep(5)
                Continue While
            End If
            Dim b As Byte = recvBuf(0)
            If b = EOT Then
                SendByte(hCom, ACK)
                Log(String.Format("  EOT受信 -> ACK送信 (受信完了: {0}点)", pointCount))
                Exit While
            ElseIf b = NUL Then
                Dim line As String = lineBuffer.ToString().Trim()
                If line.Length > 0 Then
                    dataLines.Add(line)
                    pointCount += 1
                    If pointCount Mod 100 = 0 OrElse pointCount = SCAN_POINTS Then
                        Log(String.Format("  {0}/{1} 点受信済み", pointCount, SCAN_POINTS))
                    End If
                End If
                lineBuffer.Clear()
                SendByte(hCom, ACK)
            ElseIf b >= &H20 Then
                lineBuffer.Append(Chr(b))
            End If
        End While

        If _stopRequested Then Return False

        dataLines.Reverse()
        Log("  データを昇順に並べ替え完了")

        If dataLines.Count = 0 Then
            Log("[STEP4] FAILED: 受信データが0件です")
            WriteLogFile(logFile, String.Format("ERROR: サイクル {0} STEP4 データ0件", cycleIndex))
            Return False
        End If
        If dataLines.Count < SCAN_POINTS Then
            Log(String.Format("[STEP4] WARNING: 受信点数({0}点) が期待値({1}点)より少ないです",
                dataLines.Count, SCAN_POINTS))
            WriteLogFile(logFile, String.Format("WARN : サイクル {0} 受信点数不足 {1}/{2}点",
                cycleIndex, dataLines.Count, SCAN_POINTS))
        End If

        Dim timeStamp As String = DateTime.Now.ToString("yyyyMMdd_HHmm")
        Dim fileName As String = Path.Combine(folder,
                                  String.Format("Spectrum_{0}_cycle{1:D4}.txt", timeStamp, cycleIndex))
        Dim sb As New StringBuilder()
        sb.AppendLine("# UV-1800 Spectrum Data")
        sb.AppendLine(String.Format("# Date       : {0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")))
        sb.AppendLine(String.Format("# Cycle      : {0}", cycleIndex))
        sb.AppendLine("# ScanRange  : 250nm - 800nm")
        sb.AppendLine(String.Format("# Points     : {0}", dataLines.Count))
        sb.AppendLine("# Wavelength(nm)  Absorbance")
        sb.AppendLine("# ---")
        For Each line In dataLines
            sb.AppendLine(line)
        Next
        File.WriteAllText(fileName, sb.ToString(), Encoding.ASCII)

        Log(String.Format("[STEP4] 保存完了: {0} ({1}点)", Path.GetFileName(fileName), dataLines.Count))
        WriteLogFile(logFile, String.Format("INFO : サイクル {0} 保存完了 {1} ({2}点)",
                              cycleIndex, fileName, dataLines.Count))
        Return True
    End Function

    ''' <summary>
    ''' ベースライン補正を実行する
    ''' </summary>
    Public Function RunBaseline(hCom As IntPtr) As Boolean
        ' ベースラインの最大待機時間（180秒 = 3分）
        Dim BASELINE_WAIT_SEC As Integer = 180

        Log("[BASELINE] 通信リセット")
        SendByte(hCom, EOT)
        Threading.Thread.Sleep(500)

        Log("[BASELINE] コマンド 'b1' 送信開始")
        Log("  ENQ送信 -> ACK待ち...")
        If Not SendEnqAndWaitAck(hCom) Then
            Log("  FAILED: ACK受信失敗")
            Return False
        End If

        Log("  OK: ACK受信")
        Log("  'b1'+NUL送信 -> ACK待ち...")
        SendCmd(hCom, "b1")
        If Not WaitForByte(hCom, ACK, 5) Then
            Log("  FAILED: ACK受信失敗")
            Return False
        End If

        Log("  OK: ACK受信 -- ベースライン測定中...")
        Log(String.Format("  完了待ち (最大{0}秒)...", BASELINE_WAIT_SEC))

        ' 装置がベースラインを終えてEOTを返してくるのを待つ
        If Not WaitForByte(hCom, EOT, BASELINE_WAIT_SEC) Then
            Log("  FAILED: EOTタイムアウト (ベースラインが時間内に終わりませんでした)")
            Return False
        End If

        ' EOTを受け取ったので、ACKを返す（確定）
        SendByte(hCom, ACK)
        Log("  OK: ベースライン完了 (EOT受信 -> ACK送信)")
        Return True
    End Function

    Public Sub RequestStop()
        _stopRequested = True
        Log("*** 強制終了が要求されました ***")
    End Sub

    Private Function SendEnqAndWaitAck(hCom As IntPtr) As Boolean
        For i As Integer = 1 To MAX_RETRY
            SendByte(hCom, ENQ)
            Dim resp As Byte = ReadOneByte(hCom, 3)
            If resp = ACK Then Return True
            If resp = NAK Then
                Log(String.Format("  (NAK->再送{0}/{1})", i, MAX_RETRY))
            Else
                Log(String.Format("  (タイムアウト->再送{0}/{1})", i, MAX_RETRY))
            End If
            Threading.Thread.Sleep(200)
        Next
        Return False
    End Function

    Private Sub SendByte(hCom As IntPtr, b As Byte)
        Dim written As Int32 = 0
        WriteFile(hCom, New Byte() {b}, 1, written, IntPtr.Zero)
    End Sub

    Private Sub SendCmd(hCom As IntPtr, cmdText As String)
        Dim written As Int32 = 0
        Dim cmdBytes As Byte() = Encoding.ASCII.GetBytes(cmdText)
        Dim payload(cmdBytes.Length) As Byte
        Array.Copy(cmdBytes, payload, cmdBytes.Length)
        payload(payload.Length - 1) = NUL
        WriteFile(hCom, payload, payload.Length, written, IntPtr.Zero)
    End Sub

    Private Function WaitForByte(hCom As IntPtr, target As Byte, timeoutSec As Integer) As Boolean
        Dim deadline As DateTime = DateTime.Now.AddSeconds(timeoutSec)
        While DateTime.Now < deadline
            Dim b As Byte = ReadOneByte(hCom, 1)
            If b = target Then Return True
        End While
        Return False
    End Function

    Private Function ReadOneByte(hCom As IntPtr, timeoutSec As Integer) As Byte
        Dim buf(0) As Byte
        Dim readLen As Int32 = 0
        Dim deadline As DateTime = DateTime.Now.AddSeconds(timeoutSec)
        While DateTime.Now < deadline
            If ReadFile(hCom, buf, 1, readLen, IntPtr.Zero) AndAlso readLen > 0 Then
                Return buf(0)
            End If
            Threading.Thread.Sleep(5)
        End While
        Return 0
    End Function

    Private Sub WriteLogFile(logFile As String, message As String)
        Dim line As String = String.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message)
        Try
            File.AppendAllText(logFile, line & Environment.NewLine, Encoding.UTF8)
        Catch
        End Try
    End Sub

End Module