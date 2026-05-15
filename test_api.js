const https = require('https');
const data = JSON.stringify({ contents: [{ parts: [{ text: "merhaba" }] }] });
const options = {
  hostname: 'generativelanguage.googleapis.com',
  path: '/v1beta/models/gemini-1.5-flash-latest:generateContent?key=AIzaSyBcieVrOBddmbdprNAljleS3fZ3FL1hlqM',
  method: 'POST',
  headers: { 'Content-Type': 'application/json', 'Content-Length': data.length }
};
const req = https.request(options, res => {
  let body = '';
  res.on('data', d => body += d);
  res.on('end', () => console.log("LATEST:", body.substring(0, 200)));
});
req.write(data); req.end();

const options2 = {
  hostname: 'generativelanguage.googleapis.com',
  path: '/v1beta/models/gemini-1.5-flash:generateContent?key=AIzaSyBcieVrOBddmbdprNAljleS3fZ3FL1hlqM',
  method: 'POST',
  headers: { 'Content-Type': 'application/json', 'Content-Length': data.length }
};
const req2 = https.request(options2, res => {
  let body = '';
  res.on('data', d => body += d);
  res.on('end', () => console.log("FLASH:", body.substring(0, 200)));
});
req2.write(data); req2.end();