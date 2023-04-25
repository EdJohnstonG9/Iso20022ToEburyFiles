$Dir="D:\VisualStudio\Source\GitHub\Iso20022ToEburyFiles\EburyMPIsoFiles\bin\Release\app.publish\"
$Target="/Apps/Uat/Iso20022ToEburyFileSoln"

$ftpHost = "ftp://access949598297.webspace-data.io"
$user = ""
$pass = ""

#$client = New-Object System.Net.WebClient
#$client.BaseAddress = $ftpHost
#$client.Credentials = New-Object System.Net.NetworkCredential($user, $pass)
#$client.UploadFile(
$remote="ftp://access949598297.webspace-data.io/Apps/Uat/Iso20022ToEburyFileSoln/HelloWorld.txt"
$local="D:\junk\HelloWorld.txt"


#$webclient = New-Object System.Net.WebClient

#$webclient.Credentials = New-Object System.Net.NetworkCredential($user,$pass)
echo $remote
echo $local
#function uploadToFTPServer($remote, $local) {
    $request = [System.Net.FtpWebRequest]::Create($remote)
    $request.Credentials = New-Object System.Net.NetworkCredential($user, $pass)
    $request.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile
    $request.UsePassive = $true
    $request.KeepAlive = $true
    $request.UseBinary = $true
    $request.EnableSsl = $true

    $fileStream = [System.IO.File]::OpenRead($local)
    $ftpStream = $request.GetRequestStream()

    $fileStream.CopyTo($ftpStream)

    $ftpStream.Dispose()
    $fileStream.Dispose()
#}

uploadToFTPServer([System.String]"ftp://access949598297.webspace-data.io/Apps/Uat/Iso20022ToEburyFileSoln/HelloWorld.txt", "D:\junk\HelloWorld.txt")

cd $Dir
$files = Get-ChildItem -Recurse | Resolve-Path -Relative
#list every file
foreach($item in ($files)){
    $targetFile=$ftpHost+$Target+$item.Substring(1)
    $uri = New-Object System.Uri($targetFile)
    $uri = [System.Uri]$uri
    echo "Uploading ..."
    echo $item
    echo $uri.AbsoluteUri
    uploadToFTPServer("ftp://access949598297.webspace-data.io/Apps/Uat/Iso20022ToEburyFileSoln/Application%20Files/EburyMPIsoFiles_1_3_6_57/Resources/AppIconIso.ico.deploy", $item)
 } 
