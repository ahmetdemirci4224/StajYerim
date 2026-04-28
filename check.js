const https = require('https');
https.get('https://staj-yerim.vercel.app/assets/index-D9mE6wo-.js', (res) => {
    let data = '';
    res.on('data', chunk => data += chunk);
    res.on('end', () => {
        const matches = data.match(/https:\/\/stajyerim-api[^\"]*/g);
        console.log(matches);
    });
});