Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json

Namespace Model
#Disable Warning IDE1006
#Disable Warning InconsistentNaming

    Partial Public Class HolData 
        Public Property meta As Meta
        Public Property response As Response
    End Class

    Partial Public Class Meta
        Public Property code As Integer
    End Class

    Partial Public Class Response
        Public Property holidays() As Holiday()
    End Class

    Partial Public Class Holiday
        Public Property name As String
        Public Property description As String
        Public Property _date As _Date
        Public Property type() As String()
        Public Property locations As String
        Public Property states As Object
    End Class

    Partial Public Class States
        Public Property id As Integer
        Public Property abbrev As String
        Public Property name As String
        Public Property exception As String
        Public Property iso As String
    End Class

    Partial Public Class _Date
        Public Property iso As String
        Public Property _datetime As _Datetime
        Public Property timezone As Timezone
    End Class

    Partial Public Class _Datetime
        Public Property year As Integer
        Public Property month As Integer
        Public Property day As Integer
        Public Property hour As Integer
        Public Property minute As Integer
        Public Property second As Integer
    End Class

    Partial Public Class Timezone
        Public Property offset As String
        Public Property zoneabb As String
        Public Property zoneoffset As Integer
        Public Property zonedst As Integer
        Public Property zonetotaloffset As Integer
    End Class

    Partial Public Class HolData

        Public Shared Function FromJson(json As String) As HolData
            Dim settings = New JsonSerializerSettings With {
                     .NullValueHandling = NullValueHandling.Ignore,
                     .MissingMemberHandling = MissingMemberHandling.Ignore
                     }
            Return JsonConvert.DeserializeObject(Of HolData)(json, settings)
        End Function

    End Class

    Module Serialize

        <Extension()>
        Function ToJson(self As HolData) As String
            Return JsonConvert.SerializeObject(self, Converter.Settings)
        End Function

    End Module

    Public Class Converter

        Public Shared ReadOnly _
            Settings As JsonSerializerSettings = New JsonSerializerSettings _
            With {.MetadataPropertyHandling = MetadataPropertyHandling.Ignore, .DateParseHandling = DateParseHandling.None}

    End Class

End Namespace