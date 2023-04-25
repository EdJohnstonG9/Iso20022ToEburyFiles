$Dir="D:\VisualStudio\Source\GitHub\Iso20022ToEburyFiles\EburyMPIsoFiles\bin\Release\app.publish\"
$Target="/Apps/Uat/Iso20022ToEburyFileSoln"

$ftpHost = "ftp://access949598297.webspace-data.io"
$user = ""
$pass = ""

cd $Dir
$files = Get-ChildItem -Recurse | Resolve-Path -Relative
#list every file
foreach($item in ($files)){
    $targetFile=$ftpHost+$Target+$item.Substring(1)
    $uri = New-Object System.Uri($targetFile)
    "Uploading $item..."
    echo $uri.AbsoluteUri

     # create the FtpWebRequest and configure it
    $ftp = [System.Net.FtpWebRequest]::Create($uri.AbsoluteUri)
    $ftp = [System.Net.FtpWebRequest]$ftp
    $ftp.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile
    $ftp.Credentials = new-object System.Net.NetworkCredential($user,$pass)
    $ftp.UseBinary = $true
    $ftp.UsePassive = $true
    # read in the file to upload as a byte array
    $content = [System.IO.File]::ReadAllBytes($item)
    $ftp.ContentLength = $content.Length
    # get the request stream, and write the bytes into it
    $rs = $ftp.GetRequestStream()
    $rs.Write($content, 0, $content.Length)
    # be sure to clean up after ourselves
    $rs.Close()
    $rs.Dispose()
 } 

