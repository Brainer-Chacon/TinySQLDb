param (
    [Parameter(Mandatory = $true)]
    [string]$IP,
    [Parameter(Mandatory = $true)]
    [int]$Port
)

function Execute-MyQuery {
    param (
        [Parameter(Mandatory = $true)]
        [string]$QueryFile,  # Path del archivo SQL
        [Parameter(Mandatory = $true)]
        [string]$IP,         # Dirección IP del servidor
        [Parameter(Mandatory = $true)]
        [int]$Port           # Puerto del servidor
    )

    # Crear el endpoint con los parámetros proporcionados
    $ipEndPoint = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Parse($IP), $Port)

    # Leer el archivo de consulta SQL
    if (Test-Path $QueryFile) {
        $sqlCommands = Get-Content -Path $QueryFile -Raw -Delimiter ';'
    } else {
        Write-Error "El archivo SQL no se encuentra."
        return
    }

    # Función para enviar mensajes al servidor
    function Send-Message {
        param (
            [Parameter(Mandatory = $true)]
            [pscustomobject]$message,
            [Parameter(Mandatory = $true)]
            [System.Net.Sockets.Socket]$client
        )

        $stream = New-Object System.Net.Sockets.NetworkStream($client)
        $writer = New-Object System.IO.StreamWriter($stream)
        try {
            $writer.WriteLine($message)
            $writer.Flush()
        }
        finally {
            $writer.Close()
            $stream.Close()
        }
    }

    # Función para recibir mensajes del servidor
    function Receive-Message {
        param (
            [System.Net.Sockets.Socket]$client
        )
        $stream = New-Object System.Net.Sockets.NetworkStream($client)
        $reader = New-Object System.IO.StreamReader($stream)
        try {
            return $reader.ReadLine() -ne $null ? $reader.ReadLine() : ""
        }
        finally {
            $reader.Close()
            $stream.Close()
        }
    }

    # Función para enviar un comando SQL y recibir el resultado
    function Send-SQLCommand {
        param (
            [string]$command
        )
        $client = New-Object System.Net.Sockets.Socket($ipEndPoint.AddressFamily, [System.Net.Sockets.SocketType]::Stream, [System.Net.Sockets.ProtocolType]::Tcp)
        $client.Connect($ipEndPoint)

        $requestObject = [PSCustomObject]@{
            RequestType = 0;  # Asume que 0 corresponde a SQL
            RequestBody = $command
        }
        $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress
        Send-Message -client $client -message $jsonMessage
        $response = Receive-Message -client $client

        $client.Shutdown([System.Net.Sockets.SocketShutdown]::Both)
        $client.Close()

        return $response
    }

    # Ejecutar cada sentencia SQL del archivo
    foreach ($command in $sqlCommands) {
        if (-not [string]::IsNullOrWhiteSpace($command)) {
            Write-Host -ForegroundColor Green "`nEjecutando comando: $command"

            # Medir el tiempo de ejecución
            $executionTime = Measure-Command {
                $response = Send-SQLCommand -command $command
            }

            # Convertir la respuesta en objeto PowerShell
            $responseObject = ConvertFrom-Json -InputObject $response

            # Mostrar el resultado en formato tabla
            if ($responseObject -and $responseObject.result) {
                $responseObject.result | Format-Table -AutoSize
            } else {
                Write-Host -ForegroundColor Red "Error en la ejecución o respuesta vacía."
            }

            # Mostrar el tiempo que tardó la ejecución
            Write-Host -ForegroundColor Yellow "Tiempo de ejecución: $($executionTime.TotalMilliseconds) ms"
        }
    }
}

# Ejemplo de uso:
# Execute-MyQuery -QueryFile ".\Script.tinysql" -IP "10.0.0.2" -Port 8000
