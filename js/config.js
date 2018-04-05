'use strict';
var http = location.protocol;
var slashes = http.concat("//");
var host = slashes.concat(window.location.hostname);
var subPathApp = '/mhm/mhm';
var subPathApi = '/mhm/';
// var hostBaseURL=host+subPathApp;
// var hostBaseURL = 'http://localhost:54182/mhm';
// //var hostAPI=hostAPI+subPathApi;
// var hostAPI = 'http://localhost:54182';

var hostBaseURL = 'http://192.168.0.36/mhm';
//var hostAPI=hostAPI+subPathApi;
var hostAPI = 'http://localhost:54182';
//var hostAPI='http://192.168.0.185/MHM_API';
// var hostAPI='http://192.168.0.36/mhm';
//var hostAPI='http://test.myhealthmath.com/api';