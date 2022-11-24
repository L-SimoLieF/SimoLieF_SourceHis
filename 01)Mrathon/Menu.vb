'マラソン大会の集計ソフト
'Programming by SimoLy
Imports MySql.Data.MySqlClient
Imports Microsoft.Office.Interop
Imports System.Runtime.InteropServices

Public Class Menu
    'SQLサーバへの接続
    Dim Builder = New MySqlConnectionStringBuilder()
    Dim Con As New MySqlConnection
    Dim ConStr As String
    Dim SqlStr As String
    Dim Adapter As New MySqlDataAdapter
    Dim Adapter1 As New MySqlDataAdapter
    Dim Adapter2 As New MySqlDataAdapter
    Dim TimeAdapter As New MySqlDataAdapter
    Dim OutAdapter As New MySqlDataAdapter
    Dim Checker As MySqlDataReader
    Dim Command As MySqlCommand
    Dim CheckCom As MySqlCommand
    Dim DGVDs As New DataSet
    Dim DGVDs2 As New DataSet
    Dim DGVDs3 As New DataSet
    Dim hDGVDs As New DataSet
    Dim OutDGVDs As New DataSet
    Dim Flag As New Integer
    Dim Year As Integer




    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.TabControl2.SelectedTab = Me.TabPage4
        Button1.Enabled = False
        Button2.Enabled = True
        Button3.Enabled = True
        Button4.Enabled = True
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.TabControl2.SelectedTab = Me.TabPage5
        Button1.Enabled = True
        Button2.Enabled = False
        Button3.Enabled = True
        Button4.Enabled = True
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.TabControl2.SelectedTab = Me.TabPage6
        Button1.Enabled = True
        Button2.Enabled = True
        Button3.Enabled = False
        Button4.Enabled = True
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Me.TabControl2.SelectedTab = Me.TabPage7
        Button1.Enabled = True
        Button2.Enabled = True
        Button3.Enabled = True
        Button4.Enabled = False
    End Sub
    '初期設定
    Private Sub Menu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'SQLサーバへの接続
        Builder.Server = "MYSQLSERVER"
        Builder.Port = 3306
        Builder.UserID = "simoly"
        Builder.Password = "simolyprog"
        Builder.Database = "marathon2018"
        ConStr = Builder.ToString()
        Con.ConnectionString = ConStr

        'DataGridViewへの表示
        Adapter1.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',stu_name AS '氏名',stu_sta AS '事前情報' From classtbl,studenttbl,resalttbl Where studenttbl.cls_code = classtbl.cls_code And studenttbl.stu_code = resalttbl.stu_code ", Con)
        Adapter1.Fill(DGVDs)

        DataGridView1.DataSource = DGVDs.Tables(0)

        Adapter2.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',stu_name AS '氏名',rank AS '順位',stu_sta AS '事前情報' From classtbl,studenttbl,resalttbl Where studenttbl.cls_code = classtbl.cls_code And studenttbl.stu_code = resalttbl.stu_code ", Con)
        Adapter2.Fill(DGVDs2)
        DataGridView2.DataSource = DGVDs2.Tables(0)
    End Sub

    '事前情報入力
    Private Sub Update1_Click(sender As Object, e As EventArgs) Handles Update1.Click
        Try
            Dim No As Integer = PriorTextBox2.Text
            Dim status As String = CStr(PriorComboBox2.SelectedItem)


            Command = New MySqlCommand("Update marathon2018.resalttbl Left Join studenttbl On resalttbl.stu_code = studenttbl.stu_code set stu_sta = @status Where studenttbl.cls_code = 'C" & (PriorComboBox1.SelectedIndex + 6 * (Val(PriorTextBox1.Text) - 1) + 1).ToString("00") & "' And studenttbl.no = @No ", Con)
            Command.Parameters.AddWithValue("@status", status)
            Command.Parameters.AddWithValue("@No", No)
            Command.Connection.Open()
            Command.ExecuteNonQuery()
            Command.Connection.Close()

            DGVDs.Clear()
            DataGridView1.DataSource = DGVDs.Tables
            Adapter1.Fill(DGVDs)
            DataGridView1.DataSource = DGVDs.Tables(0)

        Catch ex As Exception
            MsgBox("入力データが誤っています。再度確認してください。")
        End Try

        Update1.Enabled = False
        PriorComboBox1.Focus()
    End Sub
    '順位入力
    Private Sub Update2_Click(sender As Object, e As EventArgs) Handles Update2.Click
        Dim c_code As String = CStr(RankComboBox1.SelectedValue)
        Dim No As Integer = Val(RankTextBox2.Text)
        Dim rank As Integer = Val(RankTextBox3.Text)

        If DoubleRankChecker(rank) = 0 Then
            MsgBox("重複認証不成立の為、順位登録を中断しました。")
        Else
            Command = New MySqlCommand("Update marathon2018.resalttbl Left Join studenttbl On resalttbl.stu_code = studenttbl.stu_code Set rank = @rank  Where studenttbl.cls_code = 'C" & (RankComboBox1.SelectedIndex + 6 * (Val(RankTextBox1.Text) - 1) + 1).ToString("00") & "' And studenttbl.no = @No ", Con)
            Command.Parameters.AddWithValue("@rank", rank)
            Command.Parameters.AddWithValue("@c_code", c_code)
            Command.Parameters.AddWithValue("@No", No)
            Command.Connection.Open()
            Command.ExecuteNonQuery()
            Command.Connection.Close()
        End If

        DGVDs2.Clear()
        DataGridView2.DataSource = DGVDs2.Tables
        Adapter2.Fill(DGVDs2)
        DataGridView2.DataSource = DGVDs2.Tables(0)

        Update2.Enabled = False
        RankComboBox1.Focus()
    End Sub
    'タイム入力用のデータ作成
    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click

        TimeAdapter.SelectCommand = New MySqlCommand("Select rank AS '順位',time AS 'タイム' From ranktbl Where rank IS NOT NULL", Con)
        TimeAdapter.Fill(DGVDs3)
        DataGridView3.DataSource = DGVDs3.Tables(0)

    End Sub
    'タイム入力
    Private Sub Update3_Click(sender As Object, e As EventArgs) Handles Update3.Click
        Dim builder As MySqlCommandBuilder
        TimeAdapter.SelectCommand.CommandText = "SELECT rank AS '順位',time AS 'タイム' FROM ranktbl WHERE rank IS NOT NULL"
        builder = New MySqlCommandBuilder(TimeAdapter)
        TimeAdapter.Update(DGVDs3)
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        DGVDs3.Clear()
        DataGridView4.DataSource = DGVDs3.Tables
        TimeAdapter.Fill(DGVDs3)
        DataGridView4.DataSource = DGVDs3.Tables(0)
    End Sub


    '完走者順位入力
    Private Sub Update4_Click(sender As Object, e As EventArgs) Handles Update4.Click

        Dim comprank As Integer = Val(TextBox4.Text)
        Command = New MySqlCommand("Update marathon2018.tourtbl set comp_rank = @comprank Where tour_code = 'T001'", Con)
        Command.Parameters.AddWithValue("@comprank", comprank)
        Command.Connection.Open()
        Command.ExecuteNonQuery()
        Command.Connection.Close()

        Command = New MySqlCommand("Update marathon2018.resalttbl set stu_sta = '〇' Where stu_code IN((SELECT stu_code FROM (SELECT stu_code,rank FROM resalttbl)temporarytbl WHERE resalttbl.rank <= (SELECT comp_rank From tourtbl WHERE tour_code = 'T001')))", Con)
        Command.Connection.Open()
        Command.ExecuteNonQuery()
        Command.Connection.Close()

        Command = New MySqlCommand("Update marathon2018.resalttbl set stu_sta = '×' Where stu_code IN((SELECT stu_code FROM (SELECT stu_code,rank FROM resalttbl)temporarytbl WHERE resalttbl.rank > (SELECT comp_rank From tourtbl WHERE tour_code = 'T001')))", Con)
        Command.Connection.Open()
        Command.ExecuteNonQuery()
        Command.Connection.Close()

        MsgBox("完走者順位を" & Val(TextBox4.Text) & "位として登録しました。")
    End Sub

    '氏名検索
    Private Function FindName(ByVal sTab As TabPage)
        Con.Open()
        Flag = 0
        If sTab Is TabPage4 Then '事前情報登録時
            Dim No As Integer = Val(PriorTextBox2.Text)
            Command = New MySqlCommand("Select stu_name From studenttbl Where cls_code = 'C" & (PriorComboBox1.SelectedIndex + 6 * (Val(PriorTextBox1.Text) - 1) + 1).ToString("00") & "' And No = @No ", Con)
            Command.Parameters.AddWithValue("@No", No)
        ElseIf sTab Is TabPage5 Then '順位入力時
            Dim No As Integer = Val(RankTextBox2.Text)
            Command = New MySqlCommand("Select stu_name From studenttbl Where cls_code = 'C" & (RankComboBox1.SelectedIndex + 6 * (Val(RankTextBox1.Text) - 1) + 1).ToString("00") & "' And No = @No ", Con)
            Command.Parameters.AddWithValue("@No", No)

            '選手状態確認
            CheckCom = New MySqlCommand("Select stu_code From resaltTbl Where stu_code = (Select stu_code From studenttbl Where cls_code = 'C" & (RankComboBox1.SelectedIndex + 6 * (Val(RankTextBox1.Text) - 1) + 1).ToString("00") & "' And No = @No) And stu_sta IN('欠','手','見')", Con)
            CheckCom.Parameters.AddWithValue("@No", No)
            Checker = CheckCom.ExecuteReader()
            If Checker.HasRows Then
                Flag = 1
            End If
            Checker.Close()


        End If

        Using Reader As MySqlDataReader = Command.ExecuteReader()
            While (Reader.Read())
                FindName = Reader.GetString(0)
            End While
        End Using
        Con.Close()
    End Function


    '順位の重複処理
    Private Function DoubleRankChecker(inRank As Integer)
        Con.Open()
        Dim CheckCom2 = New MySqlCommand("Select stu_code From resalttbl WHERE rank = @inRank", Con)
        CheckCom2.Parameters.AddWithValue("@inRank", inRank)
        Dim Checker2 = CheckCom2.ExecuteReader()
        If Checker2.HasRows Then
            If MessageBox.Show("現在入力されている順位は既に入力済みです。現在の順位を登録し、入力済みの順位を置換しますか？", "重複確認", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                Con.Close()
                Command = New MySqlCommand("Update marathon2018.resalttbl Set rank = '999' Where stu_code = (Select stu_code From (SELECT stu_code FROM resalttbl WHERE rank = @inRank) AS temporarytbl)", Con)
                Command.Parameters.AddWithValue("@inRank", inRank)
                Command.Connection.Open()
                Command.ExecuteNonQuery()
                Command.Connection.Close()
                Return 1
            Else
                Con.Close()
                Return 0
            End If
        Else
            Con.Close()
            Return 1


        End If
    End Function

    '表示画面の表出力
    Private Function DisplayDatafinder()
        If CheckBox1.Checked = True Then '未登録順位の出力
            Adapter.SelectCommand = New MySqlCommand("SELECT Nullrank AS 未登録順位 FROM (SELECT TempoT.stu_code,(SELECT COUNT(*) FROM resalttbl EntityT WHERE EntityT.stu_code <= TempoT.stu_code AND stu_sta NOT IN('欠','手','見')) AS Nullrank FROM (SELECT stu_code FROM resalttbl WHERE stu_sta NOT IN('欠','手','見')) TempoT) LOjoinT LEFT OUTER JOIN resalttbl ON LOjoinT.Nullrank = resalttbl.rank WHERE resalttbl.rank IS NULL", Con)
            Con.Open()
            Checker = Adapter.SelectCommand.ExecuteReader()
            If Checker.HasRows = False Then
                MsgBox("現在、未登録順位は存在しません。")
            Else
                MsgBox("未登録順位を確認しました。ビューに表示します。")
            End If
            Con.Close()


        ElseIf TextBox3.Text <> Nothing Then '科,年,番号の入力あり。
            Adapter.SelectCommand = New MySqlCommand("SELECT cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム' FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code = 'C" & (ComboBox2.SelectedIndex + 6 * (Val(TextBox1.Text) - 1) + 1).ToString("00") & "' AND studenttbl.cls_code = classtbl.cls_code AND no = " & Val(TextBox3.Text), Con)
        ElseIf ComboBox2.Text <> Nothing And TextBox1.Text <> Nothing Then 'クラス単位
            Adapter.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム' FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code = 'C" & (ComboBox2.SelectedIndex + 6 * (Val(TextBox1.Text) - 1) + 1).ToString("00") & "' AND studenttbl.cls_code = classtbl.cls_code", Con)
        ElseIf TextBox1.Text <> Nothing Then '学年単位
            Select Case TextBox1.Text
                Case 1
                    Adapter.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム' FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code IN('C01','C02','C03','C04','C05','C06') AND studenttbl.cls_code = classtbl.cls_code", Con)
                Case 2
                    Adapter.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム' FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code IN('C07','C08','C09','C10','C11','C12') AND studenttbl.cls_code = classtbl.cls_code", Con)
                Case 3
                    Adapter.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム' FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code IN('C13','C14','C15','C16','C17','C18') AND studenttbl.cls_code = classtbl.cls_code", Con)
            End Select
        ElseIf ComboBox2.Text <> Nothing Then '学科単位
            Select Case ComboBox2.SelectedIndex
                Case 0
                    Adapter.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム' FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code IN('C01','C07','C13') AND studenttbl.cls_code = classtbl.cls_code", Con)
                Case 1
                    Adapter.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム' FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code IN('C02','C08','C14') AND studenttbl.cls_code = classtbl.cls_code", Con)
                Case 2
                    Adapter.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム' FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code IN('C03','C09','C15') AND studenttbl.cls_code = classtbl.cls_code", Con)
                Case 3
                    Adapter.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム' FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code IN('C04','C10','C16') AND studenttbl.cls_code = classtbl.cls_code", Con)
                Case 4
                    Adapter.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム' FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code IN('C05','C11','C17') AND studenttbl.cls_code = classtbl.cls_code", Con)
                Case 5
                    Adapter.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム' FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code IN('C06','C12','C18') AND studenttbl.cls_code = classtbl.cls_code", Con)
            End Select
        Else '検索条件入力なし
            Adapter.SelectCommand = New MySqlCommand("Select cls_lname AS 'クラス',no AS '番号',stu_name AS '生徒氏名',resalttbl.rank AS '順位',time AS 'タイム'  FROM classtbl,studenttbl LEFT OUTER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT JOIN ranktbl ON resalttbl.rank = ranktbl.rank Where studenttbl.cls_code = classtbl.cls_code", Con)
        End If
        Return 0
    End Function

    '表示タブ　更新
    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        DisplayDatafinder()
        hDGVDs.Clear()
        DataGridView4.DataSource = hDGVDs.Tables
        Adapter.Fill(hDGVDs)
        DataGridView4.DataSource = hDGVDs.Tables(0)
    End Sub


    '事前情報入力のフォーカスセット
    Private Sub PriorComboBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles PriorComboBox1.KeyUp
        If e.KeyData = Keys.Enter Then
            PriorTextBox1.Focus()
        End If
    End Sub
    Private Sub PriorTextBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles PriorTextBox1.KeyUp
        If e.KeyData = Keys.Enter Then
            PriorTextBox2.Focus()
        End If
    End Sub

    Private Sub PriorTextBox2_KeyUp(sender As Object, e As KeyEventArgs) Handles PriorTextBox2.KeyUp　'番号入力時に氏名検索
        If e.KeyData = Keys.Enter Then
            PriorTextBox3.Text = FindName(TabControl2.SelectedTab)
            PriorComboBox2.Focus()
        End If
    End Sub
    Private Sub PriorComboBox2_KeyUp(sender As Object, e As KeyEventArgs) Handles PriorComboBox2.KeyUp
        If e.KeyData = Keys.Enter And PriorTextBox3.Text <> Nothing And PriorComboBox2.SelectedItem <> Nothing Then '更新のEnableをTrueに
            Update1.Enabled = True
            Update1.Focus()
        ElseIf e.KeyData = Keys.Enter And PriorTextBox3.Text <> Nothing Then
            If PriorComboBox2.Text <> Nothing Then
                MsgBox("リストから選択してください。")
                Update1.Enabled = False
                PriorTextBox2.Focus()
            Else
                If MessageBox.Show("情報タブが空白です。対象者が不参加の場合は「いいえ」", "", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    Update1.Enabled = True
                    Update1.Focus()
                Else
                    PriorTextBox2.Focus()
                    Update1.Enabled = False
                End If
            End If
        ElseIf e.KeyData = Keys.Enter And PriorComboBox2.SelectedItem <> Nothing Then
            MsgBox("生徒情報が誤っています。学科,学年,番号を確認してください。")
            PriorTextBox2.Focus()
            Update1.Enabled = False
        ElseIf e.KeyData = Keys.Enter Then
            MsgBox("入力データが誤っています。再度確認してください。")
            PriorTextBox2.Focus()
            Update1.Enabled = False
        End If
    End Sub


    '順位入力のフォーカスセット
    Private Sub RankComboBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles RankComboBox1.KeyUp
        If e.KeyData = Keys.Enter Then
            RankTextBox1.Focus()
        End If
    End Sub

    Private Sub RankTextBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles RankTextBox1.KeyUp
        If e.KeyData = Keys.Enter Then
            RankTextBox2.Focus()
        End If
    End Sub
    '番号入力時の不参加判定
    Private Sub RankTextBox2_KeyUp(sender As Object, e As KeyEventArgs) Handles RankTextBox2.KeyUp
        If e.KeyData = Keys.Enter Then
            RankTextBox4.Text = FindName(TabControl2.SelectedTab)
            RankTextBox3.Focus()
            If Flag = 1 Then
                MsgBox("対象選手は不参加生徒です。")
                Update2.Enabled = False
                RankTextBox1.Focus()
            End If
        End If
    End Sub
    Private Sub RankTextBox3_KeyUp(sender As Object, e As KeyEventArgs) Handles RankTextBox3.KeyUp
        If e.KeyData = Keys.Enter And RankTextBox4.Text <> Nothing Then '更新のEnableをTrueに
            If Flag = 0 And RankTextBox3.Text <> Nothing Then
                Update2.Enabled = True
                Update2.Focus()
            ElseIf Flag = 0 Then
                MsgBox("順位を入力してください。")
                RankTextBox2.Focus()
            End If
        End If
    End Sub

    '完走者順位のフォーカスセット
    Private Sub TextBox4_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox4.KeyUp
        If e.KeyData = Keys.Enter And TextBox4.Text <> Nothing Then
            Update4.Enabled = True
            Update4.Focus()
        ElseIf e.KeyData = Keys.Enter Then
            MsgBox("完走者順位を入力してください。")
        End If
    End Sub

    Private Sub Form_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MyBase.KeyPress 'Enter遷移時の音対策
        If e.KeyChar = ControlChars.Cr Then
            e.Handled = True
        End If
    End Sub
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            ComboBox2.Enabled = False
            TextBox1.Enabled = False
            TextBox3.Enabled = False
        Else
            ComboBox2.Enabled = True
            TextBox1.Enabled = True
            TextBox3.Enabled = True
        End If
    End Sub
    Private Sub ComboBox2_KeyUp(sender As Object, e As KeyEventArgs) Handles ComboBox2.KeyUp
        If e.KeyData = Keys.Enter Then
            TextBox1.Focus()
        End If
    End Sub
    Private Sub TextBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyUp
        If e.KeyData = Keys.Enter Then
            TextBox3.Focus()
        End If
    End Sub
    '出力画面
    'クラス順位
    Private Sub ExcOutput1_Click(sender As Object, e As EventArgs) Handles ExcOutput1.Click

        If OutDGVDs IsNot Nothing Then
            OutDGVDs = Nothing
            OutDGVDs = New DataSet
        End If
        Dim Path As String = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) & "\マラソン大会エクセルファイル"

        Dim ExcelObj As Excel.Application
        Dim WorkBookObj As Excel.Workbook
        Dim SheetObj As Excel.Worksheet

        ExcelObj = CreateObject("Excel.Application")
        WorkBookObj = ExcelObj.Workbooks.Open(Path & "\学年順位・総合順位一覧Ver.S.xlsx")

        OutDGVDs = New DataSet
        Try
            Con.Open()
            OutAdapter.SelectCommand = New MySqlCommand("SELECT Point AS 合計点,COALESCE(Cninzu,0) AS 完走者人数 FROM (classtbl LEFT JOIN (SELECT cls_lname,COUNT(stu_sta) AS Cninzu FROM resalttbl,classtbl,studenttbl WHERE tour_code = 'T001' AND resalttbl.stu_code = studenttbl.stu_code AND studenttbl.cls_code = classtbl.cls_code AND stu_sta = '〇' GROUP BY classtbl.cls_lname) AS Tcomp1tbl ON classtbl.cls_lname = Tcomp1tbl.cls_lname) LEFT JOIN (SELECT cls_lname,SUM(CASE WHEN rank<201 THEN 241-rank WHEN rank>200 AND stu_sta = '〇' THEN 20 WHEN rank>200 AND stu_sta = '×' THEN 1 ELSE 0 END) AS Point FROM resalttbl,classtbl,studenttbl WHERE tour_code = 'T001' AND resalttbl.stu_code = studenttbl.stu_code AND studenttbl.cls_code = classtbl.cls_code GROUP BY classtbl.cls_code) AS Tcomp2tbl ON classtbl.cls_lname = Tcomp2tbl.cls_lname", Con)

            OutAdapter.Fill(OutDGVDs)
            DataGridView5.DataSource = OutDGVDs.Tables(0)
        Catch ex As MySqlException
            MessageBox.Show("SQLErrorが発生しました。(" & ex.ErrorCode & ":" & ex.Message & ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            MessageBox.Show("エラーが発生しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Try
                Con.Close()
            Catch ex As Exception
            End Try
        End Try

        SheetObj = WorkBookObj.Worksheets(1)
        ExcelOutPut1(1, ExcelObj, WorkBookObj, SheetObj)
        Marshal.ReleaseComObject(SheetObj)


        WorkBookObj.SaveAs(Filename:=CreateObject("WScript.Shell").SpecialFolders("Desktop") & "\学年順位・総合順位一覧Ver.S.xlsx")
        WorkBookObj.Saved = True
        MessageBox.Show("デスクトップに出力しました。")

        WorkBookObj.Close()
        Marshal.ReleaseComObject(WorkBookObj)
        ExcelObj.Quit()
        Marshal.ReleaseComObject(ExcelObj)
        SheetObj = Nothing
        WorkBookObj = Nothing
        ExcelObj = Nothing

        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()



    End Sub
    Private Sub ExcelOutPut1(sheetnum As Integer, ExcelObj As Excel.Application, WorkBookObj As Excel.Workbook, SheetObj As Excel.Worksheet)
        Dim stCurrentDir As String = System.IO.Directory.GetCurrentDirectory()
        Try
            Dim ColumnMaxNum As Integer = DataGridView5.Columns.Count - 1
            Dim RowMaxNum As Integer = DataGridView5.Rows.Count - 1

            Dim ColumnList As New List(Of String)
            For i As Integer = 0 To (ColumnMaxNum)
                ColumnList.Add(DataGridView5.Columns(i).HeaderCell.Value)
            Next

            Dim v As Integer(,) = New Integer(RowMaxNum, ColumnMaxNum) {}
            For Row As Integer = 0 To RowMaxNum
                For Col As Integer = 0 To ColumnMaxNum
                    If DataGridView5.Rows(Row).Cells(Col).Value Is Nothing = False Then
                        v(Row, Col) = DataGridView5.Rows(Row).Cells(Col).Value
                    End If

                Next
            Next

            For i As Integer = 1 To DataGridView5.Columns.Count
                SheetObj.Cells(5, i + 2) = ColumnList(i - 1)

                SheetObj.Cells(5, i).Borders.LineStyle = True
            Next

            Dim Data As String = "C6:" & Chr(Asc("C") + ColumnMaxNum) & DataGridView5.Rows.Count + 4
            WorkBookObj.Sheets(sheetnum).Range(Data) = v

            WorkBookObj.Sheets(1).Range(Data).Borders.LineStyle = True

        Catch ex As Exception
            MessageBox.Show("エラーが発生しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    'クラス別生徒情報
    Private Sub ExcOutput2_Click(sender As Object, e As EventArgs) Handles ExcOutput2.Click
        If OutDGVDs IsNot Nothing Then
            OutDGVDs = Nothing
            OutDGVDs = New DataSet
        End If

        Dim Path As String = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) & "\マラソン大会エクセルファイル"

        Dim ExcelObj As Excel.Application
        Dim WorkBookObj As Excel.Workbook
        Dim SheetObj As Excel.Worksheet

        Dim classname() As String
        classname = Split("J1,E1,M1,K1,D1,S1,J2,E2,M2,K2,D2,S2,J3,E3,M3,K3,D3,S3", ",")
        ExcelObj = CreateObject("Excel.Application")

        WorkBookObj = ExcelObj.Workbooks.Open(Path & "\クラス別生徒情報一覧.xlsx")

        For i As Integer = 1 To 18
            OutDGVDs = New DataSet
            Try
                Con.Open()
                OutAdapter.SelectCommand = New MySqlCommand("SELECT no As 'No.',stu_name As '生徒氏名',resalttbl.rank As '順位',stu_sta AS '結果' FROM classtbl INNER JOIN studenttbl ON classtbl.cls_code = studenttbl.cls_code INNER JOIN resalttbl ON studenttbl.stu_code = resalttbl.stu_code LEFT OUTER JOIN ranktbl ON resalttbl.rank = ranktbl.rank WHERE classtbl.cls_code = 'C" & i.ToString("00") & "' ORDER BY no", Con)

                OutAdapter.Fill(OutDGVDs)
                DataGridView5.DataSource = OutDGVDs.Tables(0)
            Catch ex As MySqlException
                MessageBox.Show("SQLErrorが発生しました。(" & ex.ErrorCode & ":" & ex.Message & ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Catch ex As Exception
                MessageBox.Show("エラーが発生しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                Try
                    Con.Close()
                Catch ex As Exception
                    MessageBox.Show("エラーが発生しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Try

            SheetObj = WorkBookObj.Worksheets(1)
            ExcelOutPut2(i, ExcelObj, WorkBookObj, SheetObj)
            Marshal.ReleaseComObject(SheetObj)
        Next

        'ExcelObj.Visible = True

        WorkBookObj.SaveAs(Filename:=CreateObject("WScript.Shell").SpecialFolders("Desktop") & "\クラス別生徒情報一覧.xlsx")
        WorkBookObj.Saved = True
        MessageBox.Show("デスクトップに出力しました。")

        WorkBookObj.Close()
        Marshal.ReleaseComObject(WorkBookObj)
        ExcelObj.Quit()
        Marshal.ReleaseComObject(ExcelObj)
        SheetObj = Nothing
        WorkBookObj = Nothing
        ExcelObj = Nothing

        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()





    End Sub
    Private Sub ExcelOutPut2(sheetnum As Integer, ExcelObj As Excel.Application, WorkBookObj As Excel.Workbook, SheetObj As Excel.Worksheet)
        Dim stCurrentDir As String = System.IO.Directory.GetCurrentDirectory()

        Try
            Dim ColumnMaxNum As Integer = DataGridView5.Columns.Count - 1
            Dim RowMaxNum As Integer = DataGridView5.Rows.Count - 1

            Dim ColumnList As New List(Of String)
            For i As Integer = 0 To (ColumnMaxNum)
                ColumnList.Add(DataGridView5.Columns(i).HeaderCell.Value)
            Next

            Dim v As String(,) = New String(RowMaxNum, ColumnMaxNum) {}
            For Row As Integer = 0 To RowMaxNum
                For Col As Integer = 0 To ColumnMaxNum
                    If DataGridView5.Rows(Row).Cells(Col).Value Is Nothing = False Then
                        v(Row, Col) = DataGridView5.Rows(Row).Cells(Col).Value.ToString()
                    End If
                Next
            Next

            For i As Integer = 1 To DataGridView5.Columns.Count
                SheetObj.Cells(5, i) = ColumnList(i - 1)

                SheetObj.Cells(5, i).Borders.LineStyle = True
                'WorkBookObj.Sheets(1).Cells(6, i).Interior.Color = RGB(140, 140, 140)
                WorkBookObj.Sheets(1).Cells(6, i).Font.Color = RGB(255, 255, 255)
                WorkBookObj.Sheets(1).Cells(6, i).Font.Bold = True
            Next

            Dim Data As String = "A6:" & Chr(Asc("A") + ColumnMaxNum) & DataGridView5.Rows.Count + 5
            WorkBookObj.Sheets(sheetnum).Range(Data) = v

            WorkBookObj.Sheets(1).Range(Data).Borders.LineStyle = True

        Catch ex As Exception
            MessageBox.Show("エラーが発生しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    '順位順
    Private Sub ExcOutput3_Click(sender As Object, e As EventArgs) Handles ExcOutput3.Click
        If OutDGVDs IsNot Nothing Then
            OutDGVDs = Nothing
            OutDGVDs = New DataSet
        End If
        'DataGridView5.DataSource = OutDGVDs.Tables
        'OutPutChecker("ExcOutput3")
        'DataGridView5.DataSource = OutDGVDs.Tables(0)

        Dim Path As String = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) & "\マラソン大会エクセルファイル"

        Dim ExcelObj As Excel.Application
        Dim WorkBookObj As Excel.Workbook
        Dim SheetObj As Excel.Worksheet

        ExcelObj = CreateObject("Excel.Application")
        WorkBookObj = ExcelObj.Workbooks.Open(Path & "\入賞者一覧.xlsx")

        OutDGVDs = New DataSet
        Try
            Con.Open()
            OutAdapter.SelectCommand = New MySqlCommand("SELECT resalttbl.rank AS '順位',cls_lname AS '科',no AS 'No.',stu_name AS '生徒氏名',time AS 'タイム',sei FROM resalttbl,classtbl,ranktbl,studenttbl WHERE classtbl.cls_code = studenttbl.cls_code AND studenttbl.stu_code = resalttbl.stu_code AND resalttbl.rank = ranktbl.rank AND resalttbl.rank < 201 ORDER BY resalttbl.rank", Con)

            OutAdapter.Fill(OutDGVDs)
            DataGridView5.DataSource = OutDGVDs.Tables(0)
        Catch ex As MySqlException
            MessageBox.Show("SQLErrorが発生しました。(" & ex.ErrorCode & ":" & ex.Message & ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            MessageBox.Show("エラーが発生しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Try
                Con.Close()
            Catch ex As Exception
            End Try
        End Try

        SheetObj = WorkBookObj.Worksheets(1)
        ExcelOutPut3(1, ExcelObj, WorkBookObj, SheetObj)
        Marshal.ReleaseComObject(SheetObj)

        'ExcelObj.Visible = True

        WorkBookObj.SaveAs(Filename:=CreateObject("WScript.Shell").SpecialFolders("Desktop") & "\入賞者一覧.xlsx")
        WorkBookObj.Saved = True
        MessageBox.Show("デスクトップに出力しました。")

        WorkBookObj.Close()
        Marshal.ReleaseComObject(WorkBookObj)
        ExcelObj.Quit()
        Marshal.ReleaseComObject(ExcelObj)
        SheetObj = Nothing
        WorkBookObj = Nothing
        ExcelObj = Nothing

        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()





    End Sub
    Private Sub ExcelOutPut3(sheetnum As Integer, ExcelObj As Excel.Application, WorkBookObj As Excel.Workbook, SheetObj As Excel.Worksheet)
        Dim stCurrentDir As String = System.IO.Directory.GetCurrentDirectory()
        Try
            Dim ColumnMaxNum As Integer = DataGridView5.Columns.Count - 2
            Dim RowMaxNum As Integer = DataGridView5.Rows.Count - 1
            For Row As Integer = 0 To RowMaxNum
                For Col As Integer = 0 To ColumnMaxNum
                    If DataGridView5.Rows(Row).Cells(Col).Value Is Nothing = False Then
                        If Row < 68 Then
                            SheetObj.Cells(3 + Row, Col + 1) = DataGridView5.Rows(Row).Cells(Col).Value.ToString()
                            If DataGridView5.Rows(Row).Cells(5).Value = 1 Then
                                SheetObj.Cells(3 + Row, Col + 1).Interior.Color = RGB(255, 153, 204)
                            End If
                        ElseIf Row < 136 Then
                            SheetObj.Cells(Row - 65, Col + 7) = DataGridView5.Rows(Row).Cells(Col).Value.ToString()
                            If DataGridView5.Rows(Row).Cells(5).Value = 1 Then
                                SheetObj.Cells(Row - 65, Col + 7).Interior.Color = RGB(255, 153, 204)
                            End If
                        Else
                            SheetObj.Cells(Row - 133, Col + 13) = DataGridView5.Rows(Row).Cells(Col).Value.ToString()
                            If DataGridView5.Rows(Row).Cells(5).Value = 1 Then
                                SheetObj.Cells(Row - 133, Col + 13).Interior.Color = RGB(255, 153, 204)
                            End If
                        End If
                    End If
                Next
            Next
            SheetObj.Range("E3:E70").NumberFormatLocal = "00'00"
            SheetObj.Range("K3:K70").NumberFormatLocal = "00'00"
            SheetObj.Range("Q3:Q66").NumberFormatLocal = "00'00"
        Catch ex As Exception
            MessageBox.Show("エラーが発生しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    '男女別上位入賞者
    Private Sub ExcOutPut4_Click(sender As Object, e As EventArgs) Handles ExcOutPut4.Click
        If OutDGVDs IsNot Nothing Then
            OutDGVDs = Nothing
            OutDGVDs = New DataSet
        End If
        Dim Path As String = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) & "\マラソン大会エクセルファイル"

        Dim ExcelObj As Excel.Application
        Dim WorkBookObj As Excel.Workbook
        Dim SheetObj As Excel.Worksheet

        ExcelObj = CreateObject("Excel.Application")
        WorkBookObj = ExcelObj.Workbooks.Open(Path & "\男女別上位入賞者一覧表.xlsx")

        OutDGVDs = New DataSet
        Try
            Con.Open()
            OutAdapter.SelectCommand = New MySqlCommand("SELECT resalttbl.rank AS '順位',cls_lname AS '科',no AS 'No.',stu_name AS '生徒氏名',time AS 'タイム',sei FROM resalttbl,classtbl,ranktbl,studenttbl WHERE classtbl.cls_code = studenttbl.cls_code AND studenttbl.stu_code = resalttbl.stu_code AND resalttbl.rank = ranktbl.rank ORDER BY resalttbl.rank", Con)

            OutAdapter.Fill(OutDGVDs)
            DataGridView5.DataSource = OutDGVDs.Tables(0)
        Catch ex As MySqlException
            MessageBox.Show("SQLErrorが発生しました。(" & ex.ErrorCode & ":" & ex.Message & ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            MessageBox.Show("エラーが発生しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Try
                Con.Close()
            Catch ex As Exception
            End Try
        End Try

        SheetObj = WorkBookObj.Worksheets(1)
        ExcelOutPut4(1, ExcelObj, WorkBookObj, SheetObj)
        Marshal.ReleaseComObject(SheetObj)

        WorkBookObj.SaveAs(Filename:=CreateObject("WScript.Shell").SpecialFolders("Desktop") & "\男女別上位入賞者一覧表.xlsx")
        WorkBookObj.Saved = True
        MessageBox.Show("デスクトップに出力しました。")

        WorkBookObj.Close()
        Marshal.ReleaseComObject(WorkBookObj)
        ExcelObj.Quit()
        Marshal.ReleaseComObject(ExcelObj)
        SheetObj = Nothing
        WorkBookObj = Nothing
        ExcelObj = Nothing

        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()

    End Sub
    Private Sub ExcelOutPut4(sheetnum As Integer, ExcelObj As Excel.Application, WorkBookObj As Excel.Workbook, SheetObj As Excel.Worksheet)
        Dim stCurrentDir As String = System.IO.Directory.GetCurrentDirectory()
        Dim Count0 As Integer = 0
        Dim Count1 As Integer = 0
        Try
            Dim ColumnMaxNum As Integer = DataGridView5.Columns.Count - 2
            Dim RowMaxNum As Integer = DataGridView5.Rows.Count - 1
            For Row As Integer = 0 To RowMaxNum
                For Col As Integer = 0 To ColumnMaxNum
                    If DataGridView5.Rows(Row).Cells(Col).Value Is Nothing = False Then
                        If DataGridView5.Rows(Row).Cells(5).Value = 1 And Count1 < 10 Then
                            SheetObj.Cells(Count1 + 30, Col + 2) = DataGridView5.Rows(Row).Cells(Col).Value.ToString()
                            Flag = 0
                        ElseIf Count0 < 20 Then
                            SheetObj.Cells(7 + Count0, Col + 2) = DataGridView5.Rows(Row).Cells(Col).Value.ToString()
                            Flag = 1
                        Else
                            Flag = 2
                        End If
                    End If
                Next
                If Flag = 0 Then
                    Count1 = Count1 + 1
                ElseIf Flag = 1 Then
                    Count0 = Count0 + 1
                End If
            Next
            SheetObj.Range("F7:F26").NumberFormatLocal = "00'00"
            SheetObj.Range("F30:F39").NumberFormatLocal = "00'00"
        Catch ex As Exception
            MessageBox.Show("エラーが発生しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


End Class
