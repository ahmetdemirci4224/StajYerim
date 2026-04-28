const https = require('https');
https.get('https://staj-yerim.vercel.app/assets/index-D9mE6wo-.js', (res) => {
    let data = '';
    res.on('data', chunk => data += chunk);
    res.on('end', () => {
        const idx = data.indexOf('https://stajyerim-api');
        console.log(data.substring(Math.max(0, idx - 20), idx + 50));
    });
});