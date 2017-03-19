/// <reference path="vis/dist/vis.js" />

//var container = document.getElementById('visualization');
//var items = [
//    { x: '2014-06-11', y: 10 },
//    { x: '2014-06-12', y: 25 },
//    { x: '2014-06-13', y: 30 },
//    { x: '2014-06-14', y: 10 },
//    { x: '2014-06-15', y: 15 },
//    { x: '2014-06-16', y: 30 }
//];

//var dataset = new vis.DataSet(items);
//var options = {
//    start: '2014-06-10',
//    end: '2014-06-18'
//};
//var Graph2d = new vis.Graph2d(container, dataset, options);

//console.log('Writing new values');

//items = [
//    { x: '2014-06-11', y: 101 },
//    { x: '2014-06-12', y: 252 },
//    { x: '2014-06-13', y: 304 },
//    { x: '2014-06-14', y: 106 },
//    { x: '2014-06-15', y: 157 },
//    { x: '2014-06-16', y: 309 }
//];

//dataset = new vis.DataSet(items);

//Graph2d.redraw();

$(document).ready(function () {
    $("#accordion").accordion({
        //header: 'header'
    });
});