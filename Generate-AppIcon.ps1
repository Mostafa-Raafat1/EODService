<#
.SYNOPSIS
    Converts a PNG image into a multi-resolution Windows .ICO file (256, 128, 64, 48, 32, 16 px).
.EXAMPLE
    .\Generate-AppIcon.ps1 -SourcePng path\to\logo.png
#>
param(
    [string] = EODSettingsApp\app.ico,
    [string] = 
)

Add-Type -AssemblyName System.Drawing

if (-not (Test-Path )) {
    Write-Error Source image not found at ''
    exit 1
}

 = Join-Path  EODSettingsApp\app.ico
  = Join-Path  EODService\app.ico

 = [System.Drawing.Image]::FromFile((Resolve-Path ).Path)
 = @(256, 128, 64, 48, 32, 16)
 = @()

foreach ( in ) {
     = New-Object System.Drawing.Bitmap(, , [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
     = [System.Drawing.Graphics]::FromImage()
    .InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    .SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    .PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    .Clear([System.Drawing.Color]::Transparent)
    .DrawImage(, 0, 0, , )
    .Dispose()
    
     = New-Object System.IO.MemoryStream
    .Save(, [System.Drawing.Imaging.ImageFormat]::Png)
     += ,@(, .ToArray())
    .Dispose()
}
.Dispose()

function Write-IcoFile(, ) {
     = Split-Path -Parent 
    if (-not (Test-Path )) { New-Item -ItemType Directory -Path  -Force | Out-Null }
    
     = [System.IO.File]::Create()
     = New-Object System.IO.BinaryWriter()

    # ICONDIR header
    .Write([UInt16]0) # Reserved
    .Write([UInt16]1) # Type: 1 = ICO
    .Write([UInt16].Count) # Count of images

     = 6 + (16 * .Count)

    foreach ( in ) {
         = [0]
         = [1]
        
         = if ( -ge 256) { 0 } else {  }
         = if ( -ge 256) { 0 } else {  }

        # ICONDIRENTRY (16 bytes)
        .Write([byte])
        .Write([byte])
        .Write([byte]0)
        .Write([byte]0)
        .Write([UInt16]1)
        .Write([UInt16]32)
        .Write([UInt32].Length)
        .Write([UInt32])
        
         += .Length
    }

    foreach ( in ) {
         = [1]
        .Write()
    }

    .Flush()
    .Close()
    .Close()
    Write-Host Created ( bytes)
}

Write-IcoFile  
Write-IcoFile  
