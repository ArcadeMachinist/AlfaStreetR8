Imports System.IO.Ports

Public Class Form1
    'Dim sp As New SerialPort()
    Dim rcvQ As New List(Of Byte)
    Dim rcvQlock As New Object

    Dim g_state As Int16 = 0
    Dim g_ticks As Int16 = 0
    Dim g_ballseen As Boolean = False
    Dim g_ballannounced As Boolean = False
    Dim g_ball As Byte


    Dim c_init() As Byte = {&HAC, &H80, &H10, &H0, &H90}
    Dim c_speed1() As Byte = {&HAC, &H80, &HC0, &H80, &HC0}
    Dim c_speed2() As Byte = {&HAC, &H80, &H80, &H80, &H80}
    Dim c_speed3() As Byte = {&HAC, &H80, &H80, &H82, &H82}
    Dim c_speed4() As Byte = {&HAC, &H80, &H40, &H80, &H40}
    Dim c_speed5() As Byte = {&HAC, &H80, &HA0, &H80, &HA0}
    Dim c_speed6() As Byte = {&HAC, &H80, &H60, &H80, &H60}
    Dim c_eject() As Byte = {&HAC, &H80, &H40, &H82, &H42}
    Dim c_eatball() As Byte = {&HAC, &H80, &HE0, &H81, &HE1}
    Dim c_stop() As Byte = {&HAC, &H80, &HE0, &H80, &HE0}
    Dim c_wait() As Byte = {&HAC, &H80, &H0, &H80, &H0}

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        sp.PortName = "COM4"
        sp.BaudRate = 19200
        sp.Parity = Parity.None
        sp.DataBits = 8
        sp.StopBits = StopBits.One


        sp.Open()

        sp.Write(c_init, 0, c_init.Length)

        Timer1.Interval = 500 'one second
        Timer1.Start()
    End Sub


    Private Sub DataReceivedHandler(sender As System.Object,
                                         e As System.IO.Ports.SerialDataReceivedEventArgs) Handles sp.DataReceived
        My.Application.Log.WriteEntry("Data Receive")

        Dim n As Integer = sp.BytesToRead 'find number of bytes in buf
        Dim comBuffer(n - 1) As Byte
        sp.Read(comBuffer, 0, n) 'read data from the buffer
        Threading.Monitor.Enter(rcvQlock) 'add to q
        rcvQ.AddRange(comBuffer)
        Threading.Monitor.Exit(rcvQlock)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        g_state = 0
        g_ballseen = False
        g_ballannounced = False


        Dim cbytes() As Byte = {&HAC, &H80, &HE0, &H80, &HE0}
        sp.Write(cbytes, 0, cbytes.Length)
        '  My.Computer.Audio.Play("C:\AS8\wav\OPEN.VOC.wav", AudioPlayMode.WaitToComplete)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim cbytes() As Byte = {&HAC, &H80, &HC0, &H80, &HC0}
        sp.Write(cbytes, 0, cbytes.Length)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim cbytes() As Byte = {&HAC, &H80, &HE0, &H84, &HE4}
        sp.Write(cbytes, 0, cbytes.Length)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim cbytes() As Byte = {&HAC, &H80, &HE0, &H88, &HE8}
        sp.Write(cbytes, 0, cbytes.Length)
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim cbytes() As Byte = {&HAC, &H80, &H40, &H82, &H42}
        sp.Write(cbytes, 0, cbytes.Length)
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim cbytes() As Byte = {&HAC, &H80, &HE0, &H81, &HE1}
        sp.Write(cbytes, 0, cbytes.Length)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim cbytes() As Byte = {&HAC, &H80, &H0, &H80, &H0}
        sp.Write(cbytes, 0, cbytes.Length)
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim cbytes() As Byte = {&HAC, &H80, &H60, &H80, &H60}
        sp.Write(cbytes, 0, cbytes.Length)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        ' My.Application.Log.WriteEntry("Timer tick")

        Select Case g_state
            Case 1 'Lay your betz
                sp.Write(c_speed1, 0, c_speed1.Length)
                My.Computer.Audio.Play("C:\AS8\wav\OPEN.VOC.wav", AudioPlayMode.WaitToComplete)
                sp.Write(c_speed1, 0, c_speed1.Length)
                g_state = 2
                g_ticks = 0
            Case 2 'set speed
                sp.Write(c_speed1, 0, c_speed1.Length)
                g_ticks = g_ticks + 1
                If g_ticks > 20 Then
                    g_ticks = 0
                    g_state = 3
                End If
            Case 3  'another ss
                g_ticks = g_ticks + 1
                If g_ticks < 10 Then
                    sp.Write(c_speed2, 0, c_speed2.Length)
                End If
                If g_ticks > 10 Then
                    sp.Write(c_speed6, 0, c_speed6.Length)
                    g_ticks = 0
                    g_state = 4
                End If
            Case 4
                g_ticks = g_ticks + 1
                If g_ticks < 2 Then
                    sp.Write(c_eject, 0, c_speed6.Length)
                Else
                    sp.Write(c_speed1, 0, c_speed6.Length)
                End If
                If (g_ticks > 18) Then
                    Label1.Text = "No more bets"
                    Application.DoEvents()

                    My.Computer.Audio.Play("C:\AS8\wav\CLOSE.VOC.wav", AudioPlayMode.WaitToComplete)
                    g_state = 5
                    g_ticks = 0
                End If
            Case 5
                g_ticks = g_ticks + 1
                sp.Write(c_speed1, 0, c_speed6.Length)
                If g_ballseen = True And
                    g_ballannounced = False Then
                    'Label1.Text = "Ball!"
                    g_ballannounced = True
                    Select Case g_ball
                        Case &H0
                            Label1.Text = "6 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N6.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)

                        Case &H1
                            Label1.Text = "34 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N34.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)

                        Case &H2
                            Label1.Text = "17 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N17.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H3
                            Label1.Text = "25 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N25.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H4
                            Label1.Text = "2 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N2.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H5
                            Label1.Text = "21 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N21.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H6
                            Label1.Text = "4 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N4.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H7
                            Label1.Text = "19 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N19.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H8
                            Label1.Text = "15 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N15.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H9
                            Label1.Text = "32 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N32.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &HA
                            Label1.Text = "0 ZERO"
                            Label1.ForeColor = Color.DarkGreen
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N0.VOC.wav", AudioPlayMode.WaitToComplete)

                        Case &HB
                            Label1.Text = "26 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N26.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &HC
                            Label1.Text = "3 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N3.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &HD
                            Label1.Text = "35 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N35.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &HE
                            Label1.Text = "12 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N12.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &HF
                            Label1.Text = "28 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N28.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H10
                            Label1.Text = "7 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N7.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H11
                            Label1.Text = "29 BLACK"

                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N29.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H12
                            Label1.Text = "18 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N18.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H13
                            Label1.Text = "22 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N22.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H14
                            Label1.Text = "9 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N9.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H15
                            Label1.Text = "31 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N31.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H16
                            Label1.Text = "14 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N14.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H17
                            Label1.Text = "20 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N20.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H18
                            Label1.Text = "1 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N1.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H19
                            Label1.Text = "33 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N33.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H1A
                            Label1.Text = "16 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N16.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H1B
                            Label1.Text = "24 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N24.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H1C
                            Label1.Text = "5 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N5.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H1D
                            Label1.Text = "10 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N10.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H1E
                            Label1.Text = "23 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N23.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H1F
                            Label1.Text = "8 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N8.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H20
                            Label1.Text = "30 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N30.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H21
                            Label1.Text = "11 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N11.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H22
                            Label1.Text = "36 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N36.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H23
                            Label1.Text = "13 BLACK"
                            Label1.ForeColor = Color.Black
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N13.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\BLACK.VOC.wav", AudioPlayMode.WaitToComplete)
                        Case &H24
                            Label1.Text = "27 RED"
                            Label1.ForeColor = Color.Red
                            Application.DoEvents()
                            My.Computer.Audio.Play("C:\AS8\wav\N27.VOC.wav", AudioPlayMode.WaitToComplete)
                            My.Computer.Audio.Play("C:\AS8\wav\RED.VOC.wav", AudioPlayMode.WaitToComplete)


                    End Select
                    g_state = 6
                    g_ticks = 0
                End If
            Case 6
                g_ticks = g_ticks + 1
                If g_ticks < 20 Then
                    sp.Write(c_stop, 0, c_stop.Length)
                Else
                    sp.Write(c_eatball, 0, c_stop.Length)
                    g_ticks = 0
                    g_state = 7
                End If
            Case 7
                If g_ballseen = True Then
                    sp.Write(c_wait, 0, c_stop.Length)
                Else
                    g_ticks = 0
                    g_state = 1
                End If
        End Select




        Dim strt As Integer = rcvQ.IndexOf(&H57)
        If strt < 0 Then
            My.Application.Log.WriteEntry("strt < 0")
            'no 98
        ElseIf strt > 0 Then
            Threading.Monitor.Enter(rcvQlock)
            rcvQ.RemoveRange(0, strt)
            Threading.Monitor.Exit(rcvQlock)
            strt = 0
        End If

        If rcvQ.Count >= 5 AndAlso strt = 0 Then
            Threading.Monitor.Enter(rcvQlock)
            Dim buf() As Byte = rcvQ.GetRange(0, 5).ToArray()
            rcvQ.RemoveRange(0, 5)
            Threading.Monitor.Exit(rcvQlock)
            'at this point buf has 5 bytes

            If buf(1) = &H1 Then
                If g_state = 5 Then
                    Label1.Text = "No more bets"
                Else
                    Label1.Text = "Make bets"
                End If
                System.Windows.Forms.Application.DoEvents()
                Label1.ForeColor = Color.DarkBlue
                g_ballseen = False
                g_ballannounced = False
            ElseIf buf(1) = &H21 Then
                Label1.Text = "Reloading ball"
                Label1.ForeColor = Color.DarkBlue
                g_ballseen = False
                g_ballannounced = False
            ElseIf buf(1) = &H51 Then
                g_ballseen = True
                g_ball = buf(3)
            End If
        End If

    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        g_state = 1
        g_ballseen = False

    End Sub
End Class
