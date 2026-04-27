$apiKey = "AIzaSyD6Db33VrtUWgRF7Nwk587TJpUQxB3X6hs"
$url = "https://generativelanguage.googleapis.com/v1beta/models?key=$apiKey"
Invoke-RestMethod -Uri $url | ConvertTo-Json -Depth 3