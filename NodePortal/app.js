'use strict';
var request = require('request');
var express = require('express');
var app = express();
var bookRouter = express.Router();
var MsSql = require('sequelize');

var sequelize = new MsSql('sqldb-ipowersaver-dev', 'ajay.kumar', 'Password$', {
    host: 'oa-iot-dev.database.windows.net',
    dialect: 'mssql',

    pool: {
        max: 5,
        min: 0,
        idle: 10000
    },
    dialectOptions: {
        encrypt: true
    }
});

var User = sequelize.define('WifiSensor', {
    Id: {
        type: MsSql.STRING,
        primaryKey: true
    },
    DeviceName: {
        type: MsSql.STRING
    },
    OperationFrequecy: {
        type: MsSql.INTEGER
    },
    IsActive: {
        type: MsSql.BOOLEAN
    }
}, {
    timestamps: false
});

var port = process.env.PORT || 5000;

app.listen(port, function (err) {
    console.log('Running at port ' + port);
});

app.use(express.static('public'));
app.set('views', './src/views');

app.set('view engine', 'ejs');

bookRouter.route('/sqlConnect')
    .get(function (req, res) {
        sequelize
            .authenticate()
            .then(function (err) {
                var users = User.findAll().then(function (users) {
                    setInterval(function () {
                        console.log(Math.random());
                    }, 5000);
                    res.send(users);
                });

            })
            .catch(function (err) {
                res.send('Unable to Connect');
            });
    });
bookRouter.route('/')
    .get(function (req, res) {

        res.send('Hello Books');
    });

bookRouter.route('/Single')
    .get(function (req, res) {
        res.send('Hello Single Book');
    });

app.use('/Books', bookRouter);

app.get('/', function (req, res) {
    res.render('index', {
        title: 'Hello from render',
        nav: [{
                Link: '/Authors',
                Text: 'Authors'
            },
            {
                Link: '/Books',
                Text: 'Books'
            }
        ]
    });
});

bookRouter.route('/')
    .get(function (req, res) {
        request({
            method: 'GET',
            url: 'https://api.powerbi.com/v1.0/myorg/dashboards',
            headers: {
                'Authorization': 'Bearer'
            }
        }, function (error, response, body) {
            console.log('Status:', response.statusCode);
            console.log('Headers:', JSON.stringify(response.headers));
            console.log('Response:', body);
        });
        res.send('Hello Books');
    });

request({
    method: 'GET',
    url: 'https://api.powerbi.com/v1.0/myorg/groups/me/dashboards',
    headers: {
        'Authorization': 'Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Il9VZ3FYR190TUxkdVNKMVQ4Y2FIeFU3Y090YyIsImtpZCI6Il9VZ3FYR190TUxkdVNKMVQ4Y2FIeFU3Y090YyJ9.eyJhdWQiOiI2NDIyOGNjNy1kZWVlLTRmMjEtYWY0My02OTllZTg0ZjhkYjAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9mMzU1MTNmZC00MGUxLTQ5YjktODYxZS01OWFmOWFmMDkxNGIvIiwiaWF0IjoxNDg4NTI1OTg3LCJuYmYiOjE0ODg1MjU5ODcsImV4cCI6MTQ4ODUyOTg4NywiYW1yIjpbInB3ZCJdLCJmYW1pbHlfbmFtZSI6IlJhbmdhIiwiZ2l2ZW5fbmFtZSI6IkFuYW5kIiwiaXBhZGRyIjoiMTgyLjc1LjIzLjI0MiIsIm5hbWUiOiJBbmFuZCBSYW5nYSIsIm5vbmNlIjoiM2EwZGVkMDctODJlOC00ODM0LWFkODctNTFhZDkzMTFhMzRlIiwib2lkIjoiMjkxOThjYzQtYTY2OC00YzlmLTlkNmUtZTY4MDI1MDcwMDc4IiwicGxhdGYiOiIzIiwic3ViIjoiT1lSNjdEdi1ZOFdCcGFFYTRxTzkyM2hpbTVkTGxDdDU2bTg0MzRiSlVrayIsInRpZCI6ImYzNTUxM2ZkLTQwZTEtNDliOS04NjFlLTU5YWY5YWYwOTE0YiIsInVuaXF1ZV9uYW1lIjoiQW5hbmQuUmFuZ2FAb25hY3R1YXRlLmNvbSIsInVwbiI6IkFuYW5kLlJhbmdhQG9uYWN0dWF0ZS5jb20iLCJ2ZXIiOiIxLjAifQ.ypANasRJe7jPf5TqJWnpKroDvXD7m1bDuo5CVYbxs5TOBewntfbHEyOjLOv8qMVSHvqJe_MQrabK40Ne5WYO9RgJBmtPbkHeEJHmLGqcbFcfXa_kUzEaPDgJoUGRGayjmRxGBfv92alsbvGTxt_0uOA2I8dljxJMnMcJUem47EpIC6xSe7cXoCnWIK2M3gEUr65bB24wErFt0k0oV1axn13yKLYvYclSLv5ttp199dz3L0C_RbJn5PcKwPMK5yUFI4kz5P_3tmyeN75fQ_Bcu1BdKcoibWThoy_OqFVSwhmh8vOVDmPT6iFnJfjxnxGl9_r9u-HS2bnu6_GTSaEhSw'
    }
}, function (error, response, body) {
    console.log('Status:', response.statusCode);
    console.log('Headers:', JSON.stringify(response.headers));
    console.log('Response:', body);
});