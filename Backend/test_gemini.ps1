$apiKey = "AIzaSyD6Db33VrtUWgRF7Nwk587TJpUQxB3X6hs"
$url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=$apiKey"

$body = @{
    system_instruction = @{
        parts = @( @{ text = "Sen bir asistansın." } )
    }
    contents = @(
        @{ parts = @( @{ text = "Merhaba" } ) }
    )
} | ConvertTo-Json -Depth 5

$response = Invoke-WebRequest -Uri $url -Method Post -ContentType "application/json" -Body $body -ErrorAction SilentlyContinue

if ($null -ne $response) {
    Write-Output $response.Content
} else {
    Write-Output $Error[0].ErrorDetails.Message
}