#!/usr/bin/env node
//
// export AVAHI_COMPAT_NOWARN=1
//
var mdns = require('mdns');
var argv = require('optimist').argv;

if (argv.server) {
  var ad = mdns.createAdvertisement(mdns.tcp('speleo'), 4321);
  ad.start();
} else {
  var browser = mdns.createBrowser(mdns.tcp('speleo'));
  browser.on('serviceUp', function(service) {
    console.log('service up: ', service);
  });
  browser.on('serviceDown', function(service) {
    console.log('service down: ', service);
  });
  browser.start();
}
