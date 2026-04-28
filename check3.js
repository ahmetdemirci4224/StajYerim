const https = require('https');
https.get('https://staj-yerim.vercel.app/assets/index-D9mE6wo-.js', (res) => {
    let data = '';
    res.on('data', chunk => data += chunk);
    res.on('end', () => {
        const idx = data.indexOf('https://stajyerim-api');
        const str = data.substring(idx - 20, idx + 50);
        console.log("STRING:", str);
        // Print character codes
        let codes = [];
        for(let i=0; i<str.length; i++) codes.push(str.charCodeAt(i));
        console.log("CODES:", codes.join(','));
    });
});