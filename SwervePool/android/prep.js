// Prepares the APK assets: an offline copy of the game page, and the launcher icon.
const fs = require('fs');
const path = require('path');
const { chromium } = require('playwright');

const here = __dirname;
let h = fs.readFileSync(path.join(here, '..', 'swerve-pool.html'), 'utf8');
h = h.replace(/<link rel="stylesheet"[^>]*>\n/, '')
     .replace(/<meta name="claude[^>]*>\n/, '')
     .replace('https://cdnjs.cloudflare.com/ajax/libs/three.js/r128/three.min.js', 'three.min.js');
if (h.includes('cdnjs') || h.includes('googleapis')) throw new Error('external reference left in page');
const i = h.indexOf('<style>');
const page = '<!doctype html>\n<html lang="en"><head><meta charset="utf-8">' +
  '<meta name="viewport" content="width=device-width,initial-scale=1,viewport-fit=cover,user-scalable=no">\n' +
  h.slice(0, i) + '<style>body{margin:0;font-size:14px}[hidden]{display:none!important}</style>\n' +
  h.slice(i).replace('<div id="app"', '</head><body>\n<div id="app"') + '\n</body></html>\n';
fs.mkdirSync(path.join(here, 'assets'), { recursive: true });
fs.writeFileSync(path.join(here, 'assets', 'index.html'), page);
fs.copyFileSync(path.join(here, 'vendor', 'three.min.js'), path.join(here, 'assets', 'three.min.js'));

(async () => {
  const b = await chromium.launch();
  const p = await b.newPage({ viewport: { width: 192, height: 192 }, deviceScaleFactor: 1 });
  await p.setContent(`<body style="margin:0"><svg width="192" height="192" viewBox="0 0 192 192" xmlns="http://www.w3.org/2000/svg">
<defs><radialGradient id="c" cx="50%" cy="40%" r="75%"><stop offset="0" stop-color="#23895a"/><stop offset="1" stop-color="#0f4a30"/></radialGradient>
<radialGradient id="w" cx="38%" cy="32%" r="75%"><stop offset="0" stop-color="#fffdf4"/><stop offset=".6" stop-color="#e9e2cf"/><stop offset="1" stop-color="#a9a18c"/></radialGradient></defs>
<rect width="192" height="192" rx="40" fill="url(#c)"/>
<path d="M30 168 C 60 150, 60 110, 96 92" fill="none" stroke="#fff6dc" stroke-width="5" stroke-dasharray="2 11" stroke-linecap="round"/>
<ellipse cx="102" cy="104" rx="52" ry="50" fill="rgba(0,0,0,.28)"/>
<circle cx="96" cy="92" r="50" fill="url(#w)"/>
<circle cx="116" cy="92" r="11" fill="#4f8fd0" stroke="#dbe9f7" stroke-width="3"/></svg></body>`);
  const dir = path.join(here, 'res', 'mipmap-xxxhdpi');
  fs.mkdirSync(dir, { recursive: true });
  await p.screenshot({ path: path.join(dir, 'ic_launcher.png'), omitBackground: true });
  await b.close();
  console.log('assets and icon ready');
})();
