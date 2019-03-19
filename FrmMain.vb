Imports System.IO
Imports System.Net
Imports CalendarificFetch.Model
Imports Microsoft.Win32

Public Class FrmMain

    ''https://calendarific.com/
    Private ReadOnly _cFile As String = $"cal-{Now.Month}_{Now.Year}.json"

    Private _resp As String = ""

    ''enter your API key from Calendarific below.
    ''this has been disabled in favor of storing key in registry
    'Private Const CalendarApiKey = "<Enter you Calendarific API key here>"
    ''create variable to hold write API key to Registry
    Private ReadOnly _kApi As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\Calendarific\API")
    Private _hNfo As HolData

    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        With Me
            .Text = Application.ProductName
            .TxtApi.Text = _kApi.GetValue(My.Resources.reg_key, "").ToString()
            .Show()
        End With


        ''check to see if the APi key has been saved to the registry.  If it has, process the data
        ''if it has not, send the user to the "Log" tab to store the API key
        If TxtApi.Text <> "" Then
            Cursor = Cursors.WaitCursor
            DownloadData()
            Cursor = Cursors.Default
        Else
            Rtb.AppendText($"You must first enter your Calendarific API Key to download the data.{vbCrLf}")
            Tc.SelectedIndex = 1
            TxtApi.Focus()
        End If
    End Sub

    Private Sub DownloadData()

        ''if the file exists for this month/year, no need to download again.  Load the data from the cached file
        ''It is set to download a new file each month, in case there are changes to the data
        If File.Exists(_cFile) Then
            Rtb.AppendText($"Reading cached file: {_cFile}{vbCrLf}")
            Dim tsf = New StreamReader(_cFile)
            _resp = tsf.ReadToEnd()
            Rtb.AppendText($"Holiday Data loaded from cache @ {Now:T}{vbCrLf}{vbCrLf}")
            _hNfo = HolData.FromJson(_resp)
            tsf.Close()
        Else
            Rtb.AppendText($"Downloading file: {_cFile}{vbCrLf}")
            ''if the file does not exist, download the current data
            ''to fetch data from another country, change the appropriate country data in the url below.
            ''Ex: US = United States
            ''a list of supported countries found here:
            ''https://calendarific.com/supported-countries
            Dim calendarApiKey As String = _kApi.GetValue(My.Resources.reg_key, "").ToString()
            Dim url =
                $"https://www.calendarific.com/api/v2/holidays?&country=US&year=2019&api_key={calendarApiKey}"

            Try
                Dim request = CType(WebRequest.Create(url), HttpWebRequest)
                Dim response = CType(request.GetResponse(), HttpWebResponse)
                Dim dStr = response.GetResponseStream()
                Dim reader As New StreamReader(dStr)
                _resp = reader.ReadToEnd().Replace("l_n", "ln").Replace("_d", "d").Replace("\u2019", "'").Replace("date", "_date").Replace("l ho", "l Ho").Replace("observance", "Observance").Replace("ing ev", "ing Ev")

                ''cache the current data
                File.WriteAllText(_cFile, _resp)
                _hNfo = HolData.FromJson(_resp)
                reader.Close()
                response.Close()
            Catch ex As Exception
                Rtb.AppendText(ex.Message)
            Finally
                ''
            End Try
        End If
        WriteCalendarData()
    End Sub

    Private Sub WriteCalendarData()
        'For j = 0 To _hNfo.response.holidays.Count() - 1
        Try
            Dim rows As DataGridViewRowCollection = Dgv.Rows
            Dim hh = _hNfo.response.holidays()
            For j = 0 To hh.Count() - 1
                'Rtb.AppendText($"Holiday: {hh(j).name}{vbCrLf}")
                'For k = 0 To hh(j).type.Count() - 1
                '    Rtb.AppendText($"Type: {hh(j).type(k)}{vbCrLf}")
                'Next
                'Rtb.AppendText($"{hh(j).description}{vbCrLf}")
                'Dim aa = hh(j)._date.iso
                'Dim tt As Date = Date.Parse(aa.ToString())
                'Rtb.AppendText($"Date: {tt.ToString("MMMM d, yyyy")}{vbCrLf}")

                'If hh(j).states().ToString() = "All" Then
                '    Rtb.AppendText($"States: All{vbCrLf}")
                'Else
                '    Rtb.AppendText($"Locations: {hh(j).locations}{vbCrLf}")
                'End If

                'Rtb.AppendText($"{vbCrLf}")

                rows.Add($"{hh(j).name}", CDate(hh(j)._date.iso).ToString("MMM d"), $"{hh(j).description}", My.Resources.cht_downarrows)
                For k = 0 To hh(j).type.Count - 1
                    If hh(j).states().ToString() = "All" Then
                        rows.Add("", "", $"States: All", $"{hh(j).type(k)}")
                    Else
                        rows.Add("", "", $"Locations: {hh(j).locations}", $"{hh(j).type(k)}")
                    End If
                    Application.DoEvents()
                Next

            Next
            Catch ex As Exception
            Rtb.AppendText(ex.Message & vbCrLf & ex.StackTrace & vbCrLf & ex.GetBaseException().ToString() & vbCrLf & ex.TargetSite.ToString())
        Finally
            ''
        End Try
    End Sub

    Private Sub TxtApi_TextChanged(sender As Object, e As EventArgs) Handles TxtApi.TextChanged
        _kApi.SetValue(My.Resources.reg_key, TxtApi.Text, RegistryValueKind.String)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TxtApi.Text <> "" Then
            Cursor = Cursors.WaitCursor
            DownloadData()
            Cursor = Cursors.Default
        Else
            Rtb.AppendText($"You must first enter your Calendarific API Key to download the data.{vbCrLf}")
            Tc.SelectedIndex = 1
            TxtApi.Focus()
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Try
            Process.Start("https://calendarific.com")
        Catch ex As Exception
            Rtb.AppendText(ex.Message & vbCrLf)
        End Try
    End Sub

End Class