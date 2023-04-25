$ftp = "access949598297.webspace-data.io"
$user = ""
$pass = ""

$file = "c:\junk\HelloWorld.txt"
c:
cd \junk


# Load the WinSCP assembly
Add-Type -Path "C:\junk\WinSCP\WinSCPnet.dll"

# Set up session options
$sessionOptions = New-Object WinSCP.SessionOptions -Property @{
    Protocol = [WinSCP.Protocol]::Sftp
    HostName = $ftp
    UserName = $user
    Password = $pass
    PortNumber = 22
    SshHostKeyFingerprint = "0d:15:61:04:10:c2:d7:ce:af:55:68:0a:7c:9c:b8:d7"
}


# Connect to the SFTP server
$session = New-Object WinSCP.Session
$session.SessionLogPath = "c:\junk\logfile.txt"
$session.Open($sessionOptions)

# Upload a file
$session.PutFiles($file, "/Apps/Uat/HelloWorld.txt").Check()

# Disconnect from the SFTP server
$session.Dispose()
