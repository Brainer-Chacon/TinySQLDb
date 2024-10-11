# Define los par�metros que se recibir�n al ejecutar el script
param (
    [Parameter(Mandatory = $true)]
    [string]$IP,  # Direcci�n IP del servidor
    [Parameter(Mandatory = $true)]
    [int]$Port,    # Puerto del servidor
    [Parameter(Mandatory = $true)]
    [string]$QueryFile  # Ruta del archivo SQL
)

# Define la funci�n principal para ejecutar consultas SQL desde un archivo
function Execute-MyQuery {
    param (
        [Parameter(Mandatory = $true)]
        [string]$QueryFile,  # Ruta del archivo SQL
        [Parameter(Mandatory = $true)]
        [string]$IP,         # Direcci�n IP del servidor
        [Parameter(Mandatory = $true)]
        [int]$Port           # Puerto del servidor
    )

    # Crear el endpoint utilizando la direcci�n IP y el puerto proporcionados
    $ipEndPoint = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Parse($IP), $Port)

    # Leer el archivo de consultas SQL
    if (Test-Path $QueryFile) {
        # Leer el contenido del archivo y separar las sentencias SQL usando ';' como delimitador
        $sqlCommands = Get-Content -Path $QueryFile -Raw
        $sqlCommands = $sqlCommands -split ';'  # Separa las sentencias SQL
    } else {
        Write-Error "El archivo SQL no se encuentra."  # Maneja el error si el archivo no existe
        return
    }

    # Funci�n para enviar mensajes al servidor
    function Send-Message {
        param (
            [Parameter(Mandatory = $true)]
            [pscustomobject]$message,  # Mensaje que se va a enviar
            [Parameter(Mandatory = $true)]
            [System.Net.Sockets.Socket]$client  # Socket del cliente
        )

        $stream = New-Object System.Net.Sockets.NetworkStream($client)
        $writer = New-Object System.IO.StreamWriter($stream)
        try {
            $writer.WriteLine($message)  # Env�a el mensaje al servidor
            $writer.Flush()  # Asegura que el mensaje se env�e inmediatamente
        }
        finally {
            $writer.Close()  # Cierra el escritor
            $stream.Close()  # Cierra el stream
        }
    }

    # Funci�n para recibir mensajes del servidor
    function Receive-Message {
        param (
            [System.Net.Sockets.Socket]$client  # Socket del cliente
        )
        $stream = New-Object System.Net.Sockets.NetworkStream($client)
        $reader = New-Object System.IO.StreamReader($stream)
        try {
            # Lee la respuesta del servidor
            return $reader.ReadLine() # Lee una l�nea de la respuesta
        }
        finally {
            $reader.Close()  # Cierra el lector
            $stream.Close()  # Cierra el stream
        }
    }

    # Funci�n para enviar un comando SQL y recibir el resultado
    function Send-SQLCommand {
        param (
            [string]$command  # Comando SQL a enviar
        )
        
        # Manejo de excepciones al intentar conectar
        try {
            # Crea un nuevo socket para conectarse al servidor
            $client = New-Object System.Net.Sockets.Socket($ipEndPoint.AddressFamily, [System.Net.Sockets.SocketType]::Stream, [System.Net.Sockets.ProtocolType]::Tcp)
            $client.Connect($ipEndPoint)  # Conecta al servidor
        }
        catch {
            Write-Error "No se pudo establecer conexi�n: $_"
            return $null
        }

        # Crea el objeto de solicitud que se enviar�
        $requestObject = [PSCustomObject]@{
            RequestType = 0;  # Asume que 0 corresponde a SQL
            RequestBody = $command  # Cuerpo del mensaje
        }

        # Convierte el objeto de solicitud a formato JSON
        $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress
        
        # Manejo de excepciones al enviar y recibir mensajes
        try {
            Send-Message -client $client -message $jsonMessage  # Env�a el mensaje al servidor
            $response = Receive-Message -client $client  # Recibe la respuesta del servidor
        }
        catch {
            Write-Error "Error al enviar o recibir datos: $_"
            return $null
        }
        finally {
            $client.Shutdown([System.Net.Sockets.SocketShutdown]::Both)  # Cierra la conexi�n
            $client.Close()  # Cierra el socket
        }

        return $response;  # Devuelve la respuesta del servidor
    }

    # Ejecutar cada sentencia SQL del archivo
    foreach ($command in $sqlCommands) {
        if (-not [string]::IsNullOrWhiteSpace($command)) {  # Verifica que el comando no est� vac�o
            Write-Host -ForegroundColor Green "`nEjecutando comando: $command"  # Muestra el comando que se va a ejecutar

            # Medir el tiempo que tarda en ejecutar el comando
            $executionTime = Measure-Command {
                $response = Send-SQLCommand -command $command  # Env�a el comando SQL
            }

            # Convertir la respuesta en un objeto PowerShell
            $responseObject = ConvertFrom-Json -InputObject $response

            # Mostrar el resultado en formato tabla
            if ($responseObject -and $responseObject.result) {
                $responseObject.result | Format-Table -AutoSize  # Formatea y muestra la salida
            } else {
                Write-Host -ForegroundColor Red "Error en la ejecuci�n o respuesta vac�a."  # Manejo de errores si la respuesta es vac�a
            }

            # Mostrar el tiempo que tard� la ejecuci�n del comando
            Write-Host -ForegroundColor Yellow "Tiempo de ejecuci�n: $($executionTime.TotalMilliseconds) ms"  # Muestra el tiempo en milisegundos
        }
    }
}

# Llama a la funci�n Execute-MyQuery al final del script
Execute-MyQuery -QueryFile $QueryFile -IP $IP -Port $Port

