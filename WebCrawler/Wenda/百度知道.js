var config=
    {
        "url": "http://zhidao.baidu.com/search?word=%BF%AA%B5%EA&ie=gbk&site=-1&sites=0&date=0&pn={0}0",
        "baseUrl": "http://zhidao.baidu.com",
        "sleep": "2000",
        "pager": true,
        "encoding": "gbk",
        "main": {
            "container": ".dl",
            "skip": "0",
            "rules": [{
                "name": "@content",
                "selector": "dt a",
                "skip": "0",
                "filter": "",
                "childLink": "href"
            }]
        },
        "child": {
            "enable": true,
            "multi": false,
            "skip": "0",
            "container": "#body",
            "rules": [{
                "name": "@answer",
                "selector": ".recommend-text,.best-text,.ec-answer,.answer-text",
                "skip": "0",
                "filter": ""
            }]
        }
    }