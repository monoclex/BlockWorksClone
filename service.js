// nodejs service to clean files

const clean_js = require('js-beautify').js;
const clean_css = require('js-beautify').css;
const clean_html = require('js-beautify').html;
const bodyParser = require('body-parser');
const htmlencode = require('htmlencode').htmlEncode;

var express = require('express');
var http = require('http');

var app = express();
var server = http.createServer(app);

app.use(bodyParser.urlencoded({
	limit: '2mb', extended: true
}));

app.get('/', function(req, res) {
	res.send("POST some js '");
});

app.post('/', function(req, res) {
	console.log('Recieved POST request');
	if (req.body.clean == 0)
		res.send(clean_html(req.body.src));
	else if (req.body.clean == 1)
		res.send(clean_css(req.body.src));
	else if (req.body.clean == 2)
		res.send(clean_js(req.body.src));
	else res.send("no CLEAN type specified (0, html, 1, css, 2, js)");
});

server.listen(3000);
console.log('Express server started on port %s', server.address().port);
