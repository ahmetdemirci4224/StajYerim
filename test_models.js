const https = require('https');
const options = {
  hostname: 'generativelanguage.googleapis.com',
  path: '/v1beta/models?key=AIzaSyBcieVrOBddmbdprNAljleS3fZ3FL1hlqM',
  method: 'GET'
};
const req = https.request(options, res => {
  let body = '';
  res.on('data', d => body += d);
  res.on('end', () => console.log("MODELS:", body));
});
req.end();